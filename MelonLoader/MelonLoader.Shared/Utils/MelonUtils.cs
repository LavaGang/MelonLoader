using System;

namespace MelonLoader.Utils
{
    public static class MelonUtils
    {
        public static PlatformID GetPlatform => Environment.OSVersion.Platform;

        public static bool IsUnix => GetPlatform is PlatformID.Unix;

        public static bool IsWindows =>
            GetPlatform is PlatformID.Win32NT or PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.WinCE;

        public static bool IsMac => GetPlatform is PlatformID.MacOSX;
        
#if !NET35
        public static bool IsGame32Bit => !Environment.Is64BitProcess;
#else
        public static bool IsGame32Bit => IntPtr.Size == 4;
#endif
        
        public static bool IsTypeOf<T>(this Type type)
        {
            return type.IsAssignableFrom(typeof(T));
        }
    }
}