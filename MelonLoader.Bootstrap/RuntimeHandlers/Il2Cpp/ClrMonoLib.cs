#if ANDROID
using MelonLoader.Bootstrap.Proxy.Android;
using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

// NOTE: parts of this are specific to a LemonLoader fork of dotnet/runtime
internal class ClrMonoLib
{
    public required nint Handle { get; init; }

    public required SetThreadCheckerDelegate SetThreadChecker { get; init; }
    public required ThreadSuspendReloadDelegate ThreadSuspendReload { get; init; }
    public required InstallUnhandledExceptionHookDelegate InstallUnhandledExceptionHook { get; init; }
    public required PrintUnhandledExceptionDelegate PrintUnhandledException { get; init; }
    public required SetStringDelegate SetLevelString { get; init; }
    public required SetStringDelegate SetMaskString { get; init; }

    public static ClrMonoLib? TryLoad()
    {
        MelonDebug.Log("Loading CoreCLR Mono exports");

        if (string.IsNullOrEmpty(AndroidBootstrap.DotnetDir))
        {
            MelonDebug.Log("DotnetDir is not set, cannot load CoreCLR Mono exports.");
            return null;
        }

        if (!NativeLibrary.TryLoad(Path.Combine(AndroidBootstrap.DotnetDir, "shared", "Microsoft.NETCore.App", "8.0.6", "libcoreclr.so"), out var hRuntime)
            || !NativeFunc.GetExport<SetThreadCheckerDelegate>(hRuntime, "mono_melonloader_set_thread_checker", out var setThreadChecker)
            || !NativeFunc.GetExport<ThreadSuspendReloadDelegate>(hRuntime, "mono_melonloader_thread_suspend_reload", out var threadSuspendReload)
            || !NativeFunc.GetExport<InstallUnhandledExceptionHookDelegate>(hRuntime, "mono_install_unhandled_exception_hook", out var installUnhandledExceptionHook)
            || !NativeFunc.GetExport<PrintUnhandledExceptionDelegate>(hRuntime, "mono_print_unhandled_exception", out var printUnhandledException)
            || !NativeFunc.GetExport<SetStringDelegate>(hRuntime, "mono_trace_set_level_string", out var setLevelString)
            || !NativeFunc.GetExport<SetStringDelegate>(hRuntime, "mono_trace_set_mask_string", out var setMaskString))
            return null;

        return new()
        {
            Handle = hRuntime,
            SetThreadChecker = setThreadChecker,
            ThreadSuspendReload = threadSuspendReload,
            InstallUnhandledExceptionHook = installUnhandledExceptionHook,
            PrintUnhandledException = printUnhandledException,
            SetLevelString = setLevelString,
            SetMaskString = setMaskString
        };
    }

    public delegate bool CheckThreadDelegate(ulong threadId);
    public delegate void MonoUnhandledExceptionDelegate(IntPtr exc, IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SetThreadCheckerDelegate(CheckThreadDelegate checker);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ThreadSuspendReloadDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InstallUnhandledExceptionHookDelegate(MonoUnhandledExceptionDelegate callback, IntPtr userData);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PrintUnhandledExceptionDelegate(IntPtr exc);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void SetStringDelegate(string str);
}
#endif