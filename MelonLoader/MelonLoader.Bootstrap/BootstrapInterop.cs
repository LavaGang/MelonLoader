using MelonLoader.Shared.Utils;
using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap
{
    public static unsafe class BootstrapInterop
    {
        public static delegate* unmanaged<void*, void*, void*> NativeHookAttach;
        public static delegate* unmanaged<void*, void> NativeHookDetach;

        public static T HookAttach<T>(T target, T detour) where T : Delegate
            => Marshal.GetDelegateForFunctionPointer<T>(HookAttach(target.GetFunctionPointer(), detour.GetFunctionPointer()));
        public static IntPtr HookAttach(IntPtr target, IntPtr detour)
        {
            // Dereference Target Pointer to get Actual Pointer
            IntPtr targetAddr = (IntPtr)(&target);

            // Attach Hook
            void* originalFunc = NativeHookAttach(targetAddr.ToPointer(), detour.ToPointer());

            // Return Original Func as IntPtr
            return new IntPtr(originalFunc);
        }

        public static void HookDetach<T>(T target) where T : Delegate
            => HookDetach(target.GetFunctionPointer());
        public static void HookDetach(IntPtr target)
        {
            // Dereference Target Pointer to get Actual Pointer
            IntPtr targetAddr = (IntPtr)(&target);

            // Detach Hook
            NativeHookDetach(targetAddr.ToPointer());
        }
    }
}