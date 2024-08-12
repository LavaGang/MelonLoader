using MelonLoader.Il2CppAssemblyGenerator.Packages.Models;
using Semver;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Cpp2IL_StrippedCodeRegSupport : PackageBase
    {
        private string _pluginsFolder;
        private SemVersion VersionSem;

        internal Cpp2IL_StrippedCodeRegSupport(Cpp2IL cpp2IL)
        {
            Name = $"{cpp2IL.Name}.Plugin.StrippedCodeRegSupport";
            Version = cpp2IL.Version;
            VersionSem = SemVersion.Parse(Version);

            string folderpath = Path.Combine(Core.BasePath, cpp2IL.Name);
            string fileName = $"{Name}.dll";
            _pluginsFolder = Path.Combine(folderpath, "Plugins");

            FilePath =
                Destination =
                Path.Combine(_pluginsFolder, fileName);

            URL = $"https://github.com/SamboyCoding/{cpp2IL.Name}/releases/download/{cpp2IL.Version}/{fileName}";
        }

        internal override bool ShouldSetup()
        {
            if (VersionSem < Cpp2IL.NetCoreMinVersion)
                return false;

            return string.IsNullOrEmpty(Config.Values.DumperSCRSVersion)
                || !Config.Values.DumperSCRSVersion.Equals(Version);
        }

        internal override bool Setup()
        {
            if (VersionSem < Cpp2IL.NetCoreMinVersion)
                return true;

            if (!Directory.Exists(_pluginsFolder))
                Directory.CreateDirectory(_pluginsFolder);

            return base.Setup();
        }

        internal override void Save()
            => Save(ref Config.Values.DumperSCRSVersion);
    }
}
