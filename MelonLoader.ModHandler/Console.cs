using System;
using System.IO;
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
            SetTitle(BuildInfo.Name + " v" + BuildInfo.Version + " Open-Beta");
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Allocate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetTitle(string title);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetColor(ConsoleColor color);

        internal static void RunLogCallbacks(string namesection, string msg) => LogCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> LogCallbackHandler;
        internal static void RunWarningCallbacks(string namesection, string msg) => WarningCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> WarningCallbackHandler;
        internal static void RunErrorCallbacks(string namesection, string msg) => ErrorCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> ErrorCallbackHandler;
    }
}