using Semver;

namespace MelonLoader;

public static class BuildInfo
{
    public const string Name = "MelonLoader";
    public const string Description = "MelonLoader";
    public const string Author = "Lava Gang";
    public const string Company = "discord.gg/2Wn3N2P";

    public static SemVersion VersionNumber { get; private set; }

    // NOTICE: This used to be a constant. Making it a property won't break backwards compatibility.
    public static string Version { get; private set; }

    static BuildInfo()
    {
        var version = typeof(BuildInfo).Assembly.GetName().Version!;
        VersionNumber = new(version.Major, version.Minor, version.Build, version.Revision == 0 ? "" : ("ci." + version.Revision.ToString()));
        Version = VersionNumber.ToString();
    }
}