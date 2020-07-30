using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public static class Imports
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void UNLOAD_MELONLOADER();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetCompanyName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetProductName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetGameDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetGameDataDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetAssemblyDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetMonoConfigDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetExePath();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsIl2CppGame();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsDebugMode();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsConsoleEnabled();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void Hook(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void Unhook(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static bool IsOldMono();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static bool IsQuitFix();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static bool AG_Force_Regenerate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string AG_Force_Version_Unhollower();

        internal enum LoadMode
        {
            NORMAL,
            DEV,
            BOTH
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static LoadMode GetLoadMode_Plugins();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static LoadMode GetLoadMode_Mods();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr GetUnityTlsInterface();
    }
}