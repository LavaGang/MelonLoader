using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;

namespace MelonLoader
{
    internal class Console
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int AllocConsole();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(2)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern IntPtr GetConsoleWindow();

        internal static void Create()
        {
            AllocConsole();
            System.Console.SetOut(new StreamWriter(System.Console.OpenStandardOutput()) { AutoFlush = true });
            System.Console.SetIn(new StreamReader(System.Console.OpenStandardInput()));
            System.Console.Clear();
            System.Console.Title = (BuildInfo.Name + " v" + BuildInfo.Version + " Open-Beta");
            SetForegroundWindow(GetConsoleWindow());
        }
    }
}