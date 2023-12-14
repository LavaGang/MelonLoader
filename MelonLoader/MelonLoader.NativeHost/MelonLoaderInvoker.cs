using MelonLoader;
using MelonLoader.Bootstrap;
using MelonLoader.CoreClr.Bootstrap.Fixes;

namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize(bool firstRun)
        {
            BootstrapInterop.HookAttach = HookAttach;
            BootstrapInterop.HookDetach = NativeEntryPoint.Exports.HookDetach;
            BootstrapInterop.WriteLogToFile = NativeEntryPoint.Exports.WriteLogToFile;
            if (firstRun)
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