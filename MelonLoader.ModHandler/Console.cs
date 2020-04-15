using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;

namespace MelonLoader
{
    internal class Console
    {
        [DllImport("kernel32.dll")]
        private static extern int AllocConsole();

        [DllImport("user32.dll")]
        [return: MarshalAs(2)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        internal static void Create()
        {
            AllocConsole();
            System.Console.SetOut(new StreamWriter(System.Console.OpenStandardOutput()) { AutoFlush = true });
            System.Console.SetIn(new StreamReader(System.Console.OpenStandardInput()));
            System.Console.Clear();
            System.Console.Title = (BuildInfo.Name + " v" + BuildInfo.Version + " Closed-Beta");
            SetForegroundWindow(GetConsoleWindow());
        }
    }
}