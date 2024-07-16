#if !NET6_0

using MelonLoader.Lemons.Cryptography;
using MelonLoader.MonoInternals;
using MelonLoader.Utils;
using Mono.Cecil;
using MonoMod;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using System;
using System.IO;

namespace MelonLoader.InternalUtils
{
    // based-on: https://github.com/harbingerofme/Bepinex.Monomod.HookGenPatcher/blob/master/HookGenPatcher.cs
    // based-on: https://github.com/MonoMod/MonoMod/blob/reorganize/src/MonoMod.RuntimeDetour.HookGen/Program.cs
    internal static class MonoModHookGenerator
    {
        private const string LOG_PREFIX = "MonoMod.HookGen";
        private const string MMHOOK = "MMHOOK";
        private const string CacheTypeName = "~MonoModHookGenCache";
        private const string CacheTypeFileSizeFieldName = "FileSize";
        private const string CacheTypeFileHashFieldName = "FileHash";
        private const string CacheTypeMLVersionFieldName = "MLV";
        private static readonly LemonSHA512 sha512 = new();
        private static readonly MelonLogger.Instance logger = new(LOG_PREFIX);

        internal class CustomMonoModder : MonoModder
        {
            public override void Log(string text)
            {
                //if (!MelonDebug.IsEnabled())
                //    return;
                //logger.Msg(text);
            }
            public override void LogVerbose(string text)
            {
                //if (!MelonDebug.IsEnabled())
                //    return;
                //logger.Msg(text);
            }
        }

        internal static void Run()
        {
            if (!MelonLaunchOptions.MonoModHookGenerator.Enabled)
                return;

            logger.Msg("Checking Assemblies...");

            string hookDir = MelonEnvironment.MonoModHookDirectory;
            if (!Directory.Exists(hookDir))
                Directory.CreateDirectory(hookDir);

            MonoResolveManager.AddSearchDirectory(hookDir);
            RemoveOldFiles(hookDir);

            foreach (var filePath in Directory.GetFiles(MelonEnvironment.UnityGameManagedDirectory, "*.dll"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string hookFileName = $"{MMHOOK}.{fileName}";
                string hookFilePath = Path.Combine(hookDir, $"{hookFileName}.dll");
                ProcessFile(filePath, hookFilePath);
            }

            logger.Msg("Done!");
        }

        private static void ProcessFile(string pathIn, string pathOut)
        {
            if (CheckFileHash(pathIn, pathOut,
                out long sizeIn,
                out string hashIn))
                return;

            Generate(pathIn,
                pathOut,
                orig: true,
                privat: true,
                logVerboseEnabled: false,
                publicEverything: true,
                preventInline: true,
                //missingDependencyThrow: true,
                generateCache: true,
                cacheFileSize: sizeIn,
                cacheFileHash: hashIn);
        }

        private static void RemoveOldFiles(string hookDir)
        {
            foreach (string filePath in Directory.GetFiles(hookDir, $"{MMHOOK}.*.dll"))
            {
                string fileName = Path.GetFileName(filePath);
                string realFileName = fileName.Substring($"{MMHOOK}.".Length);
                string realFilePath = Path.Combine(MelonEnvironment.UnityGameManagedDirectory, realFileName);
                if (File.Exists(realFilePath))
                    continue;

                DeleteFile(filePath);
            }
        }

        private static void DeleteFile(string filePath, bool rethrowException = false)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                if (rethrowException)
                    throw ex;
                else
                    logger.Error(ex);
            }
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
                    TypeDefinition cacheType = hookAssembly.MainModule.GetType($"{nameof(MelonLoader)}.{CacheTypeName}");
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
                                    if (oldMLV.Equals(MelonUtils.HashCode)
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
                logger.Error(ex);
                isMatch = false;
            }

            hookAssembly?.Dispose();
            return isMatch;
        }

        private static string ReadFileHash(string filePath, out long fileSize)
        {
            fileSize = 0;

            byte[] fileData = File.ReadAllBytes(filePath);
            if (fileData == null)
                return null;

            fileSize = fileData.Length;
            return sha512.ComputeHash(fileData).ToString("x2");
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
                DeleteFile(pathOut, true);

                // Create CustomMonoModder Instance with Settings and HookGen Environment Options
                if (!string.IsNullOrEmpty(namespace_on))
                    Environment.SetEnvironmentVariable("MONOMOD_HOOKGEN_NAMESPACE", namespace_on);
                if (!string.IsNullOrEmpty(namespace_il))
                    Environment.SetEnvironmentVariable("MONOMOD_HOOKGEN_NAMESPACE_IL", namespace_il);
                if (orig)
                    Environment.SetEnvironmentVariable("MONOMOD_HOOKGEN_ORIG", "1");
                if (privat)
                    Environment.SetEnvironmentVariable("MONOMOD_HOOKGEN_PRIVATE", "1");

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

                // Add Search Directories
                mm.DependencyDirs.Add(MelonEnvironment.OurRuntimeDirectory);
                mm.DependencyDirs.Add(MelonEnvironment.UnityGameManagedDirectory);

                // Read Assembly and Map Dependencies
                mm.Read();
                mm.MapDependencies();

                // Create Hook Generator
                string hookFileName = Path.GetFileName(pathOut);
                logger.Msg($"Generating Assembly: {hookFileName}");

                HookGenerator hookGen = new HookGenerator(mm, hookFileName);
                mOut = hookGen.OutputModule;

                // Assembly Caching
                if (generateCache)
                {
                    // Create Type
                    TypeDefinition cacheType = new(nameof(MelonLoader), CacheTypeName, TypeAttributes.NotPublic | TypeAttributes.Class);
                    mOut.Types.Add(cacheType);

                    // Create MLV
                    FieldDefinition mlvField = new(CacheTypeMLVersionFieldName, FieldAttributes.Private | FieldAttributes.Literal, mOut.TypeSystem.String);
                    mlvField.Constant = MelonUtils.HashCode;
                    cacheType.Fields.Add(mlvField);

                    // Create FileSize
                    FieldDefinition fileSizeField = new(CacheTypeFileSizeFieldName, FieldAttributes.Private | FieldAttributes.Literal, mOut.TypeSystem.Int64);
                    fileSizeField.Constant = cacheFileSize;
                    cacheType.Fields.Add(fileSizeField);

                    // Create FileHash
                    FieldDefinition fileHashField = new(CacheTypeFileHashFieldName, FieldAttributes.Private | FieldAttributes.Literal, mOut.TypeSystem.String);
                    fileHashField.Constant = cacheFileHash;
                    cacheType.Fields.Add(fileHashField);
                }

                // Generate Hook Assembly
                hookGen.Generate();
                mOut.Write(pathOut);
                success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                success = false;
            }

            // Cleanup
            mm?.Dispose();
            mOut?.Dispose();

            if (!success)
                DeleteFile(pathOut);

            return success;
        }
    }
}

#endif