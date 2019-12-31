using System;
using System.Runtime.InteropServices;
using System.IO;

namespace MelonLoader
{
    internal class DebugConsole
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
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
            Console.Clear();
            //Console.Title = (FileInfo.Name + " v" + FileInfo.Version);
            Console.Title = (BuildInfo.Name + " v" + BuildInfo.Version + " Closed-Beta");
            SetForegroundWindow(GetConsoleWindow());
        }
    }
}