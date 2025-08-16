#if WINDOWS
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace MelonLoader.Fixes
{
    internal class WindowsUnhandledQuit
    {
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(
            IntPtr HandlerRoutine,
            bool Add);

        private delegate bool HandlerRoutine(int dwCtrlType);

        private static bool SetConsoleCtrlHandler(HandlerRoutine HandlerRoutine, bool Add)
            => SetConsoleCtrlHandler(HandlerRoutine.GetFunctionPointer(), Add);

        private static bool Exit = false;

        public static void Install()
        {
            SetConsoleCtrlHandler((type) =>
            {
                if (type == ((int)CtrlType.CTRL_CLOSE_EVENT))
                {
                    Exit = true;
                    while (Exit) Thread.Sleep(200);
                }
                return true;
            }, true);
        }

        internal static void Update()
        {
            if (Exit)
            {
                try
                {
                    MelonEvents.OnApplicationDefiniteQuit.Invoke();
                    Core.Quit();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error("An unexpected error has occurred while cleaning up before closing", ex);
                }
                Exit = false;
            }
        }

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
    }
}
#endif