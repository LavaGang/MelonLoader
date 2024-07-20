using MelonLoader.Il2CppAssemblyGenerator.Packages.Models;
using Mono.Cecil;
using MonoMod;
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

            return string.IsNullOrEmpty(Config.Values.DumperSCRSVersion)
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
            string newDest = Destination.Replace(".tmp", "");
            if (File.Exists(newDest))
                File.Delete(newDest);

            bool wasSuccess = ConvertAssembly(FilePath, newDest);

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            return wasSuccess;
        }

        internal override void Save()
            => Save(ref Config.Values.DumperSCRSVersion);

        private static bool ConvertAssembly(
            string pathIn,
            string pathOut)
        {
            bool success = false;
            CustomMonoModder mm = null;

            try
            {
                // Create MonoModder
                mm = new CustomMonoModder();
                mm.InputPath = pathIn;
                mm.OutputPath = pathOut;
                mm.ReadingMode = ReadingMode.Deferred;
                mm.MissingDependencyThrow = false;
                mm.LogVerboseEnabled = false;
                mm.CleanupEnabled = false;
                mm.PreventInline = false;
                mm.Strict = false;
                mm.PublicEverything = false;
                mm.RemovePatchReferences = true;
                mm.UpgradeMSCORLIB = MelonUtils.IsWindows;
                mm.GACEnabled = false;

                // Read Original Plugin Assembly
                mm.Read();

                // Redirect .NET Assembly Reference
                string target = MelonUtils.IsWindows ? "System.Runtime" : "mscorlib";
                string redirect = MelonUtils.IsWindows ? "mscorlib" : "System.Runtime";
                foreach (var foundRef in mm.Module.AssemblyReferences)
                    if (foundRef.Name == target)
                    {
                        foundRef.Name = redirect;
                        foundRef.Version = new Version();
                        foundRef.Attributes = 0;
                        foundRef.PublicKey = Array.Empty<byte>();
                        foundRef.PublicKeyToken = Array.Empty<byte>();
                        break;
                    }

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
