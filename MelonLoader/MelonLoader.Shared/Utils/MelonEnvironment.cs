using System.IO;
using System.Reflection;

#if NET6_0_OR_GREATER
using System;
#else
using System.Diagnostics;
#endif

namespace MelonLoader.Utils
{
    public static class MelonEnvironment
    {
        public const string OurRuntimeName =
#if NET35_OR_GREATER
            "net35";
#elif NETSTANDARD2_1_OR_GREATER
            "netstandard2.1";
#else
            "net6";
#endif
        public static string GameRootDirectory { get; private set; }
        public static string GameExecutablePath { get; private set; }
        public static string GameExecutableName { get; private set; }

        public static string MelonLoaderDirectory { get; private set; }
        public static string MelonBaseDirectory { get; private set; }

        public static string ModulesDirectory { get; private set; }
        public static string MelonsDirectory { get; private set; }
        public static string UserLibsDirectory { get; private set; }
        public static string UserDataDirectory { get; private set; }

        internal static void Initialize()
        {
            // Game based Paths
#if NET6_0_OR_GREATER
            GameExecutablePath = Environment.ProcessPath;
#else
            GameExecutablePath = Process.GetCurrentProcess().MainModule!.FileName;
#endif
            GameExecutableName = Path.GetFileNameWithoutExtension(GameExecutablePath);

            GameRootDirectory = Path.GetDirectoryName(GameExecutablePath);

            // MelonLoader based Paths
            var runtimeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var runtimeDirInfo = new DirectoryInfo(runtimeFolder);
            MelonLoaderDirectory = runtimeDirInfo.Parent!.FullName;
            MelonBaseDirectory = Directory.GetParent(MelonLoaderDirectory)!.FullName;
            ModulesDirectory = Path.Combine(MelonLoaderDirectory, "Modules");

            MelonsDirectory = Path.Combine(MelonBaseDirectory, "Melons");
            if (!Directory.Exists(MelonsDirectory))
                Directory.CreateDirectory(MelonsDirectory);

            UserDataDirectory = Path.Combine(MelonBaseDirectory, "UserData");
            if (!Directory.Exists(UserDataDirectory))
                Directory.CreateDirectory(UserDataDirectory);

            UserLibsDirectory = Path.Combine(MelonBaseDirectory, "UserLibs");
            if (!Directory.Exists(UserLibsDirectory))
                Directory.CreateDirectory(UserLibsDirectory);
        }

        internal static void PrintEnvironment()
        {
            MelonLogger.MsgDirect($"Core::BasePath = {MelonBaseDirectory}");
            MelonLogger.MsgDirect($"Game::BasePath = {GameRootDirectory}");
            MelonLogger.MsgDirect($"Game::ApplicationPath = {GameExecutablePath}");
        }
    }
}