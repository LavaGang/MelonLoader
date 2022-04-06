﻿using System.Diagnostics;
using System.IO;

namespace MelonLoader.Utils
{
    public static class MelonEnvironment
    {
        private const string OurRuntimeName =
#if !NET6_0
            "net35";
#else
            "net6";
#endif

        public static bool IsDotnetRuntime { get; } = OurRuntimeName == "net6";
        public static bool IsMonoRuntime { get; } = !IsDotnetRuntime;

        public static string MelonLoaderDirectory { get; internal set; }
        public static string GameRootDirectory { get; internal set; }

        public static string GameExecutablePath => Process.GetCurrentProcess().MainModule.FileName;
        public static string MelonBaseDirectory => Directory.GetParent(MelonLoaderDirectory).FullName;
        public static string DependenciesDirectory => Path.Combine(MelonLoaderDirectory, "Dependencies");
        public static string SupportModuleDirectory => Path.Combine(DependenciesDirectory, "SupportModules");
        public static string CompatibilityLayerDirectory => Path.Combine(DependenciesDirectory, "CompatibilityLayers");
        public static string Il2CppAssemblyGeneratorDirectory => Path.Combine(DependenciesDirectory, "Il2CppAssemblyGenerator");
        public static string ModsDirectory => Path.Combine(MelonBaseDirectory, "Mods");
        public static string PluginsDirectory => Path.Combine(MelonBaseDirectory, "Plugins");
        public static string UserLibsDirectory => Path.Combine(MelonBaseDirectory, "UserLibs");
        public static string UserDataDirectory => Path.Combine(MelonBaseDirectory, "UserData");
        public static string OurRuntimeDirectory => Path.Combine(MelonLoaderDirectory, OurRuntimeName);

        public static string GameExecutableName => Path.GetFileNameWithoutExtension(GameExecutablePath);
        public static string UnityGameDataDirectory => Path.Combine(GameRootDirectory, GameExecutableName + "_Data");
        public static string Il2CppDataDirectory => Path.Combine(UnityGameDataDirectory, "il2cpp_data");

        public static string MelonManagedDirectory => Path.Combine(MelonLoaderDirectory, "Managed");
    }
}
