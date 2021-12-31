using System;
using System.Runtime.InteropServices;

namespace Windows
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref Msg lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref Msg lpmsg);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);
        public delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern bool KillTimer(IntPtr hWnd, IntPtr uIDEvent);

        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(uint uFormat, ref DropFile hMem);
    }
}
