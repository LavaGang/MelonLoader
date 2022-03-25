using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader
{
    internal unsafe static class BootstrapInterop
    {
#if NET6_0
        internal static delegate* unmanaged<void**, void*, void> HookAttach;
        internal static delegate* unmanaged<void**, void*, void> HookDetach;
#endif

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string Internal_GetHashCode();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[return: MarshalAs(UnmanagedType.LPStr)]
        internal /*extern */static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameName, [MarshalAs(UnmanagedType.LPStr)] string GameVersion = null)
        {
            MelonLogger.Warning($"TODO: SetDefaultConsoleTitleWithGameName({GameName}, {GameVersion})");
        }

#if !NET6_0
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void EnableCloseButton(IntPtr mainWindow);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void DisableCloseButton(IntPtr mainWindow);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsUnderWineOrSteamProton();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void NativeHookAttach(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void NativeHookDetach(IntPtr target, IntPtr detour);
#else
        public static void EnableCloseButton(IntPtr mainWindow) 
        {
            MelonLogger.Warning("TODO: EnableCloseButton");
        }

        public static void DisableCloseButton(IntPtr mainWindow)
        {
            MelonLogger.Warning("TODO: DisableCloseButton");
        }

        public static bool IsUnderWineOrSteamProton()
        {
            MelonLogger.Warning("TODO: IsUnderWineOrSteamProton");
            return false;
        }

        public static unsafe void NativeHookAttach(IntPtr target, IntPtr detour) 
        {
            HookAttach((void**) target, (void*) detour);
        }

        public static unsafe void NativeHookDetach(IntPtr target, IntPtr detour)
        {
            HookDetach((void**)target, (void*)detour);
        }
#endif
    }
}
