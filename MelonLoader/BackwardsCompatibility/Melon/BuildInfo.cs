using Semver;
using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.BuildInfo is Only Here for Compatibility Reasons. Please use MelonLoader.Properties.BuildInfo instead. This will be removed in a future update.", true)]
    public static class BuildInfo
    {
        [Obsolete("MelonLoader.BuildInfo.Name is Only Here for Compatibility Reasons. Please use MelonLoader.Properties.BuildInfo.Name instead. This will be removed in a future update.", true)]
        public static string Name { get => Properties.BuildInfo.Name; }
        [Obsolete("MelonLoader.BuildInfo.Description is Only Here for Compatibility Reasons. Please use MelonLoader.Properties.BuildInfo.Description instead. This will be removed in a future update.", true)]
        public static string Description { get => Properties.BuildInfo.Description; }
        [Obsolete("MelonLoader.BuildInfo.Author is Only Here for Compatibility Reasons. Please use MelonLoader.Properties.BuildInfo.Author instead. This will be removed in a future update.", true)]
        public static string Author { get => Properties.BuildInfo.Author; }
        [Obsolete("MelonLoader.BuildInfo.Company is Only Here for Compatibility Reasons. Please use MelonLoader.Properties.BuildInfo.Company instead. This will be removed in a future update.", true)]
        public static string Company { get => Properties.BuildInfo.Company; }
        [Obsolete("MelonLoader.BuildInfo.Version is Only Here for Compatibility Reasons. Please use MelonLoader.Properties.BuildInfo.Version instead. This will be removed in a future update.", true)]
        public static string Version { get => Properties.BuildInfo.Version; }
        [Obsolete("MelonLoader.BuildInfo.VersionNumber is Only Here for Compatibility Reasons. Please use MelonLoader.Properties.BuildInfo.VersionNumber instead. This will be removed in a future update.", true)]
        public static SemVersion VersionNumber { get => Properties.BuildInfo.VersionNumber; }
    }
}