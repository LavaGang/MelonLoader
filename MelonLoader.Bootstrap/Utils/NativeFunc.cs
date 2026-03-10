using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Utils;

internal static class NativeFunc
{
    public static bool TryGetExport(nint hModule, string name, out nint export)
    {
        export = ModuleSymbolRedirect.GetSymbol(hModule, name);
        if (export == IntPtr.Zero)
            return false;
        return true;
    }

    public static T? GetExport<T>(nint hModule, string name) where T : Delegate
    {
        nint exportPtr = ModuleSymbolRedirect.GetSymbol(hModule, name);
        if (exportPtr == IntPtr.Zero)
            return null;
        return Marshal.GetDelegateForFunctionPointer<T>(exportPtr);
    }

    public static bool GetExport<T>(nint hModule, string name, [NotNullWhen(true)] out T? func) where T : Delegate
    {
        func = GetExport<T>(hModule, name);
        return func != null;
    }
}
