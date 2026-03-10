using System.Diagnostics;
using MelonLoader.Logging;
using MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;
using MelonLoader.Bootstrap.RuntimeHandlers.Mono;
using MelonLoader.Bootstrap.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using MelonLoader.Bootstrap.Logging;
using Tomlet;

namespace MelonLoader.Bootstrap;

public static class Core
{
    public static nint LibraryHandle { get; private set; }

    internal static InternalLogger Logger { get; private set; } = new(ColorARGB.BlueViolet, "MelonLoader.Bootstrap");
    internal static InternalLogger PlayerLogger { get; private set; } = new(ColorARGB.Turquoise, "UNITY");
    public static string DataDir { get; private set; } = null!;
    public static string GameDir { get; private set; } = null!;

    [RequiresDynamicCode("Calls InitConfig")]
    public static void Init(nint moduleHandle)
    {
        LibraryHandle = moduleHandle;

        var exePath = Environment.ProcessPath!;
        GameDir = Path.GetDirectoryName(exePath)!;

#if !OSX
        DataDir = Path.Combine(GameDir, Path.GetFileNameWithoutExtension(exePath) + "_Data");
#else
        DataDir = Path.Combine(Path.GetDirectoryName(GameDir)!, "Resources", "Data");
#endif
        if (!Directory.Exists(DataDir))
            return;

        LoaderConfig.Initialize();

        if (LoaderConfig.Current.Loader.Disable)
            return;

        SetupConsole();
        MelonLogger.Init();
        SetupPlayerLogCapture();
        ModuleSymbolRedirect.Attach();
    }

    private static void SetupConsole()
    {
        if (LoaderConfig.Current.Console.Hide)
            return;

        var version = typeof(Core).Assembly.GetName().Version!;
        var versionStr = version.ToString(3);
        if (version.Revision != 0)
            versionStr += "-ci." + version.Revision.ToString();

        string? title = null;
        if (!LoaderConfig.Current.Console.DontSetTitle)
        {
            // This is temporary, until managed sets it
            title = (LoaderConfig.Current.Loader.Theme == LoaderConfig.CoreConfig.LoaderTheme.Lemon ? "LemonLoader" : "MelonLoader") + " v" + versionStr;
            if (LoaderConfig.Current.Loader.DebugMode)
                title = "[D] " + title;
        }

        ConsoleHandler.OpenConsole(LoaderConfig.Current.Console.AlwaysOnTop, title);
    }

    private static void SetupPlayerLogCapture()
    {
        if (!LoaderConfig.Current.Loader.CapturePlayerLogs)
        {
            ConsoleHandler.NullHandles();
            return;
        }

#if LINUX || OSX
            UnixPlayerLogsMirroring.SetupPlayerLogMirroring();
#endif
#if WINDOWS
            WindowsPlayerLogsMirroring.SetupPlayerLogMirroring();
#endif
    }
}
