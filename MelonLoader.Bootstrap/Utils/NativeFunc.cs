using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Utils;

internal static class NativeFunc
{
    internal static unsafe nint NativeLoadLib(nint libraryPath)
    {
        if (libraryPath == 0)
            return 0;

        string? libraryPathStr = Marshal.PtrToStringAnsi(libraryPath);
        if (string.IsNullOrEmpty(libraryPathStr))
            return 0;

        if (!NativeLibrary.TryLoad(libraryPathStr, out nint addr))
            return 0;

        return addr;
    }

    internal static unsafe nint NativeGetExport(nint lib, nint name)
    {
        if (lib == 0)
            return 0;

        string? nameStr = Marshal.PtrToStringAnsi(name);
        if (string.IsNullOrEmpty(nameStr))
            return 0;

        if (!NativeLibrary.TryGetExport(lib, nameStr, out nint addr))
            return 0;

        return addr;
    }

    public static T? GetExport<T>(nint hModule, string name) where T : Delegate
    {
        return !NativeLibrary.TryGetExport(hModule, name, out var export) ? null : Marshal.GetDelegateForFunctionPointer<T>(export);
    }

    public static bool GetExport<T>(nint hModule, string name, [NotNullWhen(true)] out T? func) where T : Delegate
    {
        func = GetExport<T>(hModule, name);
        return func != null;
    }
}
