using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator
{
    internal class DeobfuscationMap : PackageBase
    {
        internal string ObfuscationRegex = null;

        internal DeobfuscationMap()
        {
            Destination = Core.il2cppassemblyunhollower.Destination; // Change Me
            NewFileName = "DeobfuscationMap.csv.gz";
            URL = RemoteAPI.LAST_RESPONSE.MappingURL;
            Version = RemoteAPI.LAST_RESPONSE.MappingFileSHA512;
            ObfuscationRegex = RemoteAPI.LAST_RESPONSE.ObfuscationRegex;
            if (string.IsNullOrEmpty(ObfuscationRegex))
                ObfuscationRegex = Config.ObfuscationRegex;
            if (string.IsNullOrEmpty(ObfuscationRegex) && Core.GameName.Equals("Among Us"))
                ObfuscationRegex = "[A-Z]{11}";
        }

        internal void Save()
        {
            Config.DeobfuscationMapHash = Version;
            Config.ObfuscationRegex = ObfuscationRegex;
            Config.Save();
        }

        private bool ShouldDownload() => (string.IsNullOrEmpty(Config.DeobfuscationMapHash) ||
                                          !Config.DeobfuscationMapHash.Equals(Version) ||
                                          !File.Exists(Path.Combine(Destination, NewFileName)));

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
            return false;
        }
    }
}
