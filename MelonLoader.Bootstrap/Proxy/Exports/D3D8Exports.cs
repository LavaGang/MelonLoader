#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class D3D8Exports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplDirect3DCreate8")]
    public static void ImplDirect3DCreate8() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirect3D8EnableMaximizedWindowedModeShim")]
    public static void ImplDirect3D8EnableMaximizedWindowedModeShim() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplValidatePixelShader")]
    public static void ImplValidatePixelShader() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplValidateVertexShader")]
    public static void ImplValidateVertexShader() { }
}
#endif
