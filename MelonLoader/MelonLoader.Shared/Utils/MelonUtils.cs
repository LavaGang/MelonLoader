using System;
using System.Drawing;

#if !NET35
using System.Runtime.InteropServices;
#endif

namespace MelonLoader.Utils
{
    public static class MelonUtils
    {
        public static Color DefaultTextColor 
            => Color.White;
        public static ConsoleColor DefaultTextConsoleColor 
            => ConsoleColor.White;

        public static string TimeStamp
            => $"{DateTime.Now:HH:mm:ss.fff}";

        public static PlatformID Platform 
            => Environment.OSVersion.Platform;

        public static bool IsUnix
            => !IsAndroid 
                && Platform is PlatformID.Unix;

        public static bool IsWindows
            => Platform is PlatformID.Win32NT 
                or PlatformID.Win32S
                or PlatformID.Win32Windows
                or PlatformID.WinCE;

        public static bool IsMac 
            => Platform is PlatformID.MacOSX;

#if !NET35
        public static bool IsAndroid => RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));
        public static bool IsGame32Bit => !Environment.Is64BitProcess;
#else
        public static bool IsGame32Bit => IntPtr.Size == 4;
        public static bool IsAndroid => false;
#endif
    }
}