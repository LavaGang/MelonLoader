using MelonLoader.Shared.Utils;
using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap
{
    public static unsafe class BootstrapInterop
    {
        public static delegate* unmanaged<void*, void*, void*> NativeHookAttach;
        public static delegate* unmanaged<void*, void> NativeHookDetach;

        public unsafe static T HookAttach<T>(T target, T detour) where T : Delegate
            => Marshal.GetDelegateForFunctionPointer<T>(
                new IntPtr(
                    NativeHookAttach(
                        target.GetFunctionPointer().ToPointer(),
                        detour.GetFunctionPointer().ToPointer())));

        public unsafe static void HookDetach<T>(T target) where T : Delegate
            => NativeHookDetach(target.GetFunctionPointer().ToPointer());
    }
}