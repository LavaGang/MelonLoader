using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MelonLoader.Utils
{
    public static class GameDirectoryManager
    {
        public static string MelonLoaderDirectory { get; internal set; }
        public static string GameRootDirectory { get; internal set; }

        public static string MelonBaseDirectory => Directory.GetParent(MelonLoaderDirectory).FullName;
        public static string DependenciesDirectory => Path.Combine(MelonLoaderDirectory, "Dependencies");
        public static string SupportModuleDirectory => Path.Combine(DependenciesDirectory, "SupportModules");
        public static string CompatibilityLayerDirectory => Path.Combine(DependenciesDirectory, "CompatibilityLayers");
        public static string Il2CppAssemblyGeneratorDirectory => Path.Combine(DependenciesDirectory, "Il2CppAssemblyGenerator");
        public static string ModsDirectory => Path.Combine(MelonBaseDirectory, "Mods");
        public static string PluginsDirectory => Path.Combine(MelonBaseDirectory, "Plugins");
        public static string UserLibsDirectory => Path.Combine(MelonBaseDirectory, "UserLibs");
        public static string UserDataDirectory => Path.Combine(MelonBaseDirectory, "UserData");

    }
}
