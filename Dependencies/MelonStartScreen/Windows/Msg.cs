using System;
using System.Runtime.InteropServices;

namespace Windows
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Msg
    {
        public IntPtr hwnd;
        public WindowMessage message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point pt;
    }
}
