using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonModInfoAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonModInfoAttribute : Attribute
    {
        [Obsolete("MelonLoader.MelonModInfoAttribute.SystemType is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.SystemType instead.")]
        public Type SystemType { get; }
        [Obsolete("MelonLoader.MelonModInfoAttribute.Name is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Name instead.")]
        public string Name { get; }
        [Obsolete("MelonLoader.MelonModInfoAttribute.Version is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Version instead.")]
        public string Version { get; }
        [Obsolete("MelonLoader.MelonModInfoAttribute.Author is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Author instead.")]
        public string Author { get; }
        [Obsolete("MelonLoader.MelonModInfoAttribute.DownloadLink is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.DownloadLink instead.")]
        public string DownloadLink { get; }
        [Obsolete("MelonLoader.MelonModInfoAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo instead.")]
        public MelonModInfoAttribute(Type type, string name, string version, string author, string downloadLink = null)
        {
            SystemType = type;
            Name = name;
            Version = version;
            Author = author;
            DownloadLink = downloadLink;
        }
        internal MelonInfoAttribute Convert() => new MelonInfoAttribute(SystemType, Name, Version, Author, DownloadLink);
    }
}