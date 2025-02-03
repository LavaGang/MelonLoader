using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MelonLoader.Utils;

public static class OsUtils
{
    public static PlatformID GetPlatform => Environment.OSVersion.Platform;
    public static bool IsUnix => GetPlatform is PlatformID.Unix;
    public static bool IsWindows => GetPlatform is PlatformID.Win32NT or PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.WinCE;
    public static bool IsMac => GetPlatform is PlatformID.MacOSX;

#if !NET35

        public static bool IsAndroid => RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));
        public static bool Is32Bit => !Environment.Is64BitProcess;
        public static bool Is64Bit => Environment.Is64BitProcess;

#else

    public static bool Is32Bit => IntPtr.Size == 4;
    public static bool Is64Bit => IntPtr.Size != 4;
    public static bool IsAndroid => false;

#endif

    public static string NativeFileExtension
    {
        get
        {
            if (IsUnix || IsAndroid)
                return ".so";
            if (IsMac)
                return ".dylib";
            return ".dll";
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    private delegate string dWineGetVersion();
    private static dWineGetVersion WineGetVersion;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint dRtlGetVersion(out OsVersionInfo versionInformation); // return type should be the NtStatus enum
    private static dRtlGetVersion RtlGetVersion;

    static OsUtils()
    {
        if (!IsWindows)
            return;

        if (!MelonNativeLibrary.TryLoadLib("ntdll.dll", out IntPtr ntdll))
            return;

        if (MelonNativeLibrary.TryGetExport(ntdll, "RtlGetVersion", out var rtlGetVersion))
            RtlGetVersion = rtlGetVersion.GetDelegate<dRtlGetVersion>();
        if (MelonNativeLibrary.TryGetExport(ntdll, "wine_get_version", out var wineGetVersion))
            WineGetVersion = wineGetVersion.GetDelegate<dWineGetVersion>();
    }

    public static bool IsWineOrProton()
        => WineGetVersion != null;

    public static void AddNativeDLLDirectory(string path)
    {
        if (!IsWindows && !IsUnix)
            return;

        path = Path.GetFullPath(path);
        if (!Directory.Exists(path))
            return;

        string envName = IsWindows ? "PATH" : "LD_LIBRARY_PATH";
        string envSep = IsWindows ? ";" : ":";
        string envPaths = Environment.GetEnvironmentVariable(envName);
        Environment.SetEnvironmentVariable(envName, $"{envPaths}{envSep}{path}");
    }

    public static string GetOSVersion()
    {
        if (!IsWindows)
            return Environment.OSVersion.VersionString;

        if (IsWineOrProton())
            return $"Wine {WineGetVersion()}";

        if (RtlGetVersion == null)
            return "Unknown";

        RtlGetVersion(out OsVersionInfo versionInformation);
        var minor = versionInformation.MinorVersion;
        var build = versionInformation.BuildNumber;

        string versionString = "";
        switch (versionInformation.MajorVersion)
        {
            case 4:
                versionString = "Windows 95/98/Me/NT";
                break;
            case 5:
                if (minor == 0)
                    versionString = "Windows 2000";
                if (minor == 1)
                    versionString = "Windows XP";
                if (minor == 2)
                    versionString = "Windows 2003";
                break;
            case 6:
                if (minor == 0)
                    versionString = "Windows Vista";
                if (minor == 1)
                    versionString = "Windows 7";
                if (minor == 2)
                    versionString = "Windows 8";
                if (minor == 3)
                    versionString = "Windows 8.1";
                break;
            case 10:
                if (build >= 22000)
                    versionString = "Windows 11";
                else
                    versionString = "Windows 10";
                break;
            default:
                versionString = "Unknown";
                break;
        }

        return $"{versionString}";
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct OsVersionInfo
    {
        private readonly uint OsVersionInfoSize;
        internal readonly uint MajorVersion;
        internal readonly uint MinorVersion;
        internal readonly uint BuildNumber;
        private readonly uint PlatformId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal readonly string CSDVersion;
    }
}