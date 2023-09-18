using System.Diagnostics;
using System.IO;

namespace MelonLoader.Shared.Utils
{
    public static class MelonEnvironment
    {
        public static string MelonLoaderDirectory { get; internal set; }
        public static string GameRootDirectory { get; internal set; }

#if NET6_0
        public static string GameExecutablePath => System.Environment.ProcessPath;
#else
        public static string GameExecutablePath => Process.GetCurrentProcess().MainModule!.FileName;
#endif
        public static string MelonBaseDirectory => Directory.GetParent(MelonLoaderDirectory)!.FullName;
        
        public static string ModsDirectory => Path.Combine(MelonBaseDirectory, "Mods");
        public static string PluginsDirectory => Path.Combine(MelonBaseDirectory, "Plugins");
        public static string UserLibsDirectory => Path.Combine(MelonBaseDirectory, "UserLibs");
        public static string UserDataDirectory => Path.Combine(MelonBaseDirectory, "UserData");
        public static string ModuleDirectory => Path.Combine(MelonLoaderDirectory, "Modules");
        public static string GameExecutableName => Path.GetFileNameWithoutExtension(GameExecutablePath);
    }
}