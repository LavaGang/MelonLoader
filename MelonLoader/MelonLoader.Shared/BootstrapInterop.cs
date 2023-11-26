using System;
using System.Runtime.CompilerServices;
#if NET6_0
using MelonLoader.Shared.CoreClrUtils;
using MelonLoader.Shared.Utils;
#endif
namespace MelonLoader.Bootstrap
{
    public static unsafe class BootstrapInterop
    {
#if NET6_0
        public static delegate* unmanaged<void*, void*, void*> HookAttach;

        public static IntPtr NativeHookAttach(IntPtr target, IntPtr detour)
        {
            // if (!CoreClrDelegateFixer.SanityCheckDetour(ref detour))
            //     return IntPtr.Zero;

            var trampoline = (IntPtr)HookAttach((void*)target, (void*)detour);
            NativeStackWalk.RegisterHookAddr((ulong)detour, $"Mod-requested detour of 0x{target:X}");
            
            MelonLogger.Msg(trampoline.ToString("X"));
            return trampoline;
        }
        public static delegate* unmanaged<void*, void> HookDetach;
        
        public static void NativeHookDetach(IntPtr target) => HookDetach((void*)target);
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr NativeHookAttach(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void NativeHookDetach(IntPtr target);
#endif
    }
}
