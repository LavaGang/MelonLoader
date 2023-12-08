using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Utils
{
    public static class MelonAssertion
    {
        private static bool ShouldContinue = true;

        //TODO: Could this be done in a better way? net35/6 load PresentationFramework differently so I could not rely on it
        //This crashes with start screen enabled
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr MessageBox(int hWnd, string text, string caption, uint type);

        public static void ThrowInternalFailure(string msg)
        {
            if (!ShouldContinue)
                return;

            ShouldContinue = false;
            var timestamp = MelonUtils.TimeStamp;
            BootstrapInterop.NativeWriteLogToFile($"[{timestamp}] [INTERNAL FAILURE] {msg}");
            string caption = "INTERNAL FAILURE!";
                
            IntPtr result = IntPtr.Zero;
            if (MelonUtils.IsWindows)
                result = MessageBox(0, msg, caption, 0);
            while (result == IntPtr.Zero)
                Environment.Exit(1);
        }
    }
}