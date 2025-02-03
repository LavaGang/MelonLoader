using System.IO;

namespace MelonLoader.Engine.Unity.Packages
{
    internal class UnityDependencies : Models.PackageBase
    {
        internal UnityDependencies(string BasePath)
        {
            Name = nameof(UnityDependencies);
            Version = UnityInformationHandler.EngineVersion.ToStringWithoutType();
            URL = $"https://github.com/LavaGang/MelonLoader.UnityDependencies/releases/download/{Version}/Managed.zip";
            Destination = Path.Combine(BasePath, Name);
            FilePath = Path.Combine(BasePath, $"{Name}_{Version}.zip");
        }

        internal override bool ShouldSetup()
            => string.IsNullOrEmpty(Config.Values.UnityVersion)
            || !Config.Values.UnityVersion.Equals(Version);

        internal override void Save()
            => Save(ref Config.Values.UnityVersion);
    }
}
