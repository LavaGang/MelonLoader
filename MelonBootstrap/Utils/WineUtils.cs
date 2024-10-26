using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Utils;

internal static class WineUtils
{
    public static bool IsWine { get; private set; }

    static WineUtils()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        if (NativeLibrary.TryLoad("ntdll.dll", out var ntdll) && NativeLibrary.TryGetExport(ntdll, "wine_get_version", out _))
            IsWine = true;
    }
}
