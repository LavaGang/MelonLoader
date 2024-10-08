using System;
using System.Runtime.InteropServices;
using MelonLoader.NativeUtils;

namespace MelonLoader.Utils;

internal static class OsUtils
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    private delegate string dWineGetVersion();
    private static dWineGetVersion WineGetVersion;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    private delegate uint dRtlGetVersion(out OsVersionInfo versionInformation); // return type should be the NtStatus enum
    private static dRtlGetVersion RtlGetVersion;

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

    static OsUtils()
    {
        if (!MelonUtils.IsWindows)
            return;
        
        IntPtr ntdll = MelonNativeLibrary.Load("ntdll.dll");
        
        if (MelonNativeLibrary.TryGetExport<dRtlGetVersion>(ntdll, "RtlGetVersion", out var rtlGetVersion))
            RtlGetVersion = rtlGetVersion;
        
        if (MelonNativeLibrary.TryGetExport<dWineGetVersion>(ntdll, "wine_get_version", out var wineGetVersion))
            WineGetVersion = wineGetVersion;
    }

    internal static bool IsWineOrProton()
        => WineGetVersion != null;

    internal static string GetOSVersion()
    {
        if (!MelonUtils.IsWindows)
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
}