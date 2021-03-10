using System.IO;

namespace MelonLoader.AssemblyGenerator
{
    internal class DeobfuscationMap : PackageBase
    {
        internal string ObfuscationRegex = null;

        internal DeobfuscationMap()
        {
            Destination = Core.il2cppassemblyunhollower.Destination;
            NewFileName = "DeobfuscationMap.csv.gz";
            URL = RubyAPI.LAST_RESPONSE.mappingURL;
            Version = RubyAPI.LAST_RESPONSE.mappingFileSHA512;
            ObfuscationRegex = RubyAPI.LAST_RESPONSE.obfuscationRegex;
            if (string.IsNullOrEmpty(ObfuscationRegex))
                ObfuscationRegex = Config.ObfuscationRegex;
        }

        private void Save()
        {
            Config.DeobfuscationMapHash = Version;
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
                Logger.Msg("Deobfuscation Map is up to date. No Download Needed.");
                return true;
            }
            Logger.Msg("Downloading Deobfuscation Map...");
            if (base.Download(true))
            {
                Save();
                return true;
            }
            return false;
        }
    }
}
