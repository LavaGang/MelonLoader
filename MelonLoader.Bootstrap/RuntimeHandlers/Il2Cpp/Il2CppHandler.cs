using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal static class Il2CppHandler
{
    private static Dobby.Patch<Il2CppLib.InitFn>? initPatch;
    private static Dobby.Patch<Il2CppLib.RuntimeInvokeFn>? invokePatch;

    private static Action? startFunc; // Prevent GC

    private static Il2CppLib il2cpp = null!;

    public static bool TryInitialize()
    {
        var il2cppLib = Il2CppLib.TryLoad();
        if (il2cppLib == null)
            return false;

        il2cpp = il2cppLib;

        MelonDebug.Log("Patching il2cpp init");
        initPatch = Dobby.CreatePatch<Il2CppLib.InitFn>(il2cpp.InitPtr, InitDetour);

        return true;
    }

    private static nint InitDetour(nint a)
    {
        if (initPatch == null)
            return 0;

        initPatch.Destroy();

        ConsoleHandler.ResetHandles();
        MelonDebug.Log("In init detour");

        var domain = initPatch.Original(a);

        InitializeManaged();

        return domain;
    }

    private static unsafe void InitializeManaged()
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

        var startFuncPtr = Core.LibraryHandle;

        MelonDebug.Log("Invoking NativeHost entry");
        initialize(ref startFuncPtr);

        if (startFuncPtr == 0 || startFuncPtr == Core.LibraryHandle)
        {
            Core.Logger.Error($"Managed did not return the initial function pointer");
            return;
        }

        startFunc = Marshal.GetDelegateForFunctionPointer<Action>(startFuncPtr);

        MelonDebug.Log("Patching invoke");
        invokePatch = Dobby.CreatePatch<Il2CppLib.RuntimeInvokeFn>(il2cpp.RuntimeInvokePtr, InvokeDetour);
    }

    private static nint InvokeDetour(nint method, nint obj, nint args, nint exc)
    {
        if (invokePatch == null)
            return 0;

        var result = invokePatch.Original(method, obj, args, exc);

        var name = il2cpp.GetMethodName(method);
        if (name == null || !name.Contains("Internal_ActiveSceneChanged"))
            return result;

        MelonDebug.Log("Invoke hijacked");
        invokePatch.Destroy();

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
