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
#if LINUX || OSX || ANDROID
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private delegate nint DlsymFn(nint handle, string symbol);
    private static readonly DlsymFn HookDlsymDelegate = HookDlsym;
#endif
#if WINDOWS
    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    private delegate nint GetProcAddressFn(nint handle, string symbol);
    private static readonly GetProcAddressFn HookGetProcAddressDelegate = HookGetProcAddress;
#endif

    public static nint LibraryHandle { get; private set; }

    internal static InternalLogger Logger { get; private set; } = new(ColorARGB.BlueViolet, "MelonLoader.Bootstrap");
    internal static InternalLogger PlayerLogger { get; private set; } = new(ColorARGB.Turquoise, "UNITY");
    public static string DataDir { get; private set; } = null!;
    public static string GameDir { get; private set; } = null!;

    private static bool _runtimeInitialised;

    [RequiresDynamicCode("Calls InitConfig")]
    public static void Init(nint moduleHandle)
    {
        LibraryHandle = moduleHandle;

        var exePath = Environment.ProcessPath!;
        GameDir = Path.GetDirectoryName(exePath)!;

#if ANDROID
        DataDir = Proxy.Android.AndroidBootstrap.GetDataDir();
        if (!Proxy.Android.AndroidBootstrap.EnsurePerms())
            throw new Exception("Permissions not granted! MelonLoader cannot continue without permissions.");

        MelonLoader.Utils.APKAssetManager.Initialize();
        Proxy.Android.AndroidProxy.Log("JNI initialized!");

        Proxy.Android.AndroidBootstrap.CopyMelonLoaderData(Proxy.Android.AndroidBootstrap.GetApkModificationDate());
        Proxy.Android.AndroidProxy.Log("APK assets copied!");
#elif OSX
        DataDir = Path.Combine(Path.GetDirectoryName(GameDir)!, "Resources", "Data");
#else
        DataDir = Path.Combine(GameDir, Path.GetFileNameWithoutExtension(exePath) + "_Data");
#endif
        if (!Directory.Exists(DataDir))
            return;

        LoaderConfig.Initialize();

        if (LoaderConfig.Current.Loader.Disable)
            return;

        MelonLogger.Init();
        if (!LoaderConfig.Current.Loader.CapturePlayerLogs)
            ConsoleHandler.NullHandles();

#if LINUX || OSX || ANDROID
        PltHook.InstallHooks
        ([
            ("dlsym", Marshal.GetFunctionPointerForDelegate(HookDlsymDelegate))
        ]);
#endif

#if WINDOWS
        PltHook.InstallHooks
        ([
            ("GetProcAddress", Marshal.GetFunctionPointerForDelegate(HookGetProcAddressDelegate))
        ]);
#endif
    }

    private static nint RedirectSymbol(nint handle, string symbolName, nint originalSymbolAddress)
    {
        if (!MonoHandler.SymbolRedirects.TryGetValue(symbolName, out var redirect)
            && !Il2CppHandler.SymbolRedirects.TryGetValue(symbolName, out redirect))
            return originalSymbolAddress;

        MelonDebug.Log($"Redirecting {symbolName}");
        if (!_runtimeInitialised)
        {
            redirect.InitMethod(handle);
            if (!LoaderConfig.Current.Loader.CapturePlayerLogs)
                ConsoleHandler.ResetHandles();
        }

        _runtimeInitialised = true;
        return redirect.detourPtr;
    }

#if LINUX || OSX || ANDROID
    private static nint HookDlsym(nint handle, string symbol)
    {
        nint originalSymbolAddress = LibcNative.Dlsym(handle, symbol);
        return RedirectSymbol(handle, symbol, originalSymbolAddress);
    }
#endif

#if WINDOWS
    private static nint HookGetProcAddress(nint handle, string symbol)
    {
        nint originalSymbolAddress = WindowsNative.GetProcAddress(handle, symbol);
        return RedirectSymbol(handle, symbol, originalSymbolAddress);
    }
#endif
}
