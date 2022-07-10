using Semver;
using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonInfoAttribute : Attribute
    {
        /// <summary>
        /// System.Type of the Melon.
        /// </summary>
        public Type SystemType { get; internal set; }

        /// <summary>
        /// Name of the Melon.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Version of the Melon.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// Author of the Melon.
        /// </summary>
        public string Author { get; internal set; } // This used to be optional, but is now required

        /// <summary>
        /// Download Link of the Melon.
        /// </summary>
        public string DownloadLink { get; internal set; } // Might get Removed. Not sure yet.

        public MelonInfoAttribute(Type type, string name, string author, uint versionMajor, uint versionMinor = 0, uint versionPatch = 0, string downloadLink = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(author))
                throw new ArgumentNullException(nameof(author));

            SystemType = type;
            Name = name;
            Author = author;

            Version = $"{versionMajor}.{versionMinor}.{versionPatch}";

            DownloadLink = downloadLink; // Might get Removed. Not sure yet.
        }

        internal MelonInfoAttribute(string name, string author, string version, string downloadLink, Type type)
        {
            SystemType = type;
            Name = name ?? "UNKNOWN";
            Version = version ?? "1.0.0";
            Author = author ?? "UNKNOWN";

            DownloadLink = downloadLink; // Might get Removed. Not sure yet.
        }

        [Obsolete("Use the new constructor.", true)]
        public MelonInfoAttribute(Type type, string name, string version, string author = null, string downloadLink = null) 
        {
            SystemType = type;
            Name = name ?? "UNKNOWN";
            Version = version ?? "1.0.0";
            Author = author ?? "UNKNOWN";

            DownloadLink = downloadLink; // Might get Removed. Not sure yet.
        }
    }
}