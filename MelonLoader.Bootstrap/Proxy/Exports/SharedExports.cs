#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class SharedExports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplDllCanUnloadNow")]
    public static void ImplDllCanUnloadNow() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDllGetClassObject")]
    public static void ImplDllGetClassObject() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDllRegisterServer")]
    public static void ImplDllRegisterServer() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDllUnregisterServer")]
    public static void ImplDllUnregisterServer() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplPrivate1")]
    public static void ImplPrivate1() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDebugSetMute")]
    public static void ImplDebugSetMute() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDebugSetLevel")]
    public static void ImplDebugSetLevel() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplGetDeviceID")]
    public static void ImplGetDeviceID() { }
}
#endif
