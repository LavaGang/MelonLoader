using MelonLoader.Bootstrap;
using System.Runtime.InteropServices;

namespace MelonLoader.NativeHost;

internal class MelonLoaderInvoker
{
    internal static unsafe void Initialize()
    {
        BootstrapInterop.HookAttach = Marshal.GetDelegateForFunctionPointer<NativeHookFn>(NativeEntryPoint.Functions.HookAttach);
        BootstrapInterop.HookDetach = Marshal.GetDelegateForFunctionPointer<NativeHookFn>(NativeEntryPoint.Functions.HookDetach);

        MelonLogger.HostLogMsg = Marshal.GetDelegateForFunctionPointer<LogMsgFn>(NativeEntryPoint.Functions.LogMsg);
        MelonLogger.HostLogError = Marshal.GetDelegateForFunctionPointer<LogErrorFn>(NativeEntryPoint.Functions.LogError);
        MelonLogger.HostLogMelonInfo = Marshal.GetDelegateForFunctionPointer<LogMelonInfoFn>(NativeEntryPoint.Functions.LogMelonInfo);

        Core.Initialize();
    }

    internal static void Start() => Core.Start();
}
