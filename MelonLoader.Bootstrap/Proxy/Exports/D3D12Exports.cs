#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class D3D12Exports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12CoreCreateLayeredDevice")]
    public static void ImplD3D12CoreCreateLayeredDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12CoreGetLayeredDeviceSize")]
    public static void ImplD3D12CoreGetLayeredDeviceSize() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12CoreRegisterLayers")]
    public static void ImplD3D12CoreRegisterLayers() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12CreateDevice")]
    public static void ImplD3D12CreateDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12CreateRootSignatureDeserializer")]
    public static void ImplD3D12CreateRootSignatureDeserializer() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12CreateVersionedRootSignatureDeserializer")]
    public static void ImplD3D12CreateVersionedRootSignatureDeserializer() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12DeviceRemovedExtendedData")]
    public static void ImplD3D12DeviceRemovedExtendedData() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12EnableExperimentalFeatures")]
    public static void ImplD3D12EnableExperimentalFeatures() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12GetDebugInterface")]
    public static void ImplD3D12GetDebugInterface() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12GetInterface")]
    public static void ImplD3D12GetInterface() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12PIXEventsReplaceBlock")]
    public static void ImplD3D12PIXEventsReplaceBlock() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12PIXGetThreadInfo")]
    public static void ImplD3D12PIXGetThreadInfo() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12PIXNotifyWakeFromFenceSignal")]
    public static void ImplD3D12PIXNotifyWakeFromFenceSignal() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12PIXReportCounter")]
    public static void ImplD3D12PIXReportCounter() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12SerializeRootSignature")]
    public static void ImplD3D12SerializeRootSignature() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D12SerializeVersionedRootSignature")]
    public static void ImplD3D12SerializeVersionedRootSignature() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplGetBehaviorValue")]
    public static void ImplGetBehaviorValue() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplSetAppCompatStringPointer")]
    public static void ImplSetAppCompatStringPointer() { }
}
#endif
