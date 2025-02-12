using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Tomlet.Attributes;

namespace MelonLoader;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public class LoaderConfig
{
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
        [TomlNonSerialized]
        public string BaseDirectory { get; internal set; } = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;

        // Technically, this will always return false, but it's still a config ¯\_(ツ)_/¯
        [TomlProperty("disable")]
        [TomlPrecedingComment("Disables MelonLoader. Equivalent to the '--no-mods' launch option")]
        public bool Disable { get; internal set; }

        [TomlProperty("debug_mode")]
        [TomlPrecedingComment("Equivalent to the '--melonloader.debug' launch option")]
        public bool DebugMode { get; internal set; }
#if DEBUG
            = true;
#endif

        [TomlProperty("force_quit")]
        [TomlPrecedingComment("Only use this if the game freezes when trying to quit. Equivalent to the '--quitfix' launch option")]
        public bool ForceQuit { get; internal set; }

        [TomlProperty("disable_start_screen")]
        [TomlPrecedingComment("Disables the start screen. Equivalent to the '--melonloader.disablestartscreen' launch option")]
        public bool DisableStartScreen { get; internal set; }

        [TomlProperty("launch_debugger")]
        [TomlPrecedingComment("Starts the dotnet debugger (only for Il2Cpp games). Equivalent to the '--melonloader.launchdebugger' launch option")]
        public bool LaunchDebugger { get; internal set; }

        [TomlProperty("theme")]
        [TomlPrecedingComment("Sets the loader theme. Currently, the only available themes are \"Normal\" and \"Lemon\". Equivalent to the '--melonloader.consolemode' launch option (0 for Normal, 4 for Lemon)")]
        public LoaderTheme Theme { get; internal set; }

        public enum LoaderTheme
        {
            Normal,
            Magenta,
            Rainbow,
            RandomRainbow,
            Lemon
        };
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class ConsoleConfig
    {
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
        [TomlProperty("max_logs")]
        [TomlPrecedingComment("Sets the maximum amount of log files in the Logs folder (Default: 10). Equivalent to the '--melonloader.maxlogs' launch option")]
        public uint MaxLogs { get; internal set; } = 10;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class MonoDebugServerConfig
    {
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
        [TomlNonSerialized]
        private const string MonoPathSeparatorDescription =
#if WINDOWS
            "semicolon (;)";
#elif LINUX
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