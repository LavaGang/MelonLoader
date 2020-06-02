using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public static class Imports
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void UNLOAD_MELONLOADER(bool alt = false);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Logger_Log(string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Logger_LogColor(string txt, ConsoleColor color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Logger_LogWarning(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Logger_LogError(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Logger_LogModError(string namesection, string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Logger_LogModStatus(int type);
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
        internal extern static bool IsRainbowMode();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static bool IsRandomRainbowMode();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void Hook(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void Unhook(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int AllocConsole();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(2)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern IntPtr GetConsoleWindow();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Console_SetColor(ConsoleColor color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static bool IsOldMono();
    }
}