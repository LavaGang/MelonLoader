using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Semver;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Cpp2IL : Models.ExecutablePackage
    {
        internal Cpp2IL()
        {
            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Dumper;
#if !DEBUG
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.Info.ForceDumperVersion;
#endif
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "2022.1.0-pre-release.8";

            Name = nameof(Cpp2IL);
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "cpp2il_out");
            URL = $"https://github.com/SamboyCoding/{Name}/releases/download/{Version}/{Name}-{Version}-Windows-Netframework472.zip";
            ExeFilePath = Path.Combine(Destination, $"{Name}.exe");
            FilePath = Path.Combine(Core.BasePath, $"{Name}_{Version}.zip");
        }

        internal override bool ShouldSetup() 
            => string.IsNullOrEmpty(Config.Values.DumperVersion) 
            || !Config.Values.DumperVersion.Equals(Version);

        internal override void Cleanup() { }

        internal override void Save()
            => Save(ref Config.Values.DumperVersion);

        internal override bool Execute()
        {
            if (SemVersion.Parse(Version) <= SemVersion.Parse("2022.0.999"))
                return ExecuteOld();
            return ExecuteNew();
        }

        private bool ExecuteNew()
        {
            if (Execute(new string[] {
                MelonDebug.IsEnabled() ? "--verbose" : string.Empty,
                "--game-path",
                "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",
                "--exe-name",
                "\"" + Process.GetCurrentProcess().ProcessName + "\"",

                "--use-processor",
                "attributeinjector",
                "--output-as",
                "dummydll"

            }, false, new Dictionary<string, string>() {
                {"NO_COLOR", "1"}
            }))
                return true;

            return false;
        }

        private bool ExecuteOld()
        {
            if (Execute(new string[] {
                MelonDebug.IsEnabled() ? "--verbose" : string.Empty,
                "--game-path",
                "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",
                "--exe-name",
                "\"" + Process.GetCurrentProcess().ProcessName + "\"",

                "--skip-analysis",
                "--skip-metadata-txts",
                "--disable-registration-prompts"

            }, false, new Dictionary<string, string>() {
                {"NO_COLOR", "1"}
            }))
                return true;

            return false;
        }
    }
}
