using MelonLoader.Engine.Unity.Packages.Models;
using Semver;
using System.IO;

namespace MelonLoader.Engine.Unity.Packages
{
    internal class Cpp2IL_StrippedCodeRegSupport : PackageBase
    {
        private static SemVersion MinVersion = SemVersion.Parse("2022.1.0-pre-release.19");
        private string _pluginsFolder;
        private SemVersion VersionSem;

        internal Cpp2IL_StrippedCodeRegSupport(ExecutablePackage cpp2IL, string BasePath)
        {
            Name = $"{cpp2IL.Name}.Plugin.StrippedCodeRegSupport";
            Version = cpp2IL.Version;
            VersionSem = SemVersion.Parse(Version);

            string folderpath = Path.Combine(BasePath, cpp2IL.Name);
            string fileName = $"{Name}.dll";
            _pluginsFolder = Path.Combine(folderpath, "Plugins");
            if (!Directory.Exists(_pluginsFolder))
                Directory.CreateDirectory(_pluginsFolder);

            FilePath =
                Destination =
                Path.Combine(_pluginsFolder, fileName);

            URL = $"https://github.com/SamboyCoding/{cpp2IL.Name}/releases/download/{cpp2IL.Version}/{fileName}";
        }

        internal override bool ShouldSetup()
        {
            if (VersionSem < MinVersion)
                return false;

            return string.IsNullOrEmpty(Config.Values.DumperSCRSVersion)
                || !Config.Values.DumperSCRSVersion.Equals(Version);
        }

        internal override bool Setup()
        {
            if (VersionSem < Cpp2IL.NetCoreMinVersion)
                return true;

            return base.Setup();
        }

        internal override void Save()
            => Save(ref Config.Values.DumperSCRSVersion);
    }
}
