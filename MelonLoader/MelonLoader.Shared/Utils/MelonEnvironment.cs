using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MelonLoader.Shared.Utils
{
    public static class MelonEnvironment
    {
        public static string GameRootDirectory { get; internal set; }
        public static string GameExecutablePath { get; internal set; }
        public static string GameExecutableName { get; internal set; }

        public static string MelonLoaderDirectory { get; internal set; }
        public static string MelonBaseDirectory { get; internal set; }

        public static string ModulesDirectory { get; internal set; }
        public static string ModsDirectory { get; internal set; }
        public static string PluginsDirectory { get; internal set; }
        public static string UserLibsDirectory { get; internal set; }
        public static string UserDataDirectory { get; internal set; }

        internal static void Initialize()
        {
            // Game based Paths
#if NET6_0
            GameExecutablePath = System.Environment.ProcessPath;
#else
            GameExecutablePath = Process.GetCurrentProcess().MainModule!.FileName;
#endif
            GameExecutableName = Path.GetFileNameWithoutExtension(GameExecutablePath);

            // MelonLoader based Pathed
            var runtimeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var runtimeDirInfo = new DirectoryInfo(runtimeFolder);
            MelonLoaderDirectory = runtimeDirInfo.Parent!.FullName;
            MelonBaseDirectory = Directory.GetParent(MelonLoaderDirectory)!.FullName;
            ModulesDirectory = Path.Combine(MelonLoaderDirectory, "Modules");
            ModsDirectory = Path.Combine(MelonBaseDirectory, "Mods");
            PluginsDirectory = Path.Combine(MelonBaseDirectory, "Plugins");
            UserLibsDirectory = Path.Combine(MelonBaseDirectory, "UserLibs");
            UserDataDirectory = Path.Combine(MelonBaseDirectory, "UserData");
        }
    }
}