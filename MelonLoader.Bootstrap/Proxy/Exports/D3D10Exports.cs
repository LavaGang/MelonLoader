#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class D3D10Exports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10CompileEffectFromMemory")]
    public static void ImplD3D10CompileEffectFromMemory() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10CompileShader")]
    public static void ImplD3D10CompileShader() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10CreateBlob")]
    public static void ImplD3D10CreateBlob() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10CreateDevice")]
    public static void ImplD3D10CreateDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10CreateDeviceAndSwapChain")]
    public static void ImplD3D10CreateDeviceAndSwapChain() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10CreateEffectFromMemory")]
    public static void ImplD3D10CreateEffectFromMemory() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10CreateEffectPoolFromMemory")]
    public static void ImplD3D10CreateEffectPoolFromMemory() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10CreateStateBlock")]
    public static void ImplD3D10CreateStateBlock() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10DisassembleEffect")]
    public static void ImplD3D10DisassembleEffect() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10DisassembleShader")]
    public static void ImplD3D10DisassembleShader() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10GetGeometryShaderProfile")]
    public static void ImplD3D10GetGeometryShaderProfile() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10GetInputAndOutputSignatureBlob")]
    public static void ImplD3D10GetInputAndOutputSignatureBlob() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10GetInputSignatureBlob")]
    public static void ImplD3D10GetInputSignatureBlob() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10GetOutputSignatureBlob")]
    public static void ImplD3D10GetOutputSignatureBlob() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10GetPixelShaderProfile")]
    public static void ImplD3D10GetPixelShaderProfile() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10GetShaderDebugInfo")]
    public static void ImplD3D10GetShaderDebugInfo() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10GetVersion")]
    public static void ImplD3D10GetVersion() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10GetVertexShaderProfile")]
    public static void ImplD3D10GetVertexShaderProfile() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10PreprocessShader")]
    public static void ImplD3D10PreprocessShader() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10ReflectShader")]
    public static void ImplD3D10ReflectShader() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10RegisterLayers")]
    public static void ImplD3D10RegisterLayers() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10StateBlockMaskDifference")]
    public static void ImplD3D10StateBlockMaskDifference() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10StateBlockMaskDisableAll")]
    public static void ImplD3D10StateBlockMaskDisableAll() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10StateBlockMaskDisableCapture")]
    public static void ImplD3D10StateBlockMaskDisableCapture() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10StateBlockMaskEnableAll")]
    public static void ImplD3D10StateBlockMaskEnableAll() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10StateBlockMaskEnableCapture")]
    public static void ImplD3D10StateBlockMaskEnableCapture() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10StateBlockMaskGetSetting")]
    public static void ImplD3D10StateBlockMaskGetSetting() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10StateBlockMaskIntersect")]
    public static void ImplD3D10StateBlockMaskIntersect() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D10StateBlockMaskUnion")]
    public static void ImplD3D10StateBlockMaskUnion() { }
}
#endif
