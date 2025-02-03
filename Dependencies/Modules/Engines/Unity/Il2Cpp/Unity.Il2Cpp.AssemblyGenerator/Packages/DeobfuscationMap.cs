using System;
using System.IO;
using System.Text;
using MelonLoader.Lemons.Cryptography;

namespace MelonLoader.Engine.Unity.Packages
{
    internal class DeobfuscationMap : Models.PackageBase
    {
        private static LemonSHA512 lemonSHA512 = new LemonSHA512();

        internal DeobfuscationMap(string BasePath)
        {
            Name = nameof(DeobfuscationMap);
            FilePath = Path.Combine(BasePath, $"{Name}.csv.gz");
            Destination = FilePath;
            URL = RemoteAPI.Info.MappingURL;
            Version = RemoteAPI.Info.MappingFileSHA512;
        }

        internal override bool ShouldSetup()
        {
            if (string.IsNullOrEmpty(URL))
                return false;
            if (string.IsNullOrEmpty(Version))
                return false;
            else
            {
                if (!File.Exists(FilePath))
                    return true;
                byte[] hash = lemonSHA512.ComputeHash(File.ReadAllBytes(FilePath));
                StringBuilder hashstrb = new StringBuilder(128);
                foreach (byte b in hash)
                    hashstrb.Append(b.ToString("x2"));
                string hashstr = hashstrb.ToString();
                if (!hashstr.Equals(Version, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
