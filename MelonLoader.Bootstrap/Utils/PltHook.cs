using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.Bootstrap.Logging;

namespace MelonLoader.Bootstrap;

internal static partial class PltHook
{
    [LibraryImport("*", EntryPoint = "plthook_open", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int PlthookOpen(ref nint pltHookOut, string? filename);

    [LibraryImport("*", EntryPoint = "plthook_open_by_handle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int PlthookOpenByHandle(ref nint pltHookOut, nint handle);

    [LibraryImport("*", EntryPoint = "plthook_open_by_address")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int PlthookOpenByAddress(ref nint pltHookOut, nint address);

    [LibraryImport("*", EntryPoint = "plthook_replace", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static unsafe partial int PlthookReplace(nint pltHook, string funcName, nint funcAddr, nint oldFunc);

    [LibraryImport("*", EntryPoint = "plthook_close")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void PlthookClose(nint pltHook);

    [LibraryImport("*", EntryPoint = "plthook_enum", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static unsafe partial int PlthookEnum(nint pltHook, ref uint pos, byte** name, byte*** addr);

    [LibraryImport("*", EntryPoint = "plthook_error")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint PlthookError();
}