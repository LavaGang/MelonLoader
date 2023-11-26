using MelonLoader.Bootstrap;

namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize()
        {
            BootstrapInterop.HookAttach = NativeEntryPoint.Exports.HookAttach;
            BootstrapInterop.HookDetach = NativeEntryPoint.Exports.HookDetach;

            Entrypoint.Entry();
        }
    }
}