using System;

namespace ModHelper
{
    [Obsolete("ModHelper.IMod is Only Here for Compatibility Reasons. Please use MelonPlugin or MelonMod instead.")]
    public interface IMod
    {
        [Obsolete("ModHelper.IMod.Name is Only Here for Compatibility Reasons. Please use MelonPlugin.Info.name or MelonMod.Info.name instead.")]
        string Name { get; }

        [Obsolete("ModHelper.IMod.Description is Only Here for Compatibility Reasons.")]
        string Description { get; }

        [Obsolete("ModHelper.IMod.Author is Only Here for Compatibility Reasons. Please use MelonPlugin.Info.author or MelonMod.Info.author instead.")]
        string Author { get; }

        [Obsolete("ModHelper.IMod.HomePage is Only Here for Compatibility Reasons.Please use MelonPlugin.Info.downloadLink or MelonMod.Info.downloadLink instead.")]
        string HomePage { get; }

        [Obsolete("ModHelper.IMod.DoPatching() is Only Here for Compatibility Reasons. Please use MelonBase.OnApplicationStart() instead.")]
        void DoPatching();
    }
}