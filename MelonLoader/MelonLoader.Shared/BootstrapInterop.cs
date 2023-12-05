using System;

#if NET6_0
using System.Runtime.InteropServices;
using MelonLoader.Utils;
#else
using System.Runtime.CompilerServices;
#endif

namespace MelonLoader.Bootstrap
{
    public static unsafe class BootstrapInterop
    {
#if NET6_0

        public static IntPtr NativeLoadLib(string name)
            => NativeLibrary.Load(name);
        public static IntPtr NativeGetExport(IntPtr handle, string name)
            => NativeLibrary.GetExport(handle, name);

        public delegate void* dHookAttach(void* target, void* detour);
        public static dHookAttach HookAttach;
        public static IntPtr NativeHookAttach(IntPtr target, IntPtr detour)
        {
            var trampoline = new IntPtr(HookAttach(target.ToPointer(), detour.ToPointer()));
            MelonLogger.Msg(trampoline.ToString("X"));
            return trampoline;
        }

        public static delegate* unmanaged<void*, void> HookDetach;
        public static void NativeHookDetach(IntPtr target)
            => HookDetach(target.ToPointer());

#else

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr NativeLoadLib(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr NativeGetExport(IntPtr handle, string lpProcName);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr NativeHookAttach(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void NativeHookDetach(IntPtr target);

#endif
    }
}
