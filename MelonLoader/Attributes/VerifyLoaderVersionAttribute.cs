using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class VerifyLoaderVersionAttribute : Attribute
    {
        /// <summary>
        /// Version Major of MelonLoader.
        /// </summary>
        public int Major { get; internal set; }

        /// <summary>
        /// Version Minor of MelonLoader.
        /// </summary>
        public int Minor { get; internal set; }

        /// <summary>
        /// Version Patch of MelonLoader.
        /// </summary>
        public int Patch { get; internal set; }

        /// <summary>
        /// Version Revision of MelonLoader.
        /// </summary>
        public int Revision { get; internal set; }

        /// <summary>
        /// If Version Specified is a Minimum.
        /// </summary>
        public bool IsMinimum { get; internal set; }

        public VerifyLoaderVersionAttribute(int major, int minor, int patch) { Major = major; Minor = minor; Patch = patch; }
        public VerifyLoaderVersionAttribute(int major, int minor, int patch, bool isminimum) { Major = major; Minor = minor; Patch = patch; IsMinimum = isminimum; }
        public VerifyLoaderVersionAttribute(int major, int minor, int patch, int revision) { Major = major; Minor = minor; Patch = patch; Revision = revision; }
        public VerifyLoaderVersionAttribute(int major, int minor, int patch, int revision, bool isminimum) { Major = major; Minor = minor; Patch = patch; Revision = revision; IsMinimum = isminimum; }
    }
}