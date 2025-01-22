using MelonLoader.Il2CppAssemblyGenerator.Packages.Models;
using Semver;
using System.IO;

namespace MelonLoader.Il2CppAssemblyGenerator.Packages;

internal class Cpp2IL_StrippedCodeRegSupport : PackageBase
{
    private static readonly SemVersion MinVersion = SemVersion.Parse("2022.1.0-pre-release.19");
    private readonly string _pluginsFolder;
    private readonly SemVersion VersionSem;

    internal Cpp2IL_StrippedCodeRegSupport(ExecutablePackage cpp2IL)
    {
        Name = $"{cpp2IL.Name}.Plugin.StrippedCodeRegSupport";
        Version = cpp2IL.Version;
        VersionSem = SemVersion.Parse(Version);

        var folderpath = Path.Combine(Core.BasePath, cpp2IL.Name);
        var fileName = $"{Name}.dll";
        _pluginsFolder = Path.Combine(folderpath, "Plugins");

        FilePath =
            Destination =
            Path.Combine(_pluginsFolder, fileName);

        URL = $"https://github.com/SamboyCoding/{cpp2IL.Name}/releases/download/{cpp2IL.Version}/{fileName}";
    }

    internal override bool ShouldSetup()
    {
        return VersionSem >= MinVersion
&& (string.IsNullOrEmpty(Config.Values.DumperSCRSVersion)
            || !Config.Values.DumperSCRSVersion.Equals(Version));
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
