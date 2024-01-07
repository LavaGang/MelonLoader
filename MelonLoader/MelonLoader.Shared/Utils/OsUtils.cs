using System;
using System.Runtime.InteropServices;
using MelonLoader.NativeUtils;

namespace MelonLoader.Utils;

public class OsUtils
{
    [DllImport("ntdll.dll", SetLastError = true)]
    internal static extern uint
        RtlGetVersion(out OsVersionInfo versionInformation); // return type should be the NtStatus enum

    internal static MelonNativeLibrary.StringDelegate WineGetVersion;

    [StructLayout(LayoutKind.Sequential)]
    internal struct OsVersionInfo
    {
        private readonly uint OsVersionInfoSize;

        internal readonly uint MajorVersion;
        internal readonly uint MinorVersion;

        internal readonly uint BuildNumber;

        private readonly uint PlatformId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal readonly string CSDVersion;
    }

    internal static string GetOSVersion()
    {
        if (MelonUtils.IsUnix || MelonUtils.IsMac)
            return Environment.OSVersion.VersionString;

        if (MelonUtils.IsUnderWineOrSteamProton())
            return $"Wine {WineGetVersion()}";
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

    internal static void SetupWineCheck()
    {
        if (MelonUtils.IsUnix || MelonUtils.IsMac)
            return;

        IntPtr dll = MelonNativeLibrary.Load("ntdll.dll");
        MelonNativeLibrary.TryGetExport(dll, "wine_get_version", out IntPtr wineGetVersionProc);
        if (wineGetVersionProc == IntPtr.Zero)
            return;

        WineGetVersion = (MelonNativeLibrary.StringDelegate)Marshal.GetDelegateForFunctionPointer(
            wineGetVersionProc,
            typeof(MelonNativeLibrary.StringDelegate)
        );
    }
}