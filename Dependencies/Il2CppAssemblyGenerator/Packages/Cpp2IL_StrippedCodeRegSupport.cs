using MelonLoader.Il2CppAssemblyGenerator.Packages.Models;
using MelonLoader.Utils;
using Mono.Cecil;
using MonoMod;
using MonoMod.Utils;
using Semver;
using System;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Cpp2IL_StrippedCodeRegSupport : PackageBase
    {
        private static SemVersion _minVer = SemVersion.Parse("2022.1.0-pre-release.13");
        private string _pluginsFolder;

        internal class CustomMonoModder : MonoModder
        {
            public override void Log(string text)
            {
                //if (!MelonDebug.IsEnabled())
                //    return;
                //Core.Logger.Msg(text);
            }
            public override void LogVerbose(string text)
            {
                //if (!MelonDebug.IsEnabled())
                //    return;
                //Core.Logger.Msg(text);
            }
        }

        internal Cpp2IL_StrippedCodeRegSupport(Cpp2IL cpp2IL)
        {
            Name = $"{cpp2IL.Name}.Plugin.StrippedCodeRegSupport";
            Version = cpp2IL.Version;

            string fileName = $"{Name}.dll";
            _pluginsFolder = Path.Combine(cpp2IL.Destination, "Plugins");

            FilePath = 
                Destination = 
                Path.Combine(_pluginsFolder, $"{fileName}.tmp");

            URL = $"https://github.com/SamboyCoding/{cpp2IL.Name}/releases/download/{cpp2IL.Version}/{fileName}";
        }

        internal override bool ShouldSetup()
        {
            if (SemVersion.Parse(Version) < _minVer)
                return false;

            return !Directory.Exists(_pluginsFolder)
                || !File.Exists(FilePath)
                || string.IsNullOrEmpty(Config.Values.DumperSCRSVersion)
                || !Config.Values.DumperSCRSVersion.Equals(Version);
        }

        internal override bool Setup()
        {
            if (SemVersion.Parse(Version) < _minVer)
                return true;

            if (!Directory.Exists(_pluginsFolder))
                Directory.CreateDirectory(_pluginsFolder);

            return base.Setup();
        }

        internal override bool OnProcess()
        {
            bool wasSuccess = Generate(FilePath,
                Destination.Replace(".tmp", ""),
                removePatchReferences: true,
                upgradeMSCORLIB: true);

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            return wasSuccess;
        }

        internal override void Save()
            => Save(ref Config.Values.DumperSCRSVersion);

        private static bool Generate(
            string pathIn,
            string pathOut,
            ReadingMode readingMode = ReadingMode.Deferred,
            bool missingDependencyThrow = false,
            bool logVerboseEnabled = false,
            bool cleanupEnabled = false,
            bool publicEverything = false,
            bool preventInline = false,
            bool strict = false,
            bool removePatchReferences = false,
            bool upgradeMSCORLIB = false,
            bool gacEnabled = false)
        {
            bool success = false;
            CustomMonoModder mm = null;

            try
            {
                // Create MonoModder
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

                // Add MelonLoader/Managed as a reference
                mm.DependencyDirs.Add(MelonEnvironment.MelonManagedDirectory);

                // Read Original Plugin Assembly
                mm.Read();

                // Replace System.Runtime.dll with mscorlib.dll
                AssemblyNameReference systemRuntime = null;
                foreach (var foundRef in mm.Module.AssemblyReferences)
                {
                    if (foundRef.Name != "System.Runtime")
                        continue;

                    // This should hopefully Auto-Resolve to the mscorlib.dll of the .NET Framework
                    foundRef.Name = "mscorlib";
                    foundRef.Version = new Version();
                    foundRef.Attributes = 0;
                    foundRef.PublicKey = Array.Empty<byte>();
                    foundRef.PublicKeyToken = Array.Empty<byte>();

                    break;
                }
                if (systemRuntime != null)
                    mm.Module.AssemblyReferences.Remove(systemRuntime);

                // Remove Non-Essential Assembly Attributes
                foreach (CustomAttribute att in mm.Module.CustomAttributes.ToArray())
                {
                    if (att.AttributeType.Namespace.StartsWith("Cpp2IL"))
                        continue;
                    mm.Module.CustomAttributes.Remove(att);
                }
                
                // Write new Plugin Assembly
                mm.MapDependencies();
                mm.Write(null, pathOut);

                success = true;
            }
            catch (Exception ex)
            {
                Core.Logger.Error(ex);
                success = false;
            }

            mm?.Dispose();
            return success;
        }
    }
}
