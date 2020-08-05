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

        /*
        private static void RunLogCallbacks(string msg) => LogCallbackHandler?.Invoke(msg);
        public static event Action<string> LogCallbackHandler;
        private static void RunWarningCallbacks(string msg) => WarningCallbackHandler1?.Invoke(msg);
        private static void RunWarningCallbacks(string namesection, string msg) => WarningCallbackHandler2?.Invoke(namesection, msg);
        public static event Action<string> WarningCallbackHandler1;
        public static event Action<string, string> WarningCallbackHandler2;
        private static void RunErrorCallbacks(string msg) => ErrorCallbackHandler1?.Invoke(msg);
        private static void RunErrorCallbacks(string namesection, string msg) => ErrorCallbackHandler2?.Invoke(namesection, msg);
        public static event Action<string> ErrorCallbackHandler1;
        public static event Action<string, string> ErrorCallbackHandler2;
        */
    }
}