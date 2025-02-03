using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonPluginInfoAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo instead. This will be removed in a future update.", true)]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonPluginInfoAttribute : MelonInfoAttribute
    {
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.SystemType is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.SystemType instead. This will be removed in a future update.", true)]
        new public Type SystemType => base.SystemType;
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.Name is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Name instead. This will be removed in a future update.", true)]
        new public string Name => base.Name;
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.Version is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Version instead. This will be removed in a future update.", true)]
        new public string Version => base.Version;
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.Author is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Author instead. This will be removed in a future update.", true)]
        new public string Author => base.Author;
        [Obsolete("MelonLoader.MelonPluginInfoAttribute.DownloadLink is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.DownloadLink instead. This will be removed in a future update.", true)]
        new public string DownloadLink => base.DownloadLink;
        [Obsolete("MelonLoader.MelonPluginInfoAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo instead. This will be removed in a future update.", true)]
        public MelonPluginInfoAttribute(Type type, string name, string version, string author, string downloadLink = null) : base(type, name, version, author, downloadLink) { }
    }
}