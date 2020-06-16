using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public class Console
    {
        public static bool Enabled = false;

        internal static void Create()
        {
            Allocate();
            System.Console.SetOut(new StreamWriter(System.Console.OpenStandardOutput()) { AutoFlush = true });
            System.Console.SetIn(new StreamReader(System.Console.OpenStandardInput()));
            System.Console.Clear();
            System.Console.Title = (BuildInfo.Name + " v" + BuildInfo.Version + " Open-Beta");
            IntPtr hwnd = GetHWND();
            SetForegroundWindow(hwnd);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int Allocate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(2)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern IntPtr GetHWND();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetColor(ConsoleColor color);
    }
}