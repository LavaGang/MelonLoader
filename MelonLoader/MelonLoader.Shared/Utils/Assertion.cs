using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Shared.Utils
{
    public static class Assertion
    {
        internal static bool ShouldContinue = true;

        //TODO: Could this be done in a better way? net35/6 load PresentationFramework differently so I could not rely on it
        //This crashes with start screen enabled
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr MessageBox(int hWnd, String text, String caption, uint type);

        public static void ThrowInternalFailure(string msg)
        {
            if (ShouldContinue)
            {
                ShouldContinue = false;
                var timestamp = LoggerUtils.GetTimeStamp();
                MelonLogger.LogWriter.WriteLine($"[{timestamp}] [INTERNAL FAILURE] {msg}");
                string caption = "INTERNAL FAILURE!";
                
                IntPtr result = IntPtr.Zero;
                
                if (MelonUtils.IsWindows)
                    result = MessageBox(0, msg, caption, 0);

                while (result == IntPtr.Zero)

                    Environment.Exit(1);
            }
        }
    }
}