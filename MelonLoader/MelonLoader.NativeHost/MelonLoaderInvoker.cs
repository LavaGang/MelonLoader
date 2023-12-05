using MelonLoader.Bootstrap;
using MelonLoader.CoreClr.Bootstrap.Fixes;

namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize()
        {
            BootstrapInterop.HookAttach = HookAttach;
            BootstrapInterop.HookDetach = NativeEntryPoint.Exports.HookDetach;

            Entrypoint.Entry();
        }

        private static unsafe void* HookAttach(void* target, void* detour)
        {
            // if (!CoreClrDelegateFixer.SanityCheckDetour(ref detour))
            //     return IntPtr.Zero;

            void* trampoline = NativeEntryPoint.Exports.HookAttach(target, detour);
            NativeStackWalk.RegisterHookAddr((ulong)detour, $"Requested detour of 0x{(IntPtr)target:X}");
            return trampoline;
        }
    }
}