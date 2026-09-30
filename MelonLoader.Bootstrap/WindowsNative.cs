#if WINDOWS
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

internal static partial class WindowsNative
{
    internal const uint StdInputHandle = 4294967286;
    internal const uint StdOutputHandle = 4294967285;
    internal const uint StdErrorHandle = 4294967284;

    [LibraryImport("kernel32.dll", EntryPoint = "GetProcAddress", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint GetProcAddress(nint hModule, string lpProcName);
    
    [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryA", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint LoadLibrary(string lpLibFileName);

    [LibraryImport("kernel32.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint GetStdHandle(uint nStdHandle);

    [LibraryImport("kernel32.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int SetStdHandle(uint nStdHandle, nint handle);

    [LibraryImport("kernel32", EntryPoint = "CloseHandle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int CloseHandle(uint hObject);

    [LibraryImport("kernel32.dll", EntryPoint = "CreateFileW", StringMarshalling = StringMarshalling.Utf16)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint CreateFileW(string lpFileName,
        uint dwDesiredAccess,
        int dwShareMode,
        nint lpSecurityAttributes,
        int dwCreationDisposition,
        int dwFlagsAndAttributes,
        nint hTemplateFile);

    [LibraryImport("kernel32.dll", EntryPoint = "SetFilePointer")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int SetFilePointer(nint hFile, int lDistanceToMove, nint lpDistanceToMoveTo, int dwMoveMethod);

    [LibraryImport("kernel32", EntryPoint = "WriteFile")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int WriteFile(nint hFile, nint lpBuffer, int nNumberOfBytesToWrite,
                                        ref int lpNumberOfBytesWritten,  nint lpOverlapped);

    [LibraryImport("kernel32.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AllocConsole();

    [LibraryImport("kernel32.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint GetConsoleWindow();

    [LibraryImport("user32.dll")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
}
#endif