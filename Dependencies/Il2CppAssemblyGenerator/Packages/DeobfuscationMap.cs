using MelonLoader.Lemons.Cryptography;
using System;
using System.IO;
using System.Text;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages;

internal class DeobfuscationMap : Models.PackageBase
{
    private static readonly LemonSHA512 lemonSHA512 = new();

    internal DeobfuscationMap()
    {
        Name = nameof(DeobfuscationMap);
        FilePath = Path.Combine(Core.BasePath, $"{Name}.csv.gz");
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
            var hash = lemonSHA512.ComputeHash(File.ReadAllBytes(FilePath));
            var hashstrb = new StringBuilder(128);
            foreach (var b in hash)
                hashstrb.Append(b.ToString("x2"));
            var hashstr = hashstrb.ToString();
            if (!hashstr.Equals(Version, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
