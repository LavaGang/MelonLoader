#if WINDOWS
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy.Exports;

internal static class DSoundExports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundCaptureCreate")]
    public static void ImplDirectSoundCaptureCreate() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundCaptureCreate8")]
    public static void ImplDirectSoundCaptureCreate8() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundCaptureEnumerateA")]
    public static void ImplDirectSoundCaptureEnumerateA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundCaptureEnumerateW")]
    public static void ImplDirectSoundCaptureEnumerateW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundCreate")]
    public static void ImplDirectSoundCreate() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundCreate8")]
    public static void ImplDirectSoundCreate8() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundEnumerateA")]
    public static void ImplDirectSoundEnumerateA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundEnumerateW")]
    public static void ImplDirectSoundEnumerateW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDirectSoundFullDuplexCreate")]
    public static void ImplDirectSoundFullDuplexCreate() { }
}
#endif