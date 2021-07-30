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
        public string Author { get; internal set; }

        /// <summary>
        /// Download Link of the Melon.
        /// </summary>
        public string DownloadLink { get; internal set; } // Might get Removed. Not sure yet.

        public MelonInfoAttribute(Type type, 
            string name, 
            string version, 
            string author = null, 
            string downloadLink = null // Might get Removed. Not sure yet.
        ) {
            SystemType = type;
            Name = name;
            Version = version;
            Author = author;

            DownloadLink = downloadLink; // Might get Removed. Not sure yet.
        }
    }
}