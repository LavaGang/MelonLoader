using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Utils;

internal static class NativeFunc
{
    public static T? GetExport<T>(nint hModule, string name) where T : Delegate
    {
        if (!NativeLibrary.TryGetExport(hModule, name, out var export))
            return null;

        return Marshal.GetDelegateForFunctionPointer<T>(export);
    }

    public static bool GetExport<T>(nint hModule, string name, [NotNullWhen(true)] out T? func) where T : Delegate
    {
        func = GetExport<T>(hModule, name);
        return func != null;
    }
}
