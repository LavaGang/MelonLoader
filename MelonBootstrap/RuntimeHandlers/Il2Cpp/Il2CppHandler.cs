using MelonBootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonBootstrap.RuntimeHandlers.Il2Cpp;

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

        initPatch = Dobby.CreatePatch<Il2CppLib.InitFn>(il2cpp.InitPtr, InitDetour);
        if (initPatch == null)
            return false;

        return true;
    }

    private static nint InitDetour(nint a)
    {
        if (initPatch == null)
            return 0;

        initPatch.Destroy();

        ConsoleUtils.ResetHandles();

        var domain = initPatch.Original(a);

        InitializeManaged();

        return domain;
    }

    private static void InitializeManaged()
    {
        var managedDir = Path.Combine(Core.BaseDir, "MelonLoader", "net6");
        var runtimeConfigPath = Path.Combine(managedDir, "MelonLoader.runtimeconfig.json");
        var nativeHostPath = Path.Combine(managedDir, "MelonLoader.NativeHost.dll");

        if (!File.Exists(runtimeConfigPath))
        {
            Console.WriteLine("a");
            return;
        }

        if (!File.Exists(nativeHostPath))
        {
            Console.WriteLine("b");
        }

        if (!Dotnet.LoadHostfxr())
        {
            Console.WriteLine("c");
            return;
        }

        if (!Dotnet.InitializeForRuntimeConfig(runtimeConfigPath, out var context))
        {
            Console.WriteLine("d");
            return;
        }

        var initialize = Dotnet.LoadAssemblyAndGetFunctionUCO<InitializeFn>(context, nativeHostPath, "MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost", "NativeEntry");
        if (initialize == null)
        {
            Console.WriteLine("e");
            return;
        }

        functionExchange = new FunctionExchange()
        {
            HookAttach = NativeHookAttachImpl,
            HookDetach = NativeHookDetachImpl
        };

        initialize(ref functionExchange);

        if (functionExchange.Start == null)
        {
            Console.WriteLine("f");
            return;
        }

        invokePatch = Dobby.CreatePatch<Il2CppLib.RuntimeInvokeFn>(il2cpp.RuntimeInvokePtr, InvokeDetour);
        if (invokePatch == null)
        {
            Console.WriteLine("g");
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
            invokePatch.Destroy();

            Start();
        }

        return result;
    }

    private static void Start()
    {
        functionExchange.Start?.Invoke();
    }

    private static void NativeHookAttachImpl(ref nint target, nint detour)
    {
        target = Dobby.HookAttach(target, detour);
    }

    private static void NativeHookDetachImpl(ref nint target, nint detour)
    {
        Dobby.HookDetach(target);
    }

    private delegate void InitializeFn(ref FunctionExchange exchange);
    private delegate void NativeHook(ref nint target, nint detour);

    [StructLayout(LayoutKind.Sequential)]
    private struct FunctionExchange
    {
        internal required NativeHook HookAttach;
        internal required NativeHook HookDetach;

        public Action? Start;
    }
}
