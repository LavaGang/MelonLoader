#if WINDOWS
using System.Runtime.InteropServices;
#endif

namespace MelonLoader.Bootstrap.Utils;

internal static class WineUtils
{
    public static bool IsWine { get; }

    static WineUtils()
    {
#if WINDOWS
        if (NativeLibrary.TryLoad("ntdll.dll", out var ntdll) && NativeLibrary.TryGetExport(ntdll, "wine_get_version", out _))
            IsWine = true;
#endif
#if LINUX
        IsWine = false;
#endif
    }
}
