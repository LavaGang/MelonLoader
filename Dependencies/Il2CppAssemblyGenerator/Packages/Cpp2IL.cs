using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Semver;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Cpp2IL : Models.ExecutablePackage
    {
        internal static SemVersion NetCoreMinVersion = SemVersion.Parse("2022.1.0-pre-release.18");
        internal SemVersion VersionSem;
        private string BaseFolder;

        private static string ReleaseName =>
            MelonUtils.IsWindows ? "Windows" : MelonUtils.IsUnix ? "Linux" : "OSX";
		
        internal Cpp2IL()
        {
            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Dumper;
#if !DEBUG
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.Info.ForceDumperVersion;
#endif
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = $"2022.1.0-pre-release.18";
            VersionSem = SemVersion.Parse(Version);

            Name = nameof(Cpp2IL);
            string filename = $"{Name}.exe";

            BaseFolder = Path.Combine(Core.BasePath, Name);
            if (!Directory.Exists(BaseFolder))
                Directory.CreateDirectory(BaseFolder);

            FilePath =
                ExeFilePath =
                Destination =
                Path.Combine(BaseFolder, filename);

            OutputFolder = Path.Combine(BaseFolder, "cpp2il_out");

            URL = $"https://github.com/SamboyCoding/{Name}/releases/download/{Version}/{Name}-{Version}-{ReleaseName}.exe";
        }

        internal override bool ShouldSetup() 
            => string.IsNullOrEmpty(Config.Values.DumperVersion) 
            || !Config.Values.DumperVersion.Equals(Version);

        internal override void Cleanup() { }

        internal override void Save()
            => Save(ref Config.Values.DumperVersion);

        internal override bool Execute()
            => Execute([
                MelonDebug.IsEnabled() ? "--verbose" : string.Empty,

                "--game-path",
                "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",

                "--exe-name",
                "\"" + Process.GetCurrentProcess().ProcessName + "\"",

                "--output-as",
                "dummydll",

                "--use-processor",
                "attributeanalyzer",
                "attributeinjector",
                MelonLaunchOptions.Cpp2IL.CallAnalyzer ? "callanalyzer" : string.Empty,
                MelonLaunchOptions.Cpp2IL.NativeMethodDetector ? "nativemethoddetector" : string.Empty,
                //"deobfmap",
                //"stablenamer",

            ], false, new Dictionary<string, string>() {
                {"NO_COLOR", "1"},
                {"DOTNET_BUNDLE_EXTRACT_BASE_DIR", BaseFolder }
            });
    }
}
