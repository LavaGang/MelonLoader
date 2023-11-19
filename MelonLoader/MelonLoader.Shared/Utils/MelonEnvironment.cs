using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MelonLoader.Shared.Utils
{
    public static class MelonEnvironment
    {
        public static string GameRootDirectory { get; private set; }
        public static string GameExecutablePath { get; private set; }
        public static string GameExecutableName { get; private set; }

        public static string MelonLoaderDirectory { get; private set; }
        public static string MelonBaseDirectory { get; private set; }

        public static string ModulesDirectory { get; private set; }
        public static string ModsDirectory { get; private set; }
        public static string PluginsDirectory { get; private set; }
        public static string UserLibsDirectory { get; private set; }
        public static string UserDataDirectory { get; private set; }

        internal static void Initialize()
        {
            // Game based Paths
#if NET6_0
            GameExecutablePath = System.Environment.ProcessPath;
#else
            GameExecutablePath = Process.GetCurrentProcess().MainModule!.FileName;
#endif
            GameExecutableName = Path.GetFileNameWithoutExtension(GameExecutablePath);

            // MelonLoader based Paths
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