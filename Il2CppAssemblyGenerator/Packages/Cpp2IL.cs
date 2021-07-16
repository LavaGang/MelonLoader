using System.Collections.Generic;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Cpp2IL : DumperBase
    {
        internal Cpp2IL()
        {
            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Dumper;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.Info.ForceDumperVersion;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "2021.1.2";
            string exe_name = $"Cpp2IL-{Version}-Windows.exe";
            URL = $"https://github.com/SamboyCoding/Cpp2IL/releases/download/{Version}/{exe_name}";
            Destination = Path.Combine(Core.BasePath, "Cpp2IL");
            Output = Path.Combine(Destination, "cpp2il_out");
            ExePath = Path.Combine(Destination, exe_name);
        }

        private void Save()
        {
            Config.Values.DumperVersion = Version;
            Config.Values.DumperIsCpp2IL = true;
            Config.Save();
        }

        private bool ShouldDownload() => (
            !Config.Values.DumperIsCpp2IL
            || string.IsNullOrEmpty(Config.Values.DumperVersion)
            || !Config.Values.DumperVersion.Equals(Version));

        internal override void Cleanup() { }

        internal override bool Download()
        {
            if (!ShouldDownload())
            {
                MelonLogger.Msg("Cpp2IL is up to date. No Download Needed.");
                return true;
            }
            MelonLogger.Msg("Downloading Cpp2IL...");
            if (base.Download())
            {
                Save();
                return true;
            }
            return false;
        }

        internal override bool Execute()
        {
            MelonLogger.Msg("Executing Cpp2IL...");
            return Execute(new string[] {
                MelonDebug.IsEnabled() ? "--verbose" : string.Empty,
                "--game-path",
                "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",
                "--exe-name",
                "\"" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + "\"",
                "--skip-analysis",
                "--skip-metadata-txts",
                "--disable-registration-prompts"
            }, false, new Dictionary<string, string>() {
                {"NO_COLOR", "1"}
            });
        }
    }
}