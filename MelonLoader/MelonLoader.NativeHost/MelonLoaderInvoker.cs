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
            IntPtr detourPtr = (IntPtr)detour;
            //if (!CoreClrDelegateFixer.SanityCheckDetour(ref detourPtr))
            //    return (void*)0;

            void* trampoline = NativeEntryPoint.Exports.HookAttach(target, (void*)detourPtr);
            NativeStackWalk.RegisterHookAddr((ulong)detour, $"Requested detour of 0x{(IntPtr)target:X}");
            return trampoline;
        }
    }
}