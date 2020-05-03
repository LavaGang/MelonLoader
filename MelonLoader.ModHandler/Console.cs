using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;

namespace MelonLoader
{
    internal class Console
    {
        internal static bool Enabled = false;

        internal static void Create()
        {
            Imports.AllocConsole();
            System.Console.SetOut(new StreamWriter(System.Console.OpenStandardOutput()) { AutoFlush = true });
            System.Console.SetIn(new StreamReader(System.Console.OpenStandardInput()));
            System.Console.Clear();
            System.Console.Title = (BuildInfo.Name + " v" + BuildInfo.Version + " Open-Beta");
            Imports.SetForegroundWindow(Imports.GetConsoleWindow());
        }
    }
}