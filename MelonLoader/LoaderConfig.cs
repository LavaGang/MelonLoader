using System.Runtime.InteropServices;
using Tomlet.Attributes;

#if BOOTSTRAP
using Tomlet;
using MelonLoader.Bootstrap.Utils;
using System.Diagnostics.CodeAnalysis;
#endif

namespace MelonLoader;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public class LoaderConfig
{
#if BOOTSTRAP

#if OSX
    private static string GetParentDirectory(string path, int level)
    {
        string parentPath = path;
        for (int i = 0; i < level; i++)
            parentPath = Path.GetDirectoryName(parentPath)!;
        return parentPath;
    }
#endif

    [RequiresDynamicCode("Dynamically accesses LoaderConfig properties")]
    internal static void Initialize()
    {
        var customBaseDir = ArgParser.GetValue("melonloader.basedir");
        var baseDir = Path.GetDirectoryName(Environment.ProcessPath)!;

#if OSX
        baseDir = GetParentDirectory(baseDir, 3);
#endif

        if (Directory.Exists(customBaseDir))
            baseDir = Path.GetFullPath(customBaseDir);

        var userDataFolder = Path.Combine(baseDir, "UserData");
        if (!Directory.Exists(userDataFolder))
            Directory.CreateDirectory(userDataFolder);

        var path = Path.Combine(userDataFolder, "Loader.cfg");
        if (File.Exists(path))
        {
            try
            {
                var doc = TomlParser.ParseFile(path);
                Current = TomletMain.To<LoaderConfig>(doc) ?? new();
            }
            catch { }
        }

        var doc2 = TomletMain.TomlStringFrom(Current);
        try
        {
            File.WriteAllText(path, doc2);
        }
        catch { }

        CoreConfig.Initialize(baseDir);
        ConsoleConfig.Initialize();
        LogsConfig.Initialize();
        MonoDebugServerConfig.Initialize();
        UnityEngineConfig.Initialize();
    }
#endif

    public static LoaderConfig Current { get; internal set; } = new();

    [TomlProperty("loader")]
    public CoreConfig Loader { get; internal set; } = new();

    [TomlProperty("console")]
    public ConsoleConfig Console { get; internal set; } = new();

    [TomlProperty("logs")]
    public LogsConfig Logs { get; internal set; } = new();

    [TomlProperty("mono_debug_server")]
    public MonoDebugServerConfig MonoDebugServer { get; internal set; } = new();

    [TomlProperty("unityengine")]
    public UnityEngineConfig UnityEngine { get; internal set; } = new();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class CoreConfig
    {
#if BOOTSTRAP
        [RequiresDynamicCode("Dynamically accesses LoaderConfig properties")]
        internal static void Initialize(string baseDir)
        {
            Current.Loader.BaseDirectory = baseDir;

#if !DEBUG
            if (ArgParser.IsDefined("melonloader.debug"))
#endif
                Current.Loader.DebugMode = true;

            if (ArgParser.IsDefined("melonloader.captureplayerlogs"))
                Current.Loader.CapturePlayerLogs = true;

            if (int.TryParse(ArgParser.GetValue("melonloader.harmonyloglevel"), out var harmonyLogLevel))
                Current.Loader.HarmonyLogLevel = (HarmonyLogVerbosity)Math.Clamp(harmonyLogLevel, (int)HarmonyLogVerbosity.None, (int)HarmonyLogVerbosity.IL);

            if (ArgParser.IsDefined("no-mods"))
                Current.Loader.Disable = true;

            if (ArgParser.IsDefined("quitfix"))
                Current.Loader.ForceQuit = true;

            if (ArgParser.IsDefined("melonloader.disablestartscreen"))
                Current.Loader.DisableStartScreen = true;

            if (ArgParser.IsDefined("melonloader.launchdebugger"))
                Current.Loader.LaunchDebugger = true;

            if (int.TryParse(ArgParser.GetValue("melonloader.consolemode"), out var valueint))
                Current.Loader.Theme = (LoaderTheme)Math.Clamp(valueint, (int)LoaderTheme.Normal, (int)LoaderTheme.Lemon);

            if (ArgParser.IsDefined("melonloader.nosfload"))
                Current.Loader.DisableSubFolderLoad = true;

            if (ArgParser.IsDefined("melonloader.nosfmanifest"))
                Current.Loader.DisableSubFolderManifest = true;

            string? hostfxrPath = ArgParser.GetValue("melonloader.hostfxr");
            if (hostfxrPath != null)
                Current.Loader.HostFXRPathOverride = hostfxrPath;
        }
#endif

            [TomlNonSerialized]
        public string BaseDirectory { get; internal set; } = null!;

        // Technically, this will always return false, but it's still a config ¯\_(ツ)_/¯
        [TomlProperty("disable")]
        [TomlPrecedingComment("Disables MelonLoader. Equivalent to the '--no-mods' launch option")]
        public bool Disable { get; internal set; }

        [TomlProperty("debug_mode")]
        [TomlPrecedingComment("Equivalent to the '--melonloader.debug' launch option")]
        public bool DebugMode { get; internal set; }

        [TomlProperty("capture_player_logs")]
        [TomlPrecedingComment("Capture all Unity player logs into MelonLoader's logs even if the game disabled them. NOTE: Depending on the game or Unity version, these logs can be overly verbose. Equivalent to the '--melonloader.captureplayerlogs' launch option")]
        public bool CapturePlayerLogs { get; internal set; }

        [TomlProperty("harmony_log_level")]
        [TomlPrecedingComment("The maximum Harmony log verbosity to capture into MelonLoader's logs. Possible values in verbosity order are: \"None\", \"Error\", \"Warn\", \"Info\", \"Debug\", or \"IL\". Equivalent to the '--melonloader.harmonyloglevel' launch option")]
        public HarmonyLogVerbosity HarmonyLogLevel { get; internal set; } = HarmonyLogVerbosity.Warn;

        [TomlProperty("force_quit")]
        [TomlPrecedingComment("Only use this if the game freezes when trying to quit. Equivalent to the '--quitfix' launch option")]
        public bool ForceQuit { get; internal set; }

        [TomlProperty("disable_start_screen")]
        [TomlPrecedingComment("Disables the start screen. Equivalent to the '--melonloader.disablestartscreen' launch option")]
        public bool DisableStartScreen { get; internal set; }

        [TomlProperty("launch_debugger")]
        [TomlPrecedingComment("Starts the dotnet debugger on Windows and wait it is attached or just wait until one is attached without launch on other OSes (only for Il2Cpp games). Equivalent to the '--melonloader.launchdebugger' launch option")]
        public bool LaunchDebugger { get; internal set; }

        [TomlProperty("theme")]
        [TomlPrecedingComment("Sets the loader theme. Currently, the only available themes are \"Normal\" and \"Lemon\". Equivalent to the '--melonloader.consolemode' launch option (0 for Normal, 4 for Lemon)")]
        public LoaderTheme Theme { get; internal set; }

        [TomlProperty("disable_subfolder_load")]
        [TomlPrecedingComment("Disables the loading of Melon Subfolders. Equivalent to the '--melonloader.nosfload' launch option")]
        public bool DisableSubFolderLoad { get; internal set; }

        [TomlProperty("disable_subfolder_manifest")]
        [TomlPrecedingComment("Disables the requirement of needing a manifest json inside a Melon Subfolder for it to be loaded. Equivalent to the '--melonloader.nosfmanifest' launch option")]
        public bool DisableSubFolderManifest { get; internal set; }

        [TomlProperty("hostfxr_path_override")]
        [TomlPrecedingComment("Manually defines the HostFXR path to use for DotNet Initialization. Equivalent to the '--melonloader.hostfxr' launch option")]
        public string HostFXRPathOverride { get; internal set; } = "";

        public enum LoaderTheme
        {
            Normal,
            Magenta,
            Rainbow,
            RandomRainbow,
            Lemon
        }

        public enum HarmonyLogVerbosity
        {
            None,
            Info,
            Warn,
            Error,
            Debug,
            IL
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class ConsoleConfig
    {
#if BOOTSTRAP
        [RequiresDynamicCode("Dynamically accesses LoaderConfig properties")]
        internal static void Initialize()
        {
            if (ArgParser.IsDefined("melonloader.hideconsole"))
                Current.Console.Hide = true;

            if (ArgParser.IsDefined("melonloader.consoleontop"))
                Current.Console.AlwaysOnTop = true;

            if (ArgParser.IsDefined("melonloader.consoledst"))
                Current.Console.DontSetTitle = true;

            if (ArgParser.IsDefined("melonloader.hidewarnings"))
                Current.Console.HideWarnings = true;
        }
#endif

        [TomlProperty("hide_warnings")]
        [TomlPrecedingComment("Hides warnings from displaying. Equivalent to the '--melonloader.hidewarnings' launch option")]
        public bool HideWarnings { get; internal set; }

        [TomlProperty("hide_console")]
        [TomlPrecedingComment("Hides the console. Equivalent to the '--melonloader.hideconsole' launch option")]
        public bool Hide { get; internal set; }

        [TomlProperty("console_on_top")]
        [TomlPrecedingComment("Forces the console to always stay on-top of all other applications. Equivalent to the '--melonloader.consoleontop' launch option")]
        public bool AlwaysOnTop { get; internal set; }

        [TomlProperty("dont_set_title")]
        [TomlPrecedingComment("Keeps the console title as original. Equivalent to the '--melonloader.consoledst' launch option")]
        public bool DontSetTitle { get; internal set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class LogsConfig
    {
#if BOOTSTRAP
        [RequiresDynamicCode("Dynamically accesses LoaderConfig properties")]
        internal static void Initialize()
        {
            if (uint.TryParse(ArgParser.GetValue("melonloader.maxlogs"), out var maxLogs))
                Current.Logs.MaxLogs = maxLogs;
        }
#endif

        [TomlProperty("max_logs")]
        [TomlPrecedingComment("Sets the maximum amount of log files in the Logs folder (Default: 10). Equivalent to the '--melonloader.maxlogs' launch option")]
        public uint MaxLogs { get; internal set; } = 10;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class MonoDebugServerConfig
    {
#if BOOTSTRAP
        [RequiresDynamicCode("Dynamically accesses LoaderConfig properties")]
        internal static void Initialize()
        {
            if (ArgParser.IsDefined("melonloader.debugsuspend"))
                Current.MonoDebugServer.DebugSuspend = true;

            var debugIpAddress = ArgParser.GetValue("melonloader.debugipaddress");
            if (debugIpAddress != null)
                Current.MonoDebugServer.DebugIpAddress = debugIpAddress;

            if (uint.TryParse(ArgParser.GetValue("melonloader.debugport"), out var debugPort))
                Current.MonoDebugServer.DebugPort = debugPort;
        }
#endif

        [TomlProperty("debug_suspend")]
        [TomlPrecedingComment("Let the Mono debug server wait until a debugger is attached when debug_mode is true (only for Mono games). Equivalent to the '--melonloader.debugsuspend' launch option")]
        public bool DebugSuspend { get; internal set; }

        [TomlProperty("debug_ip_address")]
        [TomlPrecedingComment("The IP address the Mono debug server will listen to when debug_mode is true (only for Mono games). Equivalent to the '--melonloader.debugipaddress' launch option")]
        public string DebugIpAddress { get; internal set; } = "127.0.0.1";

        [TomlProperty("debug_port")]
        [TomlPrecedingComment("The port the Mono debug server will listen to when debug_mode is true (only for Mono games). Equivalent to the '--melonloader.debugport' launch option")]
        public uint DebugPort { get; internal set; } = 55555;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class UnityEngineConfig
    {
#if BOOTSTRAP
        [RequiresDynamicCode("Dynamically accesses LoaderConfig properties")]
        internal static void Initialize()
        {
            var unityVersionOverride = ArgParser.GetValue("melonloader.unityversion");
            if (unityVersionOverride != null)
                Current.UnityEngine.VersionOverride = unityVersionOverride;

            if (ArgParser.IsDefined("melonloader.disableunityclc"))
                Current.UnityEngine.DisableConsoleLogCleaner = true;

            var monoSearchPathOverride = ArgParser.GetValue("melonloader.monosearchpathoverride");
            if (monoSearchPathOverride != null)
                Current.UnityEngine.MonoSearchPathOverride = monoSearchPathOverride;

            if (ArgParser.IsDefined("melonloader.agfregenerate"))
                Current.UnityEngine.ForceRegeneration = true;

            if (ArgParser.IsDefined("melonloader.mbepatch"))
                Current.UnityEngine.MBEPatch = true;

            if (ArgParser.IsDefined("melonloader.agfoffline"))
                Current.UnityEngine.ForceOfflineGeneration = true;

            var forceRegex = ArgParser.GetValue("melonloader.agfregex");
            if (forceRegex != null)
                Current.UnityEngine.ForceGeneratorRegex = forceRegex;

            var forceDumperVersion = ArgParser.GetValue("melonloader.agfvdumper");
            if (forceDumperVersion != null)
                Current.UnityEngine.ForceIl2CppDumperVersion = forceDumperVersion;

            if (ArgParser.IsDefined("cpp2il.callanalyzer"))
                Current.UnityEngine.EnableCpp2ILCallAnalyzer = true;

            if (ArgParser.IsDefined("cpp2il.nativemethoddetector"))
                Current.UnityEngine.EnableCpp2ILNativeMethodDetector = true;
        }
#endif

        [TomlNonSerialized]
        private const string MonoPathSeparatorDescription =
#if WINDOWS
            "semicolon (;)";
#elif LINUX || OSX
            "colon (:)";
#endif
        
        [TomlProperty("version_override")]
        [TomlPrecedingComment("Overrides the detected UnityEngine version. Equivalent to the '--melonloader.unityversion' launch option")]
        public string VersionOverride { get; internal set; } = "";

        [TomlProperty("disable_console_log_cleaner")]
        [TomlPrecedingComment("Disables the console log cleaner (only applies to Il2Cpp games). Equivalent to the '--melonloader.disableunityclc' launch option")]
        public bool DisableConsoleLogCleaner { get; internal set; }

        [TomlProperty("mono_search_path_override")]
        [TomlPrecedingComment($"A {MonoPathSeparatorDescription} separated list of paths that Mono will prioritise to seek mscorlib and core libraries before the Managed folder and Melon's included set of core libraries. Equivalent to the '--melonloader.monosearchpathoverride' launch option")]
        public string MonoSearchPathOverride { get; internal set; } = "";

        [TomlProperty("mono_bleeding_edge_environment_patches")]
        [TomlPrecedingComment("Forces MonoBleedingEdge to utilize included Environment Patches. Equivalent to the '--melonloader.mbepatch' launch option")]
        public bool MBEPatch { get; internal set; }

        [TomlProperty("force_offline_generation")]
        [TomlPrecedingComment("Forces the Il2Cpp Assembly Generator to run without contacting the remote API. Equivalent to the '--melonloader.agfoffline' launch option")]
        public bool ForceOfflineGeneration { get; internal set; }

        [TomlProperty("force_generator_regex")]
        [TomlPrecedingComment("Forces the Il2Cpp Assembly Generator to use the specified regex. Equivalent to the '--melonloader.agfregex' launch option")]
        public string ForceGeneratorRegex { get; internal set; } = "";

        [TomlProperty("force_il2cpp_dumper_version")]
        [TomlPrecedingComment("Forces the Il2Cpp Assembly Generator to use the specified Il2Cpp dumper version. Equivalent to the '--melonloader.agfvdumper' launch option")]
        public string ForceIl2CppDumperVersion { get; internal set; } = "";

        [TomlProperty("force_regeneration")]
        [TomlPrecedingComment("Forces the Il2Cpp Assembly Generator to always regenerate assemblies. Equivalent to the '--melonloader.agfregenerate' launch option")]
        public bool ForceRegeneration { get; internal set; }

        [TomlProperty("enable_cpp2il_call_analyzer")]
        [TomlPrecedingComment("Enables the CallAnalyzer processor for Cpp2IL. Equivalent to the '--cpp2il.callanalyzer' launch option")]
        public bool EnableCpp2ILCallAnalyzer { get; internal set; }

        [TomlProperty("enable_cpp2il_native_method_detector")]
        [TomlPrecedingComment("Enables the NativeMethodDetector processor for Cpp2IL. Equivalent to the '--cpp2il.nativemethoddetector' launch option")]
        public bool EnableCpp2ILNativeMethodDetector { get; internal set; }
    }
}
