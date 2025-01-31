#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class D3D9Exports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPERF_BeginEvent")]
    public static void ImplD3DPERF_BeginEvent() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPERF_EndEvent")]
    public static void ImplD3DPERF_EndEvent() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPERF_GetStatus")]
    public static void ImplD3DPERF_GetStatus() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPERF_QueryRepeatFrame")]
    public static void ImplD3DPERF_QueryRepeatFrame() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPERF_SetMarker")]
    public static void ImplD3DPERF_SetMarker() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPERF_SetOptions")]
    public static void ImplD3DPERF_SetOptions() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPERF_SetRegion")]
    public static void ImplD3DPERF_SetRegion() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirect3D9EnableMaximizedWindowedModeShim")]
    public static void ImplDirect3D9EnableMaximizedWindowedModeShim() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirect3DCreate9")]
    public static void ImplDirect3DCreate9() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirect3DCreate9Ex")]
    public static void ImplDirect3DCreate9Ex() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirect3DCreate9On12")]
    public static void ImplDirect3DCreate9On12() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirect3DCreate9On12Ex")]
    public static void ImplDirect3DCreate9On12Ex() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplDirect3DShaderValidatorCreate9")]
    public static void ImplDirect3DShaderValidatorCreate9() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplPSGPError")]
    public static void ImplPSGPError() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplPSGPSampleTexture")]
    public static void ImplPSGPSampleTexture() { }
}
#endif
