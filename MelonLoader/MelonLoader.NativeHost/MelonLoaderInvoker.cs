using MelonLoader.Bootstrap;

namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize()
        {
            BootstrapInterop.NativeHookAttach = NativeEntryPoint.Exports.HookAttach;
            BootstrapInterop.NativeHookDetach = NativeEntryPoint.Exports.HookDetach;

            Entrypoint.Entry();
        }
    }
}