using System;
using System.Linq;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonPlatformAttribute : Attribute
    {
        public MelonPlatformAttribute(params CompatiblePlatforms[] platforms) => Platforms = platforms;

        // <summary>Enum for Melon Platform Compatibility.</summary>
        public enum CompatiblePlatforms
        {
            UNIVERSAL,
            WINDOWS_X86,
            WINDOWS_X64
        };

        // <summary>Platforms Compatible with the Melon.</summary>
        public CompatiblePlatforms[] Platforms { get; internal set; }

        public bool IsCompatible(CompatiblePlatforms platform)
            => Platforms == null || Platforms.Length == 0 || Platforms.Contains(platform);
    }
}