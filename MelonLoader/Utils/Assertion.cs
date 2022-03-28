using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MelonLoader.Utils
{
    internal static class Assertion
    {
        internal static bool ShouldContinue = true;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr MessageBox(int hWnd, String text, String caption, uint type);

        internal static void ThrowInternalFailure(string msg)
        {
            if (ShouldContinue)
            {
                ShouldContinue = false;
                var timestamp = LoggerUtils.GetTimeStamp();
                MelonLogger.LogWriter.WriteLine($"[{timestamp}] [INTERNAL FAILURE] {msg}");
                string caption = "INTERNAL FAILURE!";
                
                var result = MessageBox(0, msg, caption, 0);

                while (result == IntPtr.Zero)

                Environment.Exit(1);
            }
        }
    }
}
