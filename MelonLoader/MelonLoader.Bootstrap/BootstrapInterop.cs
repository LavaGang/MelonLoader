using System;

namespace MelonLoader.Bootstrap
{
    public static unsafe class BootstrapInterop
    {
        public static delegate* unmanaged<void*, void*, void*> NativeHookAttach;
        public static delegate* unmanaged<void*, void> NativeHookDetach;

        public static IntPtr HookAttach(IntPtr target, IntPtr detour)
            => new IntPtr(NativeHookAttach(target.ToPointer(), detour.ToPointer()));
        public static void HookDetach(IntPtr target)
            => NativeHookDetach(target.ToPointer());
    }
}