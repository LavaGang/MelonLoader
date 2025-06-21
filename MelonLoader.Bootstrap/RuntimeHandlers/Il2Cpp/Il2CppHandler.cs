using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal static class Il2CppHandler
{
    private static Action? startFunc; // Prevent GC

    private static Il2CppLib il2cpp = null!;
#if ANDROID
    private static ClrMonoLib mono = null!;
#endif
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

#if ANDROID
        var monoLib = ClrMonoLib.TryLoad();
        if (monoLib is null)
        {
            Core.Logger.Error("Could not load CoreCLR Mono");
            return;
        }
        mono = monoLib;
#endif
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

#if ANDROID
        // Workaround so that we can use .NET 8 on Android without upgrading MelonLoader's .NET version
        string runtimeConfig = File.ReadAllText(runtimeConfigPath);
        if (runtimeConfig.Contains("net6"))
        {
            runtimeConfig = runtimeConfig.Replace("6.0", "8.0");
            File.WriteAllText(runtimeConfigPath, runtimeConfig);
        }
#endif

        MelonDebug.Log("Attempting to load hostfxr");
        if (!Dotnet.LoadHostfxr())
        {
            DotnetInstaller.AttemptInstall();
            if (!Dotnet.LoadHostfxr())
            {
                Core.Logger.Error("Failed to load Hostfxr");
                return;
            }
        }

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

#if ANDROID
        ApplyMonoPatches();
#endif

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

#if ANDROID
        mono.ThreadSuspendReload();
        MelonDebug.Log("Mono thread reset");
#endif

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

#if ANDROID
    private static void ApplyMonoPatches()
    {
        MelonDebug.Log("Applying Mono runtime patches");

        mono.SetLevelString("warning");
        mono.SetMaskString("all");

        MelonDebug.Log("Enabled Mono logging");

        mono.InstallUnhandledExceptionHook(MonoUnhandledExceptionHandler, IntPtr.Zero);

        MelonDebug.Log("Installed unhandled exception hook");

        mono.SetThreadChecker(MonoCheckThread);

        MelonDebug.Log("Installed thread checker");
    }

    private static void MonoUnhandledExceptionHandler(IntPtr exc, IntPtr userData)
    {
        if (exc == IntPtr.Zero)
            return;

        mono.PrintUnhandledException(exc);
    }

    private static bool MonoCheckThread(ulong threadId)
    {
        UIntPtr size = 0;

        IntPtr threads = il2cpp.GetAllAttachedThreads(ref size);
        IntPtr[] threadsSlice = new IntPtr[(int)size];
        Marshal.Copy(threads, threadsSlice, 0, threadsSlice.Length);

        for (int i = 0; i < threadsSlice.Length; i++)
        {
            Il2CppThread thread = Marshal.PtrToStructure<Il2CppThread>(threadsSlice[i]);
            IntPtr internalThreadPtr = thread.internal_thread;

            if (internalThreadPtr != IntPtr.Zero)
            {
                Il2CppInternalThread internalThread = Marshal.PtrToStructure<Il2CppInternalThread>(internalThreadPtr);

                if (internalThread.tid == threadId)
                {
                    MelonDebug.Log($"Attached IL2CPP thread {internalThread.tid:X}");
                    return false;
                }
            }
        }

        return true;
    }
#endif

    // Requires the bootstrap handle to be passed first
    private delegate void InitializeFn(ref nint startFunc);
}
