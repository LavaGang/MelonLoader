using System;

namespace IllusionPlugin
{
    [Obsolete("IllusionPlugin.IEnhancedPlugin is Only Here for Compatibility Reasons. Please use MelonPlugin or MelonMod instead.")]
    public interface IEnhancedPlugin : IPlugin
    {
        [Obsolete("IllusionPlugin.IEnhancedPlugin.Filter is Only Here for Compatibility Reasons. Please use the MelonGame Attribute instead.")]
        string[] Filter { get; }
        [Obsolete("IllusionPlugin.IEnhancedPlugin.OnLateUpdate() is Only Here for Compatibility Reasons. Please use MelonPlugin.OnLateUpdate() or MelonMod.OnLateUpdate() instead.")]
        void OnLateUpdate();
    }
}