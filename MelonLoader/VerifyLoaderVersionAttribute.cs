using Semver;
using System;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly)]
public class VerifyLoaderVersionAttribute(SemVersion semver, bool isMinimum) : Attribute
{
    /// <summary>
    /// Specified SemVersion.
    /// </summary>
    public SemVersion SemVer { get; private set; } = semver;

    /// <summary>
    /// Specified Version Major.
    /// </summary>
    public int Major => SemVer.Major;

    /// <summary>
    /// Specified Version Minor.
    /// </summary>
    public int Minor => SemVer.Minor;

    /// <summary>
    /// Specified Version Patch.
    /// </summary>
    public int Patch => SemVer.Patch;

    /// <summary>
    /// Specified Version Prerelease.
    /// </summary>
    public string Prerelease => SemVer.Prerelease;

    /// <summary>
    /// If Version Specified is a Minimum.
    /// </summary>
    public bool IsMinimum { get; private set; } = isMinimum;

    public VerifyLoaderVersionAttribute(int major, int minor, int patch) : this(new SemVersion(major, minor, patch), false) { }
    public VerifyLoaderVersionAttribute(int major, int minor, int patch, string prerelease, bool isMinimum = false) : this(new SemVersion(major, minor, patch, prerelease), isMinimum) { }
    public VerifyLoaderVersionAttribute(int major, int minor, int patch, bool isMinimum) : this(new SemVersion(major, minor, patch), isMinimum) { }
    public VerifyLoaderVersionAttribute(string version) : this(version, false) { }
    public VerifyLoaderVersionAttribute(string version, bool isMinimum) : this(SemVersion.Parse(version), isMinimum) { }

    public bool IsCompatible(SemVersion version)
        => SemVer == null || version == null || (IsMinimum ? SemVer <= version : SemVer == version);

    public bool IsCompatible(string version)
        => !SemVersion.TryParse(version, out var ver) || IsCompatible(ver);
}