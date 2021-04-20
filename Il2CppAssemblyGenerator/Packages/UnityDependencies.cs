using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class UnityDependencies : PackageBase
    {
        internal UnityDependencies()
        {
            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_UnityDependencies;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = string.Copy(MelonUtils.GetUnityVersion());
            URL = "https://github.com/LavaGang/Unity-Runtime-Libraries/raw/master/" + Version + ".zip";
            Destination = Path.Combine(Core.BasePath, "UnityDependencies");
        }

        private void Save()
        {
            Config.Values.UnityVersion = Version;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.Values.UnityVersion) || !Config.Values.UnityVersion.Equals(Version));

        internal override bool Download()
        {
            if (!ShouldDownload())
            {
                MelonLogger.Msg("Unity Dependencies are up to date. No Download Needed.");
                return true;
            }
            MelonLogger.Msg("Downloading Unity " + Version + " Dependencies...");
            if (base.Download())
            {
                Save();
                return true;
            }
            return false;
        }
    }
}