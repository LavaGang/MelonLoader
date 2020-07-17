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
        private static extern void SetTitle(string title);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetColor(ConsoleColor color);

        private static void RunLogCallbacks(string msg) => LogCallbackHandler?.Invoke(msg);
        public static event Action<string> LogCallbackHandler;
        private static string RunLogOverrideCallbacks(string msg) => LogOverrideCallbackHandler?.Invoke(msg);
        public static event Func<string, string> LogOverrideCallbackHandler;
        private static void RunWarningCallbacks(string msg) => WarningCallbackHandler?.Invoke(msg);
        public static event Action<string> WarningCallbackHandler;
        private static string RunWarningOverrideCallbacks(string msg) => WarningOverrideCallbackHandler?.Invoke(msg);
        public static event Func<string, string> WarningOverrideCallbackHandler;
        private static void RunErrorCallbacks(string msg) => ErrorCallbackHandler?.Invoke(msg);
        public static event Action<string> ErrorCallbackHandler;
        private static string RunErrorOverrideCallbacks(string msg) => ErrorOverrideCallbackHandler?.Invoke(msg);
        public static event Func<string, string> ErrorOverrideCallbackHandler;
    }
}