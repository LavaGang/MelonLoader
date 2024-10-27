using MelonLoader.Bootstrap.Logging;
using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal static class Il2CppHandler
{
    private static Dobby.Patch<Il2CppLib.InitFn>? initPatch;
    private static Dobby.Patch<Il2CppLib.RuntimeInvokeFn>? invokePatch;

    private static FunctionExchange functionExchange; // Prevent GC

    private static Il2CppLib il2cpp = null!;

    public static bool TryInitialize()
    {
        var il2cppLib = Il2CppLib.TryLoad();
        if (il2cppLib == null)
            return false;

        il2cpp = il2cppLib;

        MelonDebug.Log("Patching il2cpp init");
        initPatch = Dobby.CreatePatch<Il2CppLib.InitFn>(il2cpp.InitPtr, InitDetour);
        if (initPatch == null)
        {
            MelonDebug.Log("Failed to patch il2cpp init");
            return false;
        }

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
        var managedDir = Path.Combine(Core.BaseDir, "MelonLoader", "net6");
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
        var initialize = Dotnet.LoadAssemblyAndGetFunctionUCO<InitializeFn>(context, nativeHostPath, "MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost", "NativeEntry");
        if (initialize == null)
        {
            Core.Logger.Error($"Failed to load assembly from: '{nativeHostPath}'");
            return;
        }

        functionExchange = new FunctionExchange()
        {
            HookAttach = NativeHookAttachImpl,
            HookDetach = NativeHookDetachImpl,
            LogMsg = MelonLogger.LogFromManaged,
            LogError = MelonLogger.LogErrorFromManaged,
            LogMelonInfo = MelonLogger.LogMelonInfoFromManaged
        };

        MelonDebug.Log("Invoking NativeHost entry");
        initialize(ref functionExchange);

        if (functionExchange.Start == null)
        {
            Core.Logger.Error($"Managed did not return the initial function pointer");
            return;
        }

        MelonDebug.Log("Patching invoke");
        invokePatch = Dobby.CreatePatch<Il2CppLib.RuntimeInvokeFn>(il2cpp.RuntimeInvokePtr, InvokeDetour);
        if (invokePatch == null)
        {
            Core.Logger.Error($"Failed to patch il2cpp invoke");
            return;
        }
    }

    private static nint InvokeDetour(nint method, nint obj, nint args, nint exc)
    {
        if (invokePatch == null)
            return 0;

        var result = invokePatch.Original(method, obj, args, exc);

        var name = il2cpp.GetMethodName(method);
        if (name != null && name.Contains("Internal_ActiveSceneChanged"))
        {
            MelonDebug.Log("Invoke hijacked");
            invokePatch.Destroy();

            Start();
        }

        return result;
    }

    private static void Start()
    {
        functionExchange.Start?.Invoke();
    }

    private static unsafe void NativeHookAttachImpl(nint* target, nint detour)
    {
        *target = Dobby.HookAttach(*target, detour);
    }

    private static unsafe void NativeHookDetachImpl(nint* target, nint detour)
    {
        Dobby.HookDetach(*target);
    }

    private delegate void InitializeFn(ref FunctionExchange exchange);

    [StructLayout(LayoutKind.Sequential)]
    private struct FunctionExchange
    {
        internal required NativeHookFn HookAttach;
        internal required NativeHookFn HookDetach;
        internal required LogMsgFn LogMsg;
        internal required LogErrorFn LogError;
        internal required LogMelonInfoFn LogMelonInfo;

        public Action? Start;
    }
}
