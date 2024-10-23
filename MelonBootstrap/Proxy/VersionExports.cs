using System.Runtime.InteropServices;

namespace MelonBootstrap.Proxy;

internal static class VersionExports
{
    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoSizeA")]
    public static void GetFileVersionInfoSizeA() { }

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoA")]
    public static void GetFileVersionInfoA() { }

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoByHandle")]
    public static void GetFileVersionInfoByHandle() { }

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoExA")]
    public static void GetFileVersionInfoExA() { }

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoExW")]
    public static void GetFileVersionInfoExW() { }

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoSizeExA")]
    public static void GetFileVersionInfoSizeExA() { }

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoSizeExW")]
    public static void GetFileVersionInfoSizeExW() { }

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoSizeW")]
    public static void GetFileVersionInfoSizeW() { }

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoW")]
    public static void GetFileVersionInfoW() { }

    [UnmanagedCallersOnly(EntryPoint = "VerFindFileA")]
    public static void VerFindFileA() { }

    [UnmanagedCallersOnly(EntryPoint = "VerFindFileW")]
    public static void VerFindFileW() { }

    [UnmanagedCallersOnly(EntryPoint = "VerInstallFileA")]
    public static void VerInstallFileA() { }

    [UnmanagedCallersOnly(EntryPoint = "VerInstallFileW")]
    public static void VerInstallFileW() { }

    [UnmanagedCallersOnly(EntryPoint = "VerLanguageNameA")]
    public static void VerLanguageNameA() { }

    [UnmanagedCallersOnly(EntryPoint = "VerLanguageNameW")]
    public static void VerLanguageNameW() { }

    [UnmanagedCallersOnly(EntryPoint = "VerQueryValueA")]
    public static void VerQueryValueA() { }

    [UnmanagedCallersOnly(EntryPoint = "VerQueryValueW")]
    public static void VerQueryValueW() { }
}
