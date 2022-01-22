using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class UnityDependencies : Models.PackageBase
    {
        internal UnityDependencies()
        {
            Name = nameof(UnityDependencies);

            Version = MelonLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_UnityDependencies;
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = string.Copy(MelonUtils.GetUnityVersion());

            URL = $"https://github.com/LavaGang/Unity-Runtime-Libraries/raw/master/{Version}.zip";
            Destination = Path.Combine(Core.BasePath, Name);
            FilePath = Path.Combine(Core.BasePath, $"{Name}_{Version}.zip");
        }

        internal override bool ShouldSetup()
            => string.IsNullOrEmpty(Config.Values.UnityVersion)
            || !Config.Values.UnityVersion.Equals(Version);

        internal override void Save()
            => Save(ref Config.Values.UnityVersion);
    }
}
