using MelonLoader.Bootstrap;

namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize()
        {
#if NET6_0
            BootstrapInterop.HookAttach = NativeEntryPoint.Exports.HookAttach;
            BootstrapInterop.HookDetach = NativeEntryPoint.Exports.HookDetach;
#endif

            Entrypoint.Entry();
        }
    }
}