#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class MSACM32Exports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverAddA")]
    public static void ImplacmDriverAddA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverAddW")]
    public static void ImplacmDriverAddW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverClose")]
    public static void ImplacmDriverClose() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverDetailsA")]
    public static void ImplacmDriverDetailsA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverDetailsW")]
    public static void ImplacmDriverDetailsW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverEnum")]
    public static void ImplacmDriverEnum() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverID")]
    public static void ImplacmDriverID() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverMessage")]
    public static void ImplacmDriverMessage() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverOpen")]
    public static void ImplacmDriverOpen() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverPriority")]
    public static void ImplacmDriverPriority() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmDriverRemove")]
    public static void ImplacmDriverRemove() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterChooseA")]
    public static void ImplacmFilterChooseA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterChooseW")]
    public static void ImplacmFilterChooseW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterDetailsA")]
    public static void ImplacmFilterDetailsA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterDetailsW")]
    public static void ImplacmFilterDetailsW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterEnumA")]
    public static void ImplacmFilterEnumA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterEnumW")]
    public static void ImplacmFilterEnumW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterTagDetailsA")]
    public static void ImplacmFilterTagDetailsA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterTagDetailsW")]
    public static void ImplacmFilterTagDetailsW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterTagEnumA")]
    public static void ImplacmFilterTagEnumA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFilterTagEnumW")]
    public static void ImplacmFilterTagEnumW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatChooseA")]
    public static void ImplacmFormatChooseA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatChooseW")]
    public static void ImplacmFormatChooseW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatDetailsA")]
    public static void ImplacmFormatDetailsA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatDetailsW")]
    public static void ImplacmFormatDetailsW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatEnumA")]
    public static void ImplacmFormatEnumA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatEnumW")]
    public static void ImplacmFormatEnumW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatSuggest")]
    public static void ImplacmFormatSuggest() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatTagDetailsA")]
    public static void ImplacmFormatTagDetailsA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatTagDetailsW")]
    public static void ImplacmFormatTagDetailsW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatTagEnumA")]
    public static void ImplacmFormatTagEnumA() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmFormatTagEnumW")]
    public static void ImplacmFormatTagEnumW() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmGetVersion")]
    public static void ImplacmGetVersion() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmMetrics")]
    public static void ImplacmMetrics() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmStreamClose")]
    public static void ImplacmStreamClose() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmStreamConvert")]
    public static void ImplacmStreamConvert() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmStreamMessage")]
    public static void ImplacmStreamMessage() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmStreamOpen")]
    public static void ImplacmStreamOpen() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmStreamPrepareHeader")]
    public static void ImplacmStreamPrepareHeader() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmStreamReset")]
    public static void ImplacmStreamReset() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmStreamSize")]
    public static void ImplacmStreamSize() { }
    [UnmanagedCallersOnly(EntryPoint = "ImplacmStreamUnprepareHeader")]
    public static void ImplacmStreamUnprepareHeader() { }
}
#endif
