using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MelonLoader.Utils;
using Semver;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Cpp2IL : Models.ExecutablePackage
    {
        internal static SemVersion NetCoreMinVersion = SemVersion.Parse("2022.1.0-pre-release.18");
        internal SemVersion VersionSem;

        internal static string _latestVersion = "2022.1.0-pre-release.21";

        private static bool _shouldUseNightly = true;
        private static string _nightlyVersion = "2022.1.0-pre-release.22";
        private static string _nightlyRunId = "34694167702";

        private static string ReleaseName =>
            MelonUtils.IsWindows ? "Windows" : MelonUtils.IsUnix ? "Linux" : "OSX";
        
        internal readonly bool IsUsingNightly;
		
        internal Cpp2IL()
        {
            Version = LoaderConfig.Current.UnityEngine.ForceIl2CppDumperVersion;
#if !DEBUG
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.GetRemoteOrCachedValue(
                    RemoteAPI.Info.ForceDumperVersion,
                    Config.Values.DumperVersion);
#endif
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
            {
                if (!_shouldUseNightly)
                    Version = _latestVersion;
                else
                {
                    Version = _nightlyVersion;
                    IsUsingNightly = true;
                }
            }

            VersionSem = SemVersion.Parse(Version);

            Name = nameof(Cpp2IL);
            
            var filename = Name;
            if (MelonUtils.IsWindows)
                filename += ".exe";

            var baseFolder = Path.Combine(Core.BasePath, Name);
            Directory.CreateDirectory(baseFolder);
            
            if (IsUsingNightly)
            {
                Destination = baseFolder;
                
                ExeFilePath = Path.Combine(Destination, Name);
                if (MelonUtils.IsWindows)
                    ExeFilePath += ".exe";
                
                FilePath = Path.Combine(Core.BasePath, $"{Name}_{Version}.zip");
            }
            else
            {
                FilePath =
                    ExeFilePath =
                        Destination =
                            Path.Combine(baseFolder, filename);
            }

            OutputFolder = Path.Combine(baseFolder, "cpp2il_out");

            if (IsUsingNightly)
            {
                string downloadPackage = "Cpp2IL-net9-";
                if (MelonUtils.IsWindows)
                    downloadPackage += "win-";
                else if (MelonUtils.IsMac)
                    downloadPackage += "osx-";
                else
                    downloadPackage += "linux-";
#if X64
                downloadPackage += "x64"; 
#elif ARM64
                downloadPackage += "arm64";
#endif
                downloadPackage += ".zip";
                URL =
                    $"https://nightly.link/SamboyCoding/{Name}/actions/runs/{_nightlyRunId}/{downloadPackage}";
            }
            else
            {
                URL =
                    $"https://github.com/SamboyCoding/{Name}/releases/download/{Version}/{Name}-{Version}-{ReleaseName}";
                if (MelonUtils.IsWindows)
                    URL += ".exe";
            }
        }

        internal override bool ShouldSetup()
        {
            if (!File.Exists(ExeFilePath))
                return true;

            return string.IsNullOrEmpty(Config.Values.DumperVersion)
                || !Config.Values.DumperVersion.Equals(Version);
        }

        internal override void Save()
            => Save(ref Config.Values.DumperVersion);

#if OSX
        internal override bool OnProcess()
        {
            bool processed = base.OnProcess();
            if (!processed)
                return false;

            var process = Process.Start("chmod", $"+x \"{FilePath}\"");
            process?.WaitForExit();
            if (process?.ExitCode != 0)
            {
                Core.Logger.Error($"Failed to set the needed permissions on Cpp2IL, please run the following command " +
                                  $"and try again: chmod +x {FilePath}");
                return false;
            }
            return true;
        }
#endif

        internal override bool Execute()
            => Execute([
                MelonDebug.IsEnabled() ? "--verbose" : string.Empty,

                "--game-path",
#if OSX
                "\"" + MelonUtils.GetPathAncestor(Core.GameAssemblyPath, 3) + "\"",
#else
                "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",
#endif

                "--exe-name",
                "\"" + Process.GetCurrentProcess().ProcessName + "\"",

                "--output-as",
                "dummydll",

                "--use-processor",
                "attributeanalyzer",
                "attributeinjector",
                LoaderConfig.Current.UnityEngine.EnableCpp2ILCallAnalyzer ? "callanalyzer" : string.Empty,
                LoaderConfig.Current.UnityEngine.EnableCpp2ILNativeMethodDetector ? "nativemethoddetector" : string.Empty,
                //"deobfmap",
                //"stablenamer",

            ], false, new Dictionary<string, string>() {
                {"NO_COLOR", "1"},
            });
    }
}
