using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class DeobfuscationMap : Models.PackageBase
    {
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
                if (string.IsNullOrEmpty(Config.Values.DeobfuscationMapHash))
                    return true;
                if (!Config.Values.DeobfuscationMapHash.Equals(Version))
                    return true;
            }
            return false;
        }

        internal override void Save() => Save(ref Config.Values.DeobfuscationMapHash);
    }
}
