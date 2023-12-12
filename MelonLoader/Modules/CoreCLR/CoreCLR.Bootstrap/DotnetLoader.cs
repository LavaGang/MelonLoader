using System;
using System.IO;
using System.Runtime.InteropServices;
using MelonLoader.NativeUtils;
using MelonLoader.Utils;

namespace MelonLoader.CoreCLR.Bootstrap;

public static class DotnetLoader
{
    #region Private Members

    public static MelonNativeDetour<HostFxrLibrary.d_hostfxr_get_runtime_delegate> hostfxr_get_runtime_delegate_detour;

    #endregion
    
    #region Public Members

    public static DotnetRuntimeInfo RuntimeInfo { get; private set; }

    #endregion
    
    #region Bootstrap
    
    public static void Startup(DotnetRuntimeInfo runtimeInfo)
    {
        // Apply the information
        RuntimeInfo = runtimeInfo;

        // Check if it found any Mono variant library
        if (RuntimeInfo == null
            || string.IsNullOrEmpty(RuntimeInfo.LibPath))
        {
            MelonAssertion.ThrowInternalFailure($"Failed to find HostFxr Library!");
            return;
        }

        HostFxrLibrary.Instance = MelonNativeLibrary.ReflectiveLoad<HostFxrLibrary>(runtimeInfo.LibPath);
        
        MelonDebug.Msg("Checking Exports");
        
        if (!CheckExports())
            return;
        
        MelonDebug.Msg("Attaching hook to hostfxr_get_runtime_delegate");
        
        hostfxr_get_runtime_delegate_detour = new MelonNativeDetour<HostFxrLibrary.d_hostfxr_get_runtime_delegate>(HostFxrLibrary.Instance.hostfxr_get_runtime_delegate, h_hostfxr_get_runtime_delegate);
        hostfxr_get_runtime_delegate_detour.Attach();
    }
    
    private static bool CheckExports()
    {
        (string, Delegate)[] listOfExports = new (string, Delegate)[]
        {
            (nameof(HostFxrLibrary.Instance.hostfxr_get_runtime_delegate), HostFxrLibrary.Instance.hostfxr_get_runtime_delegate),
        };

        foreach (var exportPair in listOfExports)
        {
            if (exportPair.Item2 != null)
                continue;

            MelonAssertion.ThrowInternalFailure($"Failed to find {exportPair.Item1} Export in hostfxr Library!");
            return false;
        }

        return true;
    }

    
    #endregion
    
    #region Hooks
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int d_load_assembly_and_get_function_pointer(IntPtr assembly_path, IntPtr type_name, IntPtr method_name, IntPtr delegate_type_name, IntPtr reserved, out IntPtr delegate_handle);
    private static d_load_assembly_and_get_function_pointer load_fn;
    private delegate void d_init();
    private delegate void d_load_icalls(IntPtr log_function);
    
    private static unsafe int h_hostfxr_get_runtime_delegate(IntPtr host_context_handle, IntPtr type, out IntPtr delegate_handle)
    {
        int result = hostfxr_get_runtime_delegate_detour.Trampoline(host_context_handle, type, out delegate_handle);
        hostfxr_get_runtime_delegate_detour.Detach();
        
        if (delegate_handle == IntPtr.Zero)
            return result;
        
        MelonDebug.Msg("Getting load_assembly_and_get_function_pointer");
        
        load_fn = Marshal.GetDelegateForFunctionPointer<d_load_assembly_and_get_function_pointer>(delegate_handle);
        if (load_fn == null)
            return result;
        
        var runtime_dir = Path.Combine(MelonEnvironment.MelonLoaderDirectory, "net6");
        var shared_path = Path.Combine(runtime_dir, "MelonLoader.Shared.dll");
        
        if (!File.Exists(shared_path))
            return result;
        
        MelonDebug.Msg($"Loading {nameof(BootstrapInterop)}.{nameof(BootstrapInterop.LoadInternalCalls)}");
        
        var load_icalls_fn = LoadAssemblyAndGetFunctionPointer<d_load_icalls>(shared_path, $"MelonLoader.Shared, {nameof(BootstrapInterop)}", nameof(BootstrapInterop.LoadInternalCalls), "MelonLoader.Shared, BootstrapInterop.dLoadInternalCalls");
        //var init_fn = LoadAssemblyAndGetFunctionPointer<d_init>(shared_path, nameof(Core), nameof(Core.Startup));
        
        if (load_icalls_fn == null)
            return result;
        
        MelonDebug.Msg("Invoking MelonLoader.BootstrapInterop.LoadInternalCalls");
        load_icalls_fn((IntPtr)BootstrapInterop.WriteLogToFile);
        
        //MelonDebug.Msg("Invoking MelonLoader.Core.Startup");
        //init_fn();
        
        return result;
    }
    
    //write a generic method to load the assembly and get the function pointer where T is the delegate type
    private static T LoadAssemblyAndGetFunctionPointer<T>(string assembly_path, string type_name, string method_name, string delegate_type_name) where T : Delegate
    {
        IntPtr delegate_handle;
        if (load_fn == null)
        {
            MelonAssertion.ThrowInternalFailure("load_fn is null!");
            return default;
        }
        
        int res = load_fn(assembly_path.ToAnsiPointer(), type_name.ToAnsiPointer(), method_name.ToAnsiPointer(), delegate_type_name?.ToAnsiPointer() ?? IntPtr.Zero, IntPtr.Zero, out delegate_handle);

        if (res != 0)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to load {assembly_path}! Status Code: {res}");
            return default;
        }

        if (delegate_handle == IntPtr.Zero)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to get {type_name}.{method_name}!");
            return default;
        }
        
        return (T)Marshal.GetDelegateForFunctionPointer(delegate_handle, typeof(T));
    }
    
    #endregion
}