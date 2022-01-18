using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class DeobfuscationMap : PackageBase
    {
        internal string ObfuscationRegex = null;

        internal DeobfuscationMap()
        {
            Destination = Core.BasePath;
            NewFileName = "DeobfuscationMap.csv.gz";
            URL = RemoteAPI.Info.MappingURL;
            Version = RemoteAPI.Info.MappingFileSHA512;

            ObfuscationRegex = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceRegex;
            if (string.IsNullOrEmpty(ObfuscationRegex))
                ObfuscationRegex = RemoteAPI.Info.ObfuscationRegex;
        }

        internal void Save()
        {
            Config.Values.DeobfuscationMapHash = Version;
            Config.Save();
        }

        private bool ShouldDownload() => string.IsNullOrEmpty(Config.Values.DeobfuscationMapHash) ||
                                          !Config.Values.DeobfuscationMapHash.Equals(Version) ||
                                          !File.Exists(Path.Combine(Destination, NewFileName));

        internal override bool Download()
        {
            if (string.IsNullOrEmpty(Version) || string.IsNullOrEmpty(URL))
                return true;
            if (!ShouldDownload())
            {
                MelonLogger.Msg("Deobfuscation Map is up to date. No Download Needed.");
                return true;
            }
            MelonLogger.Msg("Downloading Deobfuscation Map...");
            if (base.Download(true))
            {
                Save();
                return true;
            }

            ThrowInternalFailure("Failed to Download Deobfuscation Map!");
            return false;
        }
    }
}
