using MelonLoader.Shared.Utils;
using System;

namespace MelonLoader.Bootstrap
{
    public static unsafe class BootstrapInterop
    {
        #region Native

        public static delegate* unmanaged<void*, void*, void*> NativeHookAttach;
        public static delegate* unmanaged<void*, void> NativeHookDetach;

        #endregion

        #region Attach

        public static IntPtr HookAttach(IntPtr target, IntPtr detour)
            => new IntPtr(NativeHookAttach(target.ToPointer(), detour.ToPointer()));

        public static T HookAttach<T>(T target, T detour) where T : Delegate
            => HookAttach(target.GetFunctionPointer(), detour.GetFunctionPointer()).GetDelegate<T>();

        public static void HookAttach<T>(ref T target, T detour) where T : Delegate
            => target = HookAttach(target.GetFunctionPointer(), detour.GetFunctionPointer()).GetDelegate<T>();

        #endregion

        #region Detach

        public static void HookDetach(IntPtr target)
            => NativeHookDetach(target.ToPointer());

        public static void HookDetach<T>(T target) where T : Delegate
            => HookDetach(target.GetFunctionPointer());

        #endregion
    }
}