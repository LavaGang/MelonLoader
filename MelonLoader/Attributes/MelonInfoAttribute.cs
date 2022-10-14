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
        /// Semantic Version of the Melon. Will be null if Version is not using the <see href="https://semver.org">Semantic Versioning</see> format.
        /// </summary>
        public SemVersion SemanticVersion { get; internal set; }

        /// <summary>
        /// Author of the Melon.
        /// </summary>
        public string Author { get; internal set; } // This used to be optional, but is now required

        /// <summary>
        /// Download Link of the Melon.
        /// </summary>
        public string DownloadLink { get; internal set; } // Might get Removed. Not sure yet.

        /// <summary>
        /// MelonInfo constructor.
        /// </summary>
        /// <param name="type">The main Melon type of the Melon (for example TestMod)</param>
        /// <param name="name">Name of the Melon</param>
        /// <param name="version">Version of the Melon</param>
        /// <param name="author">Author of the Melon</param>
        /// <param name="downloadLink">URL to the download link of the mod [optional]</param>
        public MelonInfoAttribute(Type type, string name, string version, string author, string downloadLink = null) 
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            SystemType = type;
            Name = name ?? "UNKNOWN";
            Author = author ?? "UNKNOWN";
            DownloadLink = downloadLink; // Might get Removed. Not sure yet.

            if (string.IsNullOrEmpty(version))
                Version = "1.0.0";
            else
                Version = version;

            if (SemVersion.TryParse(Version, out SemVersion semver))
                SemanticVersion = semver;
        }

        /// <summary>
        /// MelonInfo constructor.
        /// </summary>
        /// <param name="type">The main Melon type of the Melon (for example TestMod)</param>
        /// <param name="name">Name of the Melon</param>
        /// <param name="version">Version of the Melon (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="author">Author of the Melon</param>
        /// <param name="downloadLink">URL to the download link of the mod [optional]</param>
        public MelonInfoAttribute(Type type, string name, SemVersion version, string author, string downloadLink = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (version == null)
                throw new ArgumentNullException(nameof(version));

            SystemType = type;
            Name = name ?? "UNKNOWN";
            Version = version.ToString();
            Author = author ?? "UNKNOWN";
            DownloadLink = downloadLink; // Might get Removed. Not sure yet.
        }
        /// <summary>
        /// MelonInfo constructor.
        /// </summary>
        /// <param name="type">The main Melon type of the Melon (for example TestMod)</param>
        /// <param name="name">Name of the Melon</param>
        /// <param name="version">Version of the Melon (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="author">Author of the Melon</param>
        /// <param name="downloadLink">URL to the download link of the mod [optional]</param>
        public MelonInfoAttribute(Type type, string name, (int, int, int) version, string author, string downloadLink = null)
            : this(type, name, (version.Item1, version.Item2, version.Item3, null), author, downloadLink) { }

        /// <summary>
        /// MelonInfo constructor.
        /// </summary>
        /// <param name="type">The main Melon type of the Melon (for example TestMod)</param>
        /// <param name="name">Name of the Melon</param>
        /// <param name="version">Version of the Melon (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="author">Author of the Melon</param>
        /// <param name="downloadLink">URL to the download link of the mod [optional]</param>
        public MelonInfoAttribute(Type type, string name, (int, int, int, string) version, string author, string downloadLink = null)
            : this(type, name, $"{version.Item1}.{version.Item2}.{version.Item3}{(string.IsNullOrEmpty(version.Item4) ? "" : version.Item4)}", author, downloadLink) { }
    }
}