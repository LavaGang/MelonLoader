using System;
using Semver;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class VerifyLoaderVersionAttribute : Attribute
    {
        /// <summary>
        /// Specified SemVersion.
        /// </summary>
        public SemVersion SemVer { get; internal set; }

        /// <summary>
        /// Specified Version Major.
        /// </summary>
        public int Major { get; }

        /// <summary>
        /// Specified Version Minor.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        /// Specified Version Patch.
        /// </summary>
        public int Patch { get; }

        /// <summary>
        /// Specified Version Revision.
        /// </summary>
        public int Revision { get; }

        /// <summary>
        /// If Version Specified is a Minimum.
        /// </summary>
        public bool IsMinimum { get; internal set; }


        public VerifyLoaderVersionAttribute(int major, int minor, int patch) : this($"{major}.{minor}.{patch}.0", false) { }
        public VerifyLoaderVersionAttribute(string version) : this(version, false) { }
        public VerifyLoaderVersionAttribute(string version, bool is_minimum)
        {
            SemVer = SemVersion.Parse(version);
            IsMinimum = is_minimum;
        }

        /*
        public VerifyLoaderVersionAttribute(int major, int minor, int patch) { Major = major; Minor = minor; Patch = patch; }
        public VerifyLoaderVersionAttribute(int major, int minor, int patch, bool isminimum) { Major = major; Minor = minor; Patch = patch; IsMinimum = isminimum; }
        public VerifyLoaderVersionAttribute(int major, int minor, int patch, int revision) { Major = major; Minor = minor; Patch = patch; Revision = revision; }
        public VerifyLoaderVersionAttribute(int major, int minor, int patch, int revision, bool isminimum) { Major = major; Minor = minor; Patch = patch; Revision = revision; IsMinimum = isminimum; }
        */
    }
}