using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MelonBootstrap.Proxy;

[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "We don't need extra marshalling code.")]
internal static class VersionExports
{
    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoSizeA")]
    [DllImport(@"System32\version.dll")]
    private static extern uint GetFileVersionInfoSizeA(nint a, nint b);

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoA")]
    [DllImport(@"System32\version.dll")]
    private static extern byte GetFileVersionInfoA(nint a, uint b, uint c, nint d);

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoByHandle")]
    [DllImport(@"System32\version.dll")]
    private static extern byte GetFileVersionInfoByHandle(uint a, nint b, nint c, nint d);

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoExA")]
    [DllImport(@"System32\version.dll")]
    private static extern byte GetFileVersionInfoExA(uint a, nint b, uint c, uint d, nint e);

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoExW")]
    [DllImport(@"System32\version.dll")]
    private static extern byte GetFileVersionInfoExW(uint a, nint b, uint c, uint d, nint e);

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoSizeExA")]
    [DllImport(@"System32\version.dll")]
    private static extern uint GetFileVersionInfoSizeExA(uint a, nint b, nint c);

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoSizeExW")]
    [DllImport(@"System32\version.dll")]
    private static extern uint GetFileVersionInfoSizeExW(uint a, nint b, nint c);

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoSizeW")]
    [DllImport(@"System32\version.dll")]
    private static extern uint GetFileVersionInfoSizeW(nint a, nint b);

    [UnmanagedCallersOnly(EntryPoint = "GetFileVersionInfoW")]
    [DllImport(@"System32\version.dll")]
    private static extern byte GetFileVersionInfoW(nint a, uint b, uint c, nint d);

    [UnmanagedCallersOnly(EntryPoint = "VerFindFileA")]
    [DllImport(@"System32\version.dll")]
    private static extern uint VerFindFileA(uint a, nint b, nint c, nint d, nint e, nint f, nint g, nint h);

    [UnmanagedCallersOnly(EntryPoint = "VerFindFileW")]
    [DllImport(@"System32\version.dll")]
    private static extern uint VerFindFileW(uint a, nint b, nint c, nint d, nint e, nint f, nint g, nint h);

    [UnmanagedCallersOnly(EntryPoint = "VerInstallFileA")]
    [DllImport(@"System32\version.dll")]
    private static extern uint VerInstallFileA(uint a, nint b, nint c, nint d, nint e, nint f, nint g, nint h);

    [UnmanagedCallersOnly(EntryPoint = "VerInstallFileW")]
    [DllImport(@"System32\version.dll")]
    private static extern uint VerInstallFileW(uint a, nint b, nint c, nint d, nint e, nint f, nint g, nint h);

    [UnmanagedCallersOnly(EntryPoint = "VerLanguageNameA")]
    [DllImport(@"System32\version.dll")]
    private static extern uint VerLanguageNameA(uint a, nint b, uint c);

    [UnmanagedCallersOnly(EntryPoint = "VerLanguageNameW")]
    [DllImport(@"System32\version.dll")]
    private static extern uint VerLanguageNameW(uint a, nint b, uint c);

    [UnmanagedCallersOnly(EntryPoint = "VerQueryValueA")]
    [DllImport(@"System32\version.dll")]
    private static extern byte VerQueryValueA(nint a, nint b, nint c, nint d);

    [UnmanagedCallersOnly(EntryPoint = "VerQueryValueW")]
    [DllImport(@"System32\version.dll")]
    private static extern byte VerQueryValueW(nint a, nint b, nint c, nint d);
}
