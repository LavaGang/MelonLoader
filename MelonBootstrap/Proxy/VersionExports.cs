using System.Runtime.InteropServices;

namespace MelonBootstrap.Proxy;

internal static class VersionExports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoSizeA")]
    public static void ImplGetFileVersionInfoSizeA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoA")]
    public static void ImplGetFileVersionInfoA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoByHandle")]
    public static void ImplGetFileVersionInfoByHandle() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoExA")]
    public static void ImplGetFileVersionInfoExA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoExW")]
    public static void ImplGetFileVersionInfoExW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoSizeExA")]
    public static void ImplGetFileVersionInfoSizeExA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoSizeExW")]
    public static void ImplGetFileVersionInfoSizeExW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoSizeW")]
    public static void ImplGetFileVersionInfoSizeW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetFileVersionInfoW")]
    public static void ImplGetFileVersionInfoW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplVerFindFileA")]
    public static void ImplVerFindFileA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplVerFindFileW")]
    public static void ImplVerFindFileW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplVerInstallFileA")]
    public static void ImplVerInstallFileA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplVerInstallFileW")]
    public static void ImplVerInstallFileW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplVerLanguageNameA")]
    public static void ImplVerLanguageNameA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplVerLanguageNameW")]
    public static void ImplVerLanguageNameW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplVerQueryValueA")]
    public static void ImplVerQueryValueA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplVerQueryValueW")]
    public static void ImplVerQueryValueW() { }
}
