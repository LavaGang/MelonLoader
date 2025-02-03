#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class D3D11Exports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplCreateDirect3D11DeviceFromDXGIDevice")]
    public static void ImplCreateDirect3D11DeviceFromDXGIDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplCreateDirect3D11SurfaceFromDXGISurface")]
    public static void ImplCreateDirect3D11SurfaceFromDXGISurface() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D11CoreCreateDevice")]
    public static void ImplD3D11CoreCreateDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D11CoreCreateLayeredDevice")]
    public static void ImplD3D11CoreCreateLayeredDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D11CoreGetLayeredDeviceSize")]
    public static void ImplD3D11CoreGetLayeredDeviceSize() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D11CoreRegisterLayers")]
    public static void ImplD3D11CoreRegisterLayers() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D11CreateDevice")]
    public static void ImplD3D11CreateDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D11CreateDeviceAndSwapChain")]
    public static void ImplD3D11CreateDeviceAndSwapChain() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D11CreateDeviceForD3D12")]
    public static void ImplD3D11CreateDeviceForD3D12() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3D11On12CreateDevice")]
    public static void ImplD3D11On12CreateDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTCloseAdapter")]
    public static void ImplD3DKMTCloseAdapter() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTCreateAllocation")]
    public static void ImplD3DKMTCreateAllocation() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTCreateContext")]
    public static void ImplD3DKMTCreateContext() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTCreateDevice")]
    public static void ImplD3DKMTCreateDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTCreateSynchronizationObject")]
    public static void ImplD3DKMTCreateSynchronizationObject() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTDestroyAllocation")]
    public static void ImplD3DKMTDestroyAllocation() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTDestroyContext")]
    public static void ImplD3DKMTDestroyContext() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTDestroyDevice")]
    public static void ImplD3DKMTDestroyDevice() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTDestroySynchronizationObject")]
    public static void ImplD3DKMTDestroySynchronizationObject() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTEscape")]
    public static void ImplD3DKMTEscape() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTGetContextSchedulingPriority")]
    public static void ImplD3DKMTGetContextSchedulingPriority() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTGetDeviceState")]
    public static void ImplD3DKMTGetDeviceState() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTGetDisplayModeList")]
    public static void ImplD3DKMTGetDisplayModeList() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTGetMultisampleMethodList")]
    public static void ImplD3DKMTGetMultisampleMethodList() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTGetRuntimeData")]
    public static void ImplD3DKMTGetRuntimeData() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTGetSharedPrimaryHandle")]
    public static void ImplD3DKMTGetSharedPrimaryHandle() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTLock")]
    public static void ImplD3DKMTLock() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTOpenAdapterFromHdc")]
    public static void ImplD3DKMTOpenAdapterFromHdc() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTOpenResource")]
    public static void ImplD3DKMTOpenResource() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTPresent")]
    public static void ImplD3DKMTPresent() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTQueryAdapterInfo")]
    public static void ImplD3DKMTQueryAdapterInfo() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTQueryAllocationResidency")]
    public static void ImplD3DKMTQueryAllocationResidency() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTQueryResourceInfo")]
    public static void ImplD3DKMTQueryResourceInfo() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTRender")]
    public static void ImplD3DKMTRender() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTSetAllocationPriority")]
    public static void ImplD3DKMTSetAllocationPriority() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTSetContextSchedulingPriority")]
    public static void ImplD3DKMTSetContextSchedulingPriority() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTSetDisplayMode")]
    public static void ImplD3DKMTSetDisplayMode() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTSetDisplayPrivateDriverFormat")]
    public static void ImplD3DKMTSetDisplayPrivateDriverFormat() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTSetGammaRamp")]
    public static void ImplD3DKMTSetGammaRamp() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTSetVidPnSourceOwner")]
    public static void ImplD3DKMTSetVidPnSourceOwner() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTSignalSynchronizationObject")]
    public static void ImplD3DKMTSignalSynchronizationObject() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTUnlock")]
    public static void ImplD3DKMTUnlock() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTWaitForSynchronizationObject")]
    public static void ImplD3DKMTWaitForSynchronizationObject() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DKMTWaitForVerticalBlankEvent")]
    public static void ImplD3DKMTWaitForVerticalBlankEvent() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPerformance_BeginEvent")]
    public static void ImplD3DPerformance_BeginEvent() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPerformance_EndEvent")]
    public static void ImplD3DPerformance_EndEvent() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPerformance_GetStatus")]
    public static void ImplD3DPerformance_GetStatus() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplD3DPerformance_SetMarker")]
    public static void ImplD3DPerformance_SetMarker() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplEnableFeatureLevelUpgrade")]
    public static void ImplEnableFeatureLevelUpgrade() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplOpenAdapter10")]
    public static void ImplOpenAdapter10() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplOpenAdapter10_2")]
    public static void ImplOpenAdapter10_2() { }
}
#endif
