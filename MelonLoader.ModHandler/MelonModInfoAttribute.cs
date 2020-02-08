using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonModInfoAttribute : Attribute
    {
        /// <summary>
        /// Gets the Name of the Mod.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Version of the Mod.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the Author of the Mod.
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Gets the Download Link of the Mod.
        /// </summary>
        public string DownloadLink { get; }

        public MelonModInfoAttribute(string name, string version, string author, string downloadLink = null, string modid = null)
        {
            Name = name;
            Version = version;
            Author = author;
            DownloadLink = downloadLink;
        }
    }
}