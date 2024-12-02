using MelonLoader.Bootstrap.Logging;
using MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;
using MelonLoader.Bootstrap.RuntimeHandlers.Mono;
using MelonLoader.Bootstrap.Utils;
using System.Drawing;

namespace MelonLoader.Bootstrap;

public static class Core
{
    public static nint LibraryHandle { get; private set; }

    internal static InternalLogger Logger { get; private set; } = new(Color.BlueViolet, "MelonBootstrap");
    public static string DataDir { get; private set; } = null!;
    public static string GameDir { get; private set; } = null!;
    public static string BaseDir { get; private set; } = ".";
    public static bool Debug { get; private set; }
#if DEBUG
        = true;
#else
        = false;
#endif

#if LINUX
    [System.Runtime.InteropServices.UnmanagedCallersOnly(EntryPoint = "Init")]
#endif
    public static void Init(nint moduleHandle)
    {
        LibraryHandle = moduleHandle;

        var exePath = Environment.ProcessPath!;
        GameDir = Path.GetDirectoryName(exePath)!;

        DataDir = Path.Combine(GameDir, Path.GetFileNameWithoutExtension(exePath) + "_Data");
        if (!Directory.Exists(DataDir))
            return;

        if (ArgParser.IsDefined("no-mods"))
            return;

#if !DEBUG
        if (ArgParser.IsDefined("melonloader.debug"))
            Debug = true;
#endif

        var customBaseDir = ArgParser.GetValue("melonloader.basedir");
        if (customBaseDir != null)
            BaseDir = customBaseDir;

        MelonLogger.Init();

        if (customBaseDir != null)
            Logger.Msg($"Starting from a custom base directory: '{BaseDir}'");

        MelonDebug.Log("Starting probe for runtime");

        if (Il2CppHandler.TryInitialize()
            || MonoHandler.TryInitialize())
        {
            ConsoleHandler.NullHandles();
            return;
        }

        Logger.Error("Current game runtime is not supported. The game might have a modified runtime or is not a real Unity game.");
    }
}
