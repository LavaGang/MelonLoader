using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonPlatformAttribute : Attribute
    {
        public MelonPlatformAttribute(CompatiblePlatforms platform = CompatiblePlatforms.UNIVERSAL) => Platform = platform;

        // <summary>
        /// Enum for Melon Platform Compatibility.
        /// </summary>
        public enum CompatiblePlatforms
        {
            UNIVERSAL,
            WINDOWS,
            // ANDROID_and_QUEST,
            // LINUX,
            // MACOS,
        };

        /// <summary>
        /// Platform Compatibility of the Melon.
        /// </summary>
        public CompatiblePlatforms Platform { get; internal set; }
    }
}