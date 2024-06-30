#if !NET6_0

using MelonLoader.MonoInternals;
using MelonLoader.Utils;
using Mono.Cecil;
using MonoMod;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace MelonLoader.InternalUtils
{
    // based-on: https://github.com/harbingerofme/Bepinex.Monomod.HookGenPatcher/blob/master/HookGenPatcher.cs
    // based-on: https://github.com/MonoMod/MonoMod/blob/reorganize/src/MonoMod.RuntimeDetour.HookGen/Program.cs
    internal static class MonoModHookGenerator
    {
        private const string MMHOOK = "MMHOOK";
        private const string CacheTypeName = "~MonoModHookGenCache";
        private const string CacheTypeFileSizeFieldName = "FileSize";
        private const string CacheTypeFileHashFieldName = "FileHash";
        private const string CacheTypeMLVersionFieldName = "MLV";

        internal class CustomMonoModder : MonoModder
        {
            public override void Log(string text)
                => MelonDebug.Msg($"[MonoMod.HookGen] {text}");
            public override void LogVerbose(string text)
            {
                if (!LogVerboseEnabled)
                    return;
                MelonDebug.Msg($"[MonoMod.HookGen] {text}");
            }
        }

        internal static void Run()
        {
            if (!MelonLaunchOptions.MonoModHookGenerator.Enabled)
                return;

            MelonLogger.Msg("[MonoMod.HookGen] Checking Assemblies...");

            string hookDir = MelonEnvironment.MonoModHookDirectory;
            if (!Directory.Exists(hookDir))
                Directory.CreateDirectory(hookDir);

            MonoResolveManager.AddSearchDirectory(hookDir);

            foreach (var filePath in Directory.GetFiles(MelonEnvironment.UnityGameManagedDirectory, "*.dll"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string hookFileName = $"{MMHOOK}_{fileName}";
                string hookFilePath = Path.Combine(hookDir, $"{hookFileName}.dll");
                ProcessFile(filePath, hookFilePath);
            }

            MelonLogger.Msg("[MonoMod.HookGen] Done!");
        }

        private static void ProcessFile(string pathIn, string pathOut)
        {
            if (CheckFileHash(pathIn, pathOut,
                out long sizeIn,
                out string hashIn))
                return;

            //MelonDebug.Msg($"[MonoMod.HookGen] Generation Needed for: {Path.GetFileName(pathIn)}");

            Generate(pathIn,
                pathOut,
                orig: true,
                privat: true,
                logVerboseEnabled: false,
                publicEverything: true,
                preventInline: true,
                missingDependencyThrow: true,
                generateCache: true,
                cacheFileSize: sizeIn,
                cacheFileHash: hashIn);
        }

        private static bool CheckFileHash(string pathIn, string pathOut, 
            out long sizeIn, 
            out string hashIn)
        {
            hashIn = ReadFileHash(pathIn, out sizeIn);

            if (MelonLaunchOptions.MonoModHookGenerator.ForceRegeneration
                || !File.Exists(pathOut))
                return false;

            bool isMatch = false;
            AssemblyDefinition hookAssembly = null;
            try
            {
                hookAssembly = AssemblyDefinition.ReadAssembly(pathOut);
                if (hookAssembly != null)
                {
                    TypeDefinition cacheType = hookAssembly.MainModule.GetType(CacheTypeName);
                    if (cacheType != null)
                    {
                        FieldDefinition mlvField = cacheType.FindField(CacheTypeMLVersionFieldName);
                        if (mlvField != null)
                        {
                            FieldDefinition sizeField = cacheType.FindField(CacheTypeFileSizeFieldName);
                            if (sizeField != null)
                            {
                                FieldDefinition hashField = cacheType.FindField(CacheTypeFileHashFieldName);
                                if (hashField != null)
                                {
                                    string oldMLV = (string)mlvField.Constant;
                                    long oldSize = (long)sizeField.Constant;
                                    string oldHash = (string)hashField.Constant;
                                    if (oldMLV.Equals(BuildInfo.Version)
                                        && (oldSize == sizeIn)
                                        && oldHash.Equals(hashIn))
                                        isMatch = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                isMatch = false;
            }

            hookAssembly?.Dispose();
            return isMatch;
        }

        private static string ReadFileHash(string filePath, out long fileSize)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            fileSize = fileData.Length;
            using (SHA512 sha = SHA512.Create())
                return string.Concat(sha.ComputeHash(fileData).Select(b => b.ToString("X2")).ToArray());
        }

        private static bool Generate(
            string pathIn,
            string pathOut,
            ReadingMode readingMode = ReadingMode.Deferred,
            string namespace_on = null,
            string namespace_il = null,
            bool orig = false,
            bool privat = false,
            bool missingDependencyThrow = false,
            bool logVerboseEnabled = false,
            bool cleanupEnabled = false,
            bool publicEverything = false,
            bool preventInline = false,
            bool strict = false,
            bool removePatchReferences = false,
            bool upgradeMSCORLIB = false,
            bool gacEnabled = false,
            bool generateCache = false,
            long cacheFileSize = 0,
            string cacheFileHash = "")
        {
            bool success = false;
            CustomMonoModder mm = null;
            ModuleDefinition mOut = null;

            try
            {
                // Remove Existing File
                if (File.Exists(pathOut))
                {
                    MelonDebug.Msg($"[MonoMod.HookGen] Removing {pathOut}");
                    File.Delete(pathOut);
                }

                // Create CustomMonoModder Instance with Settings and HookGen Environment Options

                /*
                MelonDebug.Msg($"[MonoMod.HookGen] Environment Options:\n" +
                    $"  MONOMOD_HOOKGEN_ORIG: {orig}\n" +
                    $"  MONOMOD_HOOKGEN_PRIVATE: {privat}\n" +
                    (!string.IsNullOrEmpty(namespace_il)
                        ? $"  MONOMOD_HOOKGEN_NAMESPACE_IL: {namespace_il}"
                        : string.Empty));
                */

                if (!string.IsNullOrEmpty(namespace_on))
                    Environment.SetEnvironmentVariable("MONOMOD_HOOKGEN_NAMESPACE", namespace_on);
                if (!string.IsNullOrEmpty(namespace_il))
                    Environment.SetEnvironmentVariable("MONOMOD_HOOKGEN_NAMESPACE_IL", namespace_il);
                if (orig)
                    Environment.SetEnvironmentVariable("MONOMOD_HOOKGEN_ORIG", "1");
                if (privat)
                    Environment.SetEnvironmentVariable("MONOMOD_HOOKGEN_PRIVATE", "1");

                /*
                MelonDebug.Msg($"[MonoMod.HookGen] Creating MonoModder with Settings:\n" +
                    $"  InputPath: {pathIn}\n" +
                    $"  OutputPath: {pathOut}\n" +
                    $"  ReadingMode: {readingMode}\n" +
                    $"  MissingDependencyThrow: {readingMode}\n" +
                    $"  LogVerboseEnabled: {logVerboseEnabled}\n" +
                    $"  CleanupEnabled: {cleanupEnabled}\n" +
                    $"  PreventInline: {preventInline}\n" +
                    $"  Strict: {strict}\n" +
                    $"  PublicEverything: {publicEverything}\n" +
                    $"  RemovePatchReferences: {removePatchReferences}\n" +
                    $"  UpgradeMSCORLIB: {upgradeMSCORLIB}\n" +
                    $"  GACEnabled: {gacEnabled}");
                */

                mm = new CustomMonoModder();
                mm.InputPath = pathIn;
                mm.OutputPath = pathOut;
                mm.ReadingMode = readingMode;
                mm.MissingDependencyThrow = missingDependencyThrow;
                mm.LogVerboseEnabled = logVerboseEnabled;
                mm.CleanupEnabled = cleanupEnabled;
                mm.PreventInline = preventInline;
                mm.Strict = strict;
                mm.PublicEverything = publicEverything;
                mm.RemovePatchReferences = removePatchReferences;
                mm.UpgradeMSCORLIB = upgradeMSCORLIB;
                mm.GACEnabled = gacEnabled;

                // Read Assembly and Map Dependencies
                //MelonDebug.Msg($"[MonoMod.HookGen] Reading Assembly: {pathIn}");
                mm.Read();
                mm.MapDependencies();

                // Manually Map MonoMod into Cache
                mm.DependencyCache.Add("Mono.Cecil", ModuleDefinition.ReadModule(Path.Combine(MelonEnvironment.OurRuntimeDirectory, "Mono.Cecil.dll")));
                mm.DependencyCache.Add("MonoMod.RuntimeDetour", ModuleDefinition.ReadModule(Path.Combine(MelonEnvironment.OurRuntimeDirectory, "MonoMod.RuntimeDetour.dll")));
                mm.DependencyCache.Add("MonoMod.Utils", ModuleDefinition.ReadModule(Path.Combine(MelonEnvironment.OurRuntimeDirectory, "MonoMod.Utils.dll")));

                // Create Hook Generator
                //MelonDebug.Msg("[MonoMod.HookGen] Creating HookGenerator");
                string hookFileName = Path.GetFileName(pathOut);
                HookGenerator gen = new HookGenerator(mm, hookFileName);

                // Get Hook Assembly
                mOut = gen.OutputModule;

                // Assembly Caching
                if (generateCache)
                {
                    // Create Type
                    //MelonDebug.Msg($"[MonoMod.HookGen] Creating {CacheTypeName}");
                    TypeDefinition cacheType = new(string.Empty, CacheTypeName, TypeAttributes.Class | TypeAttributes.Public);
                    mOut.Types.Add(cacheType);

                    // Create ML Version
                    //MelonDebug.Msg($"[MonoMod.HookGen] Creating {CacheTypeName}.{CacheTypeMLVersionFieldName}:");
                    FieldDefinition mlvField = new(CacheTypeMLVersionFieldName, FieldAttributes.Public | FieldAttributes.Literal, mOut.TypeSystem.String);
                    mlvField.Constant = BuildInfo.Version;
                    cacheType.Fields.Add(mlvField);

                    // Create File Size
                    //MelonDebug.Msg($"[MonoMod.HookGen] Creating {CacheTypeName}.{CacheTypeFileSizeFieldName}:");
                    FieldDefinition fileSizeField = new(CacheTypeFileSizeFieldName, FieldAttributes.Public | FieldAttributes.Literal, mOut.TypeSystem.Int64);
                    fileSizeField.Constant = cacheFileSize;
                    cacheType.Fields.Add(fileSizeField);

                    // Create File Hash
                    //MelonDebug.Msg($"[MonoMod.HookGen] Creating {CacheTypeName}.{CacheTypeFileHashFieldName}:");
                    FieldDefinition fileHashField = new(CacheTypeFileHashFieldName, FieldAttributes.Public | FieldAttributes.Literal, mOut.TypeSystem.String);
                    fileHashField.Constant = cacheFileHash;
                    cacheType.Fields.Add(fileHashField);
                }

                // Generate Hook Assembly
                MelonLogger.Msg($"[MonoMod.HookGen] Generating Assembly: {hookFileName}");
                gen.Generate();
                mOut.Write(pathOut);
                success = true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                success = false;
            }

            // Cleanup
            mm?.Dispose();
            mOut?.Dispose();
            return success;
        }
    }
}

#endif