using System.IO;
using System.Diagnostics;

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

#if OSX
		[System.Runtime.InteropServices.DllImport("/usr/lib/libSystem.B.dylib")]
		private static extern int _NSGetExecutablePath([System.Runtime.InteropServices.Out] byte[] buffer, ref uint size);

		private static string GetOSXGameExecutablePath() {
			// MainModule can refer to the embedding Mono runtime instead of the
			// game's executable. dyld knows the actual process executable path.
			uint size = 1024;
			var buffer = new byte[size];
			if (_NSGetExecutablePath(buffer, ref size) != 0) {
				buffer = new byte[size];
				if (_NSGetExecutablePath(buffer, ref size) != 0) {
					throw new IOException("Could not determine the game executable path.");
				}
			}
			var length = System.Array.IndexOf(buffer, (byte)0);
			if (length < 0) {
				throw new IOException("The game executable path was not null-terminated.");
			}
			var path = Path.GetDirectoryName(Path.GetFullPath(System.Text.Encoding.UTF8.GetString(buffer, 0, length)));
			while (!string.IsNullOrEmpty(path)) {
				if (path.EndsWith(".app", System.StringComparison.OrdinalIgnoreCase) &&
					Directory.Exists(Path.Combine(path, "Contents/Resources/Data"))) {
					return path;
				}
				path = Path.GetDirectoryName(path);
			}
			throw new DirectoryNotFoundException("Could not locate the Unity app bundle containing the game executable.");
		}
#endif

        public static bool IsDotnetRuntime { get; } = OurRuntimeName == "net6";
        public static bool IsMonoRuntime { get; } = !IsDotnetRuntime;

        public static string MelonBaseDirectory => LoaderConfig.Current.Loader.BaseDirectory;

        public static string GameExecutablePath { get; } =
#if OSX
            GetOSXGameExecutablePath();
#else
            Process.GetCurrentProcess().MainModule.FileName;
#endif
        public static string MelonLoaderDirectory { get; } = Path.Combine(MelonBaseDirectory, "MelonLoader");
        public static string GameRootDirectory { get; } = Path.GetDirectoryName(GameExecutablePath);


        public static string DependenciesDirectory { get; } = Path.Combine(MelonLoaderDirectory, "Dependencies");
        public static string SupportModuleDirectory { get; } = Path.Combine(DependenciesDirectory, "SupportModules");
        public static string CompatibilityLayerDirectory { get; } = Path.Combine(DependenciesDirectory, "CompatibilityLayers");
        public static string Il2CppAssemblyGeneratorDirectory { get; } = Path.Combine(DependenciesDirectory, "Il2CppAssemblyGenerator");
        public static string ModsDirectory { get; } = Path.Combine(MelonBaseDirectory, "Mods");
        public static string PluginsDirectory { get; } = Path.Combine(MelonBaseDirectory, "Plugins");
        public static string UserLibsDirectory { get; } = Path.Combine(MelonBaseDirectory, "UserLibs");
        public static string UserDataDirectory { get; } = Path.Combine(MelonBaseDirectory, "UserData");
        public static string MelonLoaderLogsDirectory { get; } = Path.Combine(MelonLoaderDirectory, "Logs");
        public static string OurRuntimeDirectory { get; } = Path.Combine(MelonLoaderDirectory, OurRuntimeName);

        public static string GameExecutableName { get; } = Path.GetFileNameWithoutExtension(GameExecutablePath);
        public static string UnityGameDataDirectory { get; } = 
#if OSX
            Path.Combine(GameExecutablePath!, "Contents/Resources/Data");
#else
            Path.Combine(GameRootDirectory, GameExecutableName + "_Data");
#endif
        public static string UnityGameManagedDirectory { get; } = Path.Combine(UnityGameDataDirectory, "Managed");
        public static string Il2CppDataDirectory { get; } = Path.Combine(UnityGameDataDirectory, "il2cpp_data");
        public static string UnityPlayerPath { get; } = Path.Combine(GameRootDirectory, "UnityPlayer.dll");

        public static string MelonManagedDirectory { get; } = Path.Combine(DependenciesDirectory, "Mono");
        public static string Il2CppAssembliesDirectory { get; } = Path.Combine(MelonLoaderDirectory, "Il2CppAssemblies");

        internal static void PrintEnvironment()
        {
            //These must not be changed, lum needs them
            MelonLogger.MsgDirect($"Core::BasePath = {MelonBaseDirectory}");
            MelonLogger.MsgDirect($"Game::BasePath = {GameRootDirectory}");
            MelonLogger.MsgDirect($"Game::DataPath = {UnityGameDataDirectory}");
            MelonLogger.MsgDirect($"Game::ApplicationPath = {GameExecutablePath}");

            MelonLogger.MsgDirect($"Runtime Type: {OurRuntimeName}");
        }
    }
}