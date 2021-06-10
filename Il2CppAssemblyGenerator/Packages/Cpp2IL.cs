using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class Cpp2IL : ExecutablePackageBase
    {
        internal Cpp2IL()
        {
            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Dumper;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.ReturnedInfo.ForceDumperVersion;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "commit_1aba7eee0667903dd465d897d88df15c2fe8b991";
            URL = "https://github.com/SamboyCoding/Cpp2IL/releases/download/" + Version + "/Cpp2IL-Win.exe";
            Destination = Path.Combine(Core.BasePath, "Cpp2IL");
            Output = Path.Combine(Destination, "cpp2il_out");
            ExePath = Path.Combine(Destination, "Cpp2IL-Win.exe");
        }

        private void Save()
        {
            Config.Values.DumperVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.Values.DumperVersion) || !Config.Values.DumperVersion.Equals(Version));

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

        internal bool Execute()
        {
            MelonLogger.Msg("Executing Cpp2IL...");
            return Execute(new string[] {
                "--game-path",
                "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",
                "--exe-name",
                "\"" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + "\"",
                "--skip-analysis",
                "--skip-metadata-txts",
                "--disable-registration-prompts"
            }, false);
        }
    }
}