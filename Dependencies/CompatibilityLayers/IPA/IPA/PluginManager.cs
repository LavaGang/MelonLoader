using IllusionPlugin;
using MelonLoader.Utils;
using System.Collections.Generic;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IllusionInjector;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class PluginManager
{
    internal static List<IPlugin> _Plugins = [];
    public static IEnumerable<IPlugin> Plugins { get => _Plugins; }
    public class AppInfo
    {
        public static string StartupPath { get => MelonEnvironment.GameRootDirectory; }
    }
}