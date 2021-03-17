using System;
using System.Collections.Generic;
using IllusionPlugin;
using MelonLoader;

namespace IllusionInjector
{
    [Obsolete("IllusionInjector.PluginManager is Only Here for Compatibility Reasons. Please use MelonHandler instead.")]
    public static class PluginManager
    {
        internal static List<IPlugin> _Plugins = new List<IPlugin>();
        [Obsolete("IllusionInjector.PluginManager.Plugins is Only Here for Compatibility Reasons. Please use MelonHandler.Mods instead.")]
        public static IEnumerable<IPlugin> Plugins { get => _Plugins; }
        [Obsolete("IllusionInjector.PluginManager.AppInfo is Only Here for Compatibility Reasons. Please use MelonUtils instead.")]
        public class AppInfo
        {
            [Obsolete("IllusionInjector.PluginManager.AppInfo.StartupPath is Only Here for Compatibility Reasons. Please use MelonUtils.GameDirectory instead.")]
            public static string StartupPath { get => MelonUtils.GameDirectory; }
        }
    }
}