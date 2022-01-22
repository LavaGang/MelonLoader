using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class DeobfuscationMap : Models.PackageBase
    {
        internal string Regex = null;

        internal DeobfuscationMap()
        {
            Name = nameof(DeobfuscationMap);
            FilePath = Path.Combine(Core.BasePath, $"{Name}.csv.gz");
            Destination = FilePath;
            URL = RemoteAPI.Info.MappingURL;
            Version = RemoteAPI.Info.MappingFileSHA512;

            Regex = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceRegex;
            if (string.IsNullOrEmpty(Regex))
                Regex = RemoteAPI.Info.ObfuscationRegex;
        }

        internal override bool ShouldSetup()
            => !string.IsNullOrEmpty(Version)
                && (string.IsNullOrEmpty(Config.Values.DeobfuscationMapHash) 
                || !Config.Values.DeobfuscationMapHash.Equals(Version));

        internal override void Save()
        {
            Config.Values.DeobfuscationRegex = Regex;
            Version = FileHandler.Hash(Destination);
            Save(ref Config.Values.DeobfuscationMapHash);
        }
    }
}
