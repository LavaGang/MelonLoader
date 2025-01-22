using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly)]
public class MelonPlatformAttribute(params MelonPlatformAttribute.CompatiblePlatforms[] platforms) : Attribute
{
    // <summary>Enum for Melon Platform Compatibility.</summary>
    [SuppressMessage("Naming", "CA1708: Identifiers should differ by more than case", Justification = "They're deprecated")]
    [SuppressMessage("Naming", "CA1069: Enums should not have duplicate values", Justification = "They're deprecated")]
    public enum CompatiblePlatforms
    {
        Universal = 0,
        WindowsX86 = 1,
        WindowsX64 = 2,

        [Obsolete("Use Universal (lower-case) instead.", true)]
        UNIVERSAL = 0,

        [Obsolete("Use WindowsX86 (lower-case) instead.", true)]
        [SuppressMessage("Naming", "CA1707: Identifiers should not contain underscores", Justification = "It's deprecated")]
        WINDOWS_X86 = 1,

        [Obsolete("Use WindowsX64 (lower-case) instead.", true)]
        [SuppressMessage("Naming", "CA1707: Identifiers should not contain underscores", Justification = "It's deprecated")]
        WINDOWS_X64 = 2
    };

    // <summary>Platforms Compatible with the Melon.</summary>
    public CompatiblePlatforms[] Platforms { get; internal set; } = platforms;

    public bool IsCompatible(CompatiblePlatforms platform)
        => Platforms == null || Platforms.Length == 0 || Platforms.Contains(platform);
}