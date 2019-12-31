using System;
using System.Runtime.InteropServices;

namespace MelonLoader
{
    public class Imports
    {
        [DllImport("MelonLoader\\MelonLoader", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr melonloader_get_il2cpp_domain();
        [DllImport("MelonLoader\\MelonLoader", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool melonloader_is_il2cpp_game();
        [DllImport("MelonLoader\\MelonLoader", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string melonloader_getcommandline();
        [DllImport("MelonLoader\\MelonLoader", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string melonloader_getgamepath();
        [DllImport("MelonLoader\\MelonLoader", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool melonloader_is_debug_mode();
    }
}