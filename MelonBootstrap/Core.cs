using MelonBootstrap.RuntimeHandlers.Il2Cpp;
using MelonBootstrap.RuntimeHandlers.Mono;
using MelonBootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonBootstrap;

public static class Core
{
    public static string DataDir { get; private set; } = null!;
    public static string GameDir { get; private set; } = null!;
    public static bool Debug { get; private set; }
#if DEBUG
        = true;
#else
        = false;
#endif

    [UnmanagedCallersOnly(EntryPoint = "DllMain")]
    public static bool DllMain(nint hModule, uint ul_reason_for_call, nint lpReserved)
    {
        if (ul_reason_for_call == 1)
            Init();

        return true;
    }

    private static unsafe void Init()
    {
        var exePath = Environment.ProcessPath!;
        GameDir = Path.GetDirectoryName(exePath)!;

        DataDir = Path.Combine(GameDir, Path.GetFileNameWithoutExtension(exePath) + "_Data");
        if (!Directory.Exists(DataDir))
            return;

        var args = Environment.GetCommandLineArgs();

        if (args.Contains("--no-mods", StringComparer.OrdinalIgnoreCase))
            return;

#if !DEBUG
        if (args.Contains("--melonloader.debug", StringComparer.OrdinalIgnoreCase))
            Debug = true;
#endif

        var baseDir = ".";
        const string baseDirParam = "--melonloader.basedir";
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            string path;
            if (arg.Equals(baseDirParam, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                path = args[i + 1];
            }
            else if (arg.StartsWith(baseDirParam + "=", StringComparison.OrdinalIgnoreCase))
            {
                path = arg[(baseDirParam.Length + 1)..];
            }
            else
            {
                continue;
            }

            if (!Directory.Exists(path))
                break;

            baseDir = path;
            break;
        }

        var version = typeof(Core).Assembly.GetName().Version!;
        var versionStr = version.ToString(3);
        if (version.Revision != 0)
            versionStr += "-ci." + version.Revision.ToString();

        var onTop = args.Contains("--melonloader.consoleontop", StringComparer.OrdinalIgnoreCase);

        ConsoleUtils.OpenConsole(Debug, versionStr, onTop);

        //var dobbyPath = Path.Combine(baseDir, "dobby.dll");
        //if (!File.Exists(dobbyPath))
        //{
        //    dobbyPath = Path.Combine(baseDir, "MelonLoader", "Dependencies", "dobby.dll");
        //    if (!File.Exists(dobbyPath))
        //    {
        //        Console.WriteLine("Failed to find dobby");
        //        return;
        //    }
        //}

        //if (!NativeLibrary.TryLoad(dobbyPath, out _))
        //{
        //    Console.WriteLine("Failed to load dobby");
        //    return;
        //}

        //Console.WriteLine("Loaded dobby.");

        if (Il2CppHandler.TryInitialize())
            return;

        if (MonoHandler.TryInitialize())
            return;

        // TODO: Error, no handler for runtime
    }
}
