using System;
using System.IO;

namespace MelonLoader.AssemblyGenerator
{
    internal class DeobfuscationMap : PackageBase
    {
        internal DeobfuscationMap()
        {
            Destination = Core.il2cppassemblyunhollower.Destination;
            NewFileName = "DeobfuscationMap.csv.gz";
            URL = SamboyAPI.Response_MappingURL;
            Version = SamboyAPI.Response_MappingFileSHA512;
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
