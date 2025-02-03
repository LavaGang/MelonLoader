using System.Diagnostics;
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
        [TomlPrecedingComment("Starts the dotnet debugger (only for Dotnet). Equivalent to the '--melonloader.launchdebugger' launch option")]
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
}