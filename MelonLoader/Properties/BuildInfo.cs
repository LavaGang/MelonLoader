using Semver;

namespace MelonLoader.Properties
{
    public static class BuildInfo
    {
        public const string Name = "MelonLoader";
        public const string Description = "The World's First Universal Mod Loader for Unity Games compatible with both Il2Cpp and Mono.";
        public const string Author = "Lava Gang";
        public const string Company = "discord.gg/2Wn3N2P";

        public static string Version { get; private set; }
        public static SemVersion VersionNumber { get; private set; }

        static BuildInfo()
        {
            var version = typeof(BuildInfo).Assembly.GetName().Version!;
            VersionNumber = new(version.Major, version.Minor, version.Build, ((version.Revision == 0)
                ? ""
                : ("ci." + version.Revision.ToString())));
            Version = VersionNumber.ToString();
        }
    }
}