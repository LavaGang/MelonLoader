#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class DDrawExports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplAcquireDDThreadLock")]
    public static void ImplAcquireDDThreadLock() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplCompleteCreateSysmemSurface")]
    public static void ImplCompleteCreateSysmemSurface() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DParseUnknownCommand")]
    public static void ImplD3DParseUnknownCommand() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDDGetAttachedSurfaceLcl")]
    public static void ImplDDGetAttachedSurfaceLcl() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDDInternalLock")]
    public static void ImplDDInternalLock() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDDInternalUnlock")]
    public static void ImplDDInternalUnlock() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDSoundHelp")]
    public static void ImplDSoundHelp() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectDrawCreate")]
    public static void ImplDirectDrawCreate() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectDrawCreateClipper")]
    public static void ImplDirectDrawCreateClipper() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectDrawCreateEx")]
    public static void ImplDirectDrawCreateEx() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectDrawEnumerateA")]
    public static void ImplDirectDrawEnumerateA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectDrawEnumerateExA")]
    public static void ImplDirectDrawEnumerateExA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectDrawEnumerateExW")]
    public static void ImplDirectDrawEnumerateExW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectDrawEnumerateW")]
    public static void ImplDirectDrawEnumerateW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplGetDDSurfaceLocal")]
    public static void ImplGetDDSurfaceLocal() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplGetOLEThunkData")]
    public static void ImplGetOLEThunkData() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplGetSurfaceFromDC")]
    public static void ImplGetSurfaceFromDC() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplRegisterSpecialCase")]
    public static void ImplRegisterSpecialCase() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplReleaseDDThreadLock")]
    public static void ImplReleaseDDThreadLock() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplSetAppCompatData")]
    public static void ImplSetAppCompatData() { }
}
#endif
