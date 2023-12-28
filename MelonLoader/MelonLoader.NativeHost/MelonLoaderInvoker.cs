using MelonLoader.Bootstrap;

namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize(bool firstRun)
        {
            BootstrapInterop.HookAttach = NativeEntryPoint.Exports.HookAttach;
            BootstrapInterop.HookDetach = NativeEntryPoint.Exports.HookDetach;
            BootstrapInterop.WriteLogToFile = NativeEntryPoint.Exports.WriteLogToFile;
            if (firstRun)
                Entrypoint.Entry();
        }
    }
}