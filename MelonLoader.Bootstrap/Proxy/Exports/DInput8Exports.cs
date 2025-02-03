#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class DInput8Exports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectInput8Create")]
    public static void ImplDirectInput8Create() { }
}
#endif
