using System;

namespace MelonLoader;

[Obsolete("MelonLoader.MelonPluginGameAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame instead. This will be removed in a future version.", true)]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class MelonPluginGameAttribute : MelonGameAttribute
{
    [Obsolete("MelonLoader.MelonPluginGameAttribute.Developer is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame.Developer instead. This will be removed in a future version.", true)]
    public new string Developer => base.Developer;
    [Obsolete("MelonLoader.MelonPluginGameAttribute.GameName is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame.Name instead. This will be removed in a future version.", true)]
    public string GameName => Name;
    [Obsolete("MelonLoader.MelonPluginGameAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame instead. This will be removed in a future version.", true)]
    public MelonPluginGameAttribute(string developer = null, string gameName = null) : base(developer, gameName) { }
}