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

        internal MelonInfoAttribute(string name, string author, string version, string downloadLink, Type type)
        {
            SystemType = type;
            Name = name ?? "UNKNOWN";
            Version = version ?? "1.0.0";
            Author = author ?? "UNKNOWN";

            DownloadLink = downloadLink; // Might get Removed. Not sure yet.
        }

        /// <summary>
        /// Main MelonInfo constructor.
        /// </summary>
        /// <param name="type">The main Melon type of the Melon (for example TestMod)</param>
        /// <param name="name">Name of the Melon</param>
        /// <param name="version">Version of the Melon (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="author">Author of the Melon</param>
        /// <param name="downloadLink">URL to the download link of the mod [optional]</param>
        public MelonInfoAttribute(Type type, string name, string version, string author, string downloadLink = null) 
        {
            SystemType = type;
            Name = string.IsNullOrEmpty(name) ? "UNKNOWN" : name;
            Author = string.IsNullOrEmpty(author) ? "UNKNOWN" : author;
            DownloadLink = downloadLink; // Might get Removed. Not sure yet.

            if (!SemVersion.TryParse(name, out SemVersion semver))
                MelonLogger.Warning($"==Normal users can ignore this warning==\nMelon '{name}' by '{Author}' has version '{version}' which does not use the Semantic Versioning format. Versions using formats other than the Semantic Versioning format will not be allowed in the future versions of MelonLoader.\nFor more details, see: https://semver.org");

            Version = semver?.ToString() ?? (string.IsNullOrEmpty(version) ? "1.0.0" : version);
        }
    }
}