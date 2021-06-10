using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonPluginInfoAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonPluginInfoAttribute : Attribute
    {
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.SystemType is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.SystemType instead.")]
        public Type SystemType { get; }
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.Name is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Name instead.")]
        public string Name { get; }
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.Version is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Version instead.")]
        public string Version { get; }
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.Author is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Author instead.")]
        public string Author { get; }
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.DownloadLink is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.DownloadLink instead.")]
        public string DownloadLink { get; }
        [Obsolete("MelonLoader.MelonPluginInfoAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo instead.")]
        public MelonPluginInfoAttribute(Type type, string name, string version, string author, string downloadLink = null)
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