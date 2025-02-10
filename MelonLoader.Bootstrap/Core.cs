using MelonLoader.Bootstrap.Logging;
using MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;
using MelonLoader.Bootstrap.RuntimeHandlers.Mono;
using MelonLoader.Bootstrap.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Tomlet;

namespace MelonLoader.Bootstrap;

public static class Core
{
    public static nint LibraryHandle { get; private set; }

    internal static InternalLogger Logger { get; private set; } = new(Color.BlueViolet, "MelonLoader.Bootstrap");
    public static string DataDir { get; private set; } = null!;
    public static string GameDir { get; private set; } = null!;

#if LINUX
    [System.Runtime.InteropServices.UnmanagedCallersOnly(EntryPoint = "Init")]
#endif
    [RequiresDynamicCode("Calls InitConfig")]
    public static void Init(nint moduleHandle)
    {
        LibraryHandle = moduleHandle;

        var exePath = Environment.ProcessPath!;
        GameDir = Path.GetDirectoryName(exePath)!;

        DataDir = Path.Combine(GameDir, Path.GetFileNameWithoutExtension(exePath) + "_Data");
        if (!Directory.Exists(DataDir))
            return;

        InitConfig();

        if (LoaderConfig.Current.Loader.Disable)
            return;

        MelonLogger.Init();

        MelonDebug.Log("Starting probe for runtime");

        if (Il2CppHandler.TryInitialize()
            || MonoHandler.TryInitialize())
        {
            ConsoleHandler.NullHandles();
            return;
        }

        Logger.Error("Current game runtime is not supported. The game might have a modified runtime or is not a real Unity game.");
    }

    [RequiresDynamicCode("Dynamically accesses LoaderConfig properties")]
    private static void InitConfig()
    {
        var customBaseDir = ArgParser.GetValue("melonloader.basedir");
        var baseDir = Directory.Exists(customBaseDir) ? Path.GetFullPath(customBaseDir) : LoaderConfig.Current.Loader.BaseDirectory;

        var path = Path.Combine(baseDir, "UserData", "Loader.cfg");

        if (File.Exists(path))
        {
            try
            {
                var doc = TomlParser.ParseFile(path);

                LoaderConfig.Current = TomletMain.To<LoaderConfig>(doc) ?? new();
            }
            catch { }
        }

        var doc2 = TomletMain.TomlStringFrom(LoaderConfig.Current);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, doc2);
        }
        catch { }

        // Override configs defined by launch options, without overriding the file

        LoaderConfig.Current.Loader.BaseDirectory = baseDir;

#if DEBUG
        LoaderConfig.Current.Loader.DebugMode = true;
#else
        if (ArgParser.IsDefined("melonloader.debug"))
            LoaderConfig.Current.Loader.DebugMode = true;
#endif

        if (ArgParser.IsDefined("no-mods"))
            LoaderConfig.Current.Loader.Disable = true;

        if (ArgParser.IsDefined("quitfix"))
            LoaderConfig.Current.Loader.ForceQuit = true;

        if (ArgParser.IsDefined("melonloader.disablestartscreen"))
            LoaderConfig.Current.Loader.DisableStartScreen = true;

        if (ArgParser.IsDefined("melonloader.launchdebugger"))
            LoaderConfig.Current.Loader.LaunchDebugger = true;

        if (int.TryParse(ArgParser.GetValue("melonloader.consolemode"), out var valueint))
            LoaderConfig.Current.Loader.Theme = (LoaderConfig.CoreConfig.LoaderTheme)Math.Clamp(valueint, (int)LoaderConfig.CoreConfig.LoaderTheme.Normal, (int)LoaderConfig.CoreConfig.LoaderTheme.Lemon);

        if (ArgParser.IsDefined("melonloader.hideconsole"))
            LoaderConfig.Current.Console.Hide = true;

        if (ArgParser.IsDefined("melonloader.consoleontop"))
            LoaderConfig.Current.Console.AlwaysOnTop = true;

        if (ArgParser.IsDefined("melonloader.consoledst"))
            LoaderConfig.Current.Console.DontSetTitle = true;

        if (ArgParser.IsDefined("melonloader.hidewarnings"))
            LoaderConfig.Current.Console.HideWarnings = true;

        if (uint.TryParse(ArgParser.GetValue("melonloader.maxlogs"), out var maxLogs))
            LoaderConfig.Current.Logs.MaxLogs = maxLogs;

        if (ArgParser.IsDefined("melonloader.debugsuspend"))
            LoaderConfig.Current.MonoDebugServer.DebugSuspend = true;

        var debugIpAddress = ArgParser.GetValue("melonloader.debugipaddress");
        if (debugIpAddress != null)
            LoaderConfig.Current.MonoDebugServer.DebugIpAddress = debugIpAddress;

        if (uint.TryParse(ArgParser.GetValue("melonloader.debugport"), out var debugPort))
            LoaderConfig.Current.MonoDebugServer.DebugPort = debugPort;
        
        var unityVersionOverride = ArgParser.GetValue("melonloader.unityversion");
        if (unityVersionOverride != null)
            LoaderConfig.Current.UnityEngine.VersionOverride = unityVersionOverride;

        if (ArgParser.IsDefined("melonloader.disableunityclc"))
            LoaderConfig.Current.UnityEngine.DisableConsoleLogCleaner = true;

        if (ArgParser.IsDefined("melonloader.agfregenerate"))
            LoaderConfig.Current.UnityEngine.ForceRegeneration = true;

        if (ArgParser.IsDefined("melonloader.agfoffline"))
            LoaderConfig.Current.UnityEngine.ForceOfflineGeneration = true;

        var forceRegex = ArgParser.GetValue("melonloader.agfregex");
        if (forceRegex != null)
            LoaderConfig.Current.UnityEngine.ForceGeneratorRegex = forceRegex;

        var forceDumperVersion = ArgParser.GetValue("melonloader.agfvdumper");
        if (forceDumperVersion != null)
            LoaderConfig.Current.UnityEngine.ForceIl2CppDumperVersion = forceDumperVersion;

        if (ArgParser.IsDefined("cpp2il.callanalyzer"))
            LoaderConfig.Current.UnityEngine.EnableCpp2ILCallAnalyzer = true;

        if (ArgParser.IsDefined("cpp2il.nativemethoddetector"))
            LoaderConfig.Current.UnityEngine.EnableCpp2ILNativeMethodDetector = true;
    }
}
