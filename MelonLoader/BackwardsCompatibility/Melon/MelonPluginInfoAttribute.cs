using System;

namespace MelonLoader;

[Obsolete("MelonLoader.MelonPluginInfoAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo instead.")]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class MelonPluginInfoAttribute : MelonInfoAttribute
{
    [Obsolete("MelonLoader.MelonPluginInfoAttribute.SystemType is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.SystemType instead.")]
    public new Type SystemType => base.SystemType;
    [Obsolete("MelonLoader.MelonPluginInfoAttribute.Name is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Name instead.")]
    public new string Name => base.Name;
    [Obsolete("MelonLoader.MelonPluginInfoAttribute.Version is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Version instead.")]
    public new string Version => base.Version;
    [Obsolete("MelonLoader.MelonPluginInfoAttribute.Author is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.Author instead.")]
    public new string Author => base.Author;
    [Obsolete("MelonLoader.MelonPluginInfoAttribute.DownloadLink is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo.DownloadLink instead.")]
    public new string DownloadLink => base.DownloadLink;
    [Obsolete("MelonLoader.MelonPluginInfoAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonInfo instead.")]
    public MelonPluginInfoAttribute(Type type, string name, string version, string author, string downloadLink = null) : base(type, name, version, author, downloadLink) { }
}