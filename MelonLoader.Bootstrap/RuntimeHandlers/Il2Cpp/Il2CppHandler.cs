using MelonLoader.Bootstrap.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal static class Il2CppHandler
{
    private static Action? startFunc; // Prevent GC

    private static Il2CppLib il2cpp = null!;
    private static bool il2cppInitDone;
    private static bool invokeStarted;

    private static readonly Il2CppLib.InitFn Il2CPPInitDetourFn = InitDetour;
    private static readonly Il2CppLib.RuntimeInvokeFn InvokeDetourFn = InvokeDetour;
    internal static readonly Dictionary<string, (Action<nint> InitMethod, IntPtr detourPtr)> SymbolRedirects = new()
    {
        { "il2cpp_init", (Initialize, Marshal.GetFunctionPointerForDelegate(Il2CPPInitDetourFn))},
        { "il2cpp_runtime_invoke", (Initialize, Marshal.GetFunctionPointerForDelegate(InvokeDetourFn))},
    };

    public static void Initialize(nint handle)
    {
        var il2cppLib = Il2CppLib.TryLoad(handle);
        if (il2cppLib is null)
        {
            Core.Logger.Error("Could not load il2cpp");
            return;
        }

        il2cpp = il2cppLib;
    }

    internal static nint InitDetour(nint a)
    {
        if (il2cppInitDone)
            return il2cpp.Init(a);

        ConsoleHandler.ResetHandles();
        MelonDebug.Log("In init detour");

        var domain = il2cpp.Init(a);

        InitializeManaged();
        il2cppInitDone = true;

        return domain;
    }

    private static void InitializeManaged()
    {
        var managedDir = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "net6");
        var runtimeConfigPath = Path.Combine(managedDir, "MelonLoader.runtimeconfig.json");
        var nativeHostPath = Path.Combine(managedDir, "MelonLoader.NativeHost.dll");

        if (!File.Exists(runtimeConfigPath))
        {
            Core.Logger.Error($"Runtime config not found at: '{runtimeConfigPath}'");
            return;
        }

        if (!File.Exists(nativeHostPath))
        {
            Core.Logger.Error($"NativeHost not found at: '{runtimeConfigPath}'");
            return;
        }

        // 1) First try to use a portable .NET runtime in the game root
        MelonDebug.Log("Checking for portable .NET runtime in game root");
        if (TryConfigurePortableDotnet())
        {
            MelonDebug.Log("Attempting to load hostfxr using portable .NET runtime");
            if (Dotnet.LoadHostfxr())
                goto HostfxrLoaded;
        }

        // 2) If no portable runtime is found or it fails, use the normal system detection/installation
        MelonDebug.Log("Attempting to load hostfxr from system");
        if (!Dotnet.LoadHostfxr())
        {
            DotnetInstaller.AttemptInstall();
            if (!Dotnet.LoadHostfxr())
            {
                Core.Logger.Error("Failed to load Hostfxr");
                return;
            }
        }

    HostfxrLoaded:

        MelonDebug.Log("Initializing domain");
        if (!Dotnet.InitializeForRuntimeConfig(runtimeConfigPath, out var context))
        {
            DotnetInstaller.AttemptInstall();
            if (!Dotnet.InitializeForRuntimeConfig(runtimeConfigPath, out context))
            {
                Core.Logger.Error($"Failed to initialize a .NET domain");
                return;
            }
        }

        MelonDebug.Log("Loading NativeHost assembly");
        var initialize = Dotnet.LoadAssemblyAndGetFunctionUco<InitializeFn>(context, nativeHostPath, "MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost", "NativeEntry");
        if (initialize == null)
        {
            Core.Logger.Error($"Failed to load assembly from: '{nativeHostPath}'");
            return;
        }

        var startFuncPtr = Core.LibraryHandle;

        MelonDebug.Log("Invoking NativeHost entry");
        initialize(ref startFuncPtr);

        if (startFuncPtr == 0 || startFuncPtr == Core.LibraryHandle)
        {
            Core.Logger.Error($"Managed did not return the initial function pointer");
            return;
        }

        startFunc = Marshal.GetDelegateForFunctionPointer<Action>(startFuncPtr);
    }

    private static bool TryConfigurePortableDotnet()
    {
#if WINDOWS
        try
        {
            var process = Process.GetCurrentProcess();
            var exePath = process?.MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
                return false;

            var gameRoot = Path.GetDirectoryName(exePath);
            if (string.IsNullOrEmpty(gameRoot) || !Directory.Exists(gameRoot))
                return false;

            var candidateDirs = Directory.GetDirectories(gameRoot, "*", SearchOption.TopDirectoryOnly);
            var portableDir = candidateDirs
                .FirstOrDefault(d =>
                {
                    var name = Path.GetFileName(d);
                    return name != null && name.IndexOf("dotnet", StringComparison.OrdinalIgnoreCase) >= 0;
                });

            if (portableDir == null)
                return false;

            var hostfxrPath = Directory.GetFiles(portableDir, "hostfxr.dll", SearchOption.AllDirectories).FirstOrDefault();
            if (hostfxrPath == null)
                return false;

            Environment.SetEnvironmentVariable("DOTNET_ROOT", portableDir);

            var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            var pathEntries = path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (!pathEntries.Any(p => string.Equals(p, portableDir, StringComparison.OrdinalIgnoreCase)))
                Environment.SetEnvironmentVariable("PATH", portableDir + Path.PathSeparator + path);

            Core.Logger.Msg($"Using portable .NET runtime from game root: '{portableDir}'");

            return true;
        }
        catch
        {
            return false;
        }
#else
        return false;
#endif
    }

    internal static nint InvokeDetour(nint method, nint obj, nint args, nint exc)
    {
        if (invokeStarted)
            return il2cpp.RuntimeInvoke(method, obj, args, exc);

        var result = il2cpp.RuntimeInvoke(method, obj, args, exc);

        var name = il2cpp.GetMethodName(method);
        if (name == null || !name.Contains("Internal_ActiveSceneChanged"))
            return result;

        invokeStarted = true;
        MelonDebug.Log("Invoke hijacked");

        Start();

        return result;
    }

    private static void Start()
    {
        startFunc?.Invoke();
    }

    private static unsafe void NativeHookAttachImpl(nint* target, nint detour)
    {
        *target = Dobby.HookAttach(*target, detour);
    }

    private static unsafe void NativeHookDetachImpl(nint* target, nint detour)
    {
        Dobby.HookDetach(*target);
    }

    // Requires the bootstrap handle to be passed first
    private delegate void InitializeFn(ref nint startFunc);
}
