using System.IO;

namespace MelonLoader.AssemblyGenerator
{
    internal class Cpp2IL : ExecutablePackageBase
    {
        internal Cpp2IL()
        {
            Version = Utils.ForceVersion_Cpp2IL();
            if (string.IsNullOrEmpty(Version))
                Version = "commit_2d18fe2746a3dd5df13f74b8abd7b4c79eda0b0c";
            URL = "https://github.com/SamboyCoding/Cpp2IL/releases/download/" + Version + "/Cpp2IL-Win.exe";
            Destination = Path.Combine(Core.BasePath, "Cpp2IL");
            Output = Path.Combine(Destination, "cpp2il_out");
            ExePath = Path.Combine(Destination, "Cpp2IL-Win.exe");
        }

        private void Save()
        {
            Config.Cpp2ILVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.Cpp2ILVersion) || !Config.Cpp2ILVersion.Equals(Version));

        internal override void Cleanup() { }

        internal override bool Download()
        {
            if (!ShouldDownload())
            {
                Logger.Msg("Cpp2IL is up to date. No Download Needed.");
                return true;
            }
            Logger.Msg("Downloading Cpp2IL...");
            if (base.Download())
            {
                Save();
                return true;
            }
            return false;
        }

        internal bool Execute()
        {
            Logger.Msg("Executing Cpp2IL...");
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