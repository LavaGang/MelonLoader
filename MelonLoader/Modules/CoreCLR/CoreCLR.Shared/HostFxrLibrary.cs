using System;
using System.Runtime.InteropServices;
using MelonLoader.NativeUtils;

namespace MelonLoader.CoreCLR;

public enum hostfxr_delegate_type
{
    hdt_com_activation,
    hdt_load_in_memory_assembly,
    hdt_winrt_activation,
    hdt_com_register,
    hdt_com_unregister,
    hdt_load_assembly_and_get_function_pointer,
    hdt_get_function_pointer,
    hdt_load_assembly,
    hdt_load_assembly_bytes,
}

public class HostFxrLibrary
{
    #region Private Members

    private static HostFxrLibrary _instance;

    #endregion

    #region Public Members

    public static readonly IntPtr UNMANAGEDCALLERSONLY_METHOD = (IntPtr)(-1);

    public static HostFxrLibrary Instance
    {
        get => _instance;
        set
        {
            if (_instance != null)
                return;
            _instance = value;
        }
    }
    
    #endregion
    
    #region Exports
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_hostfxr_get_runtime_delegate(IntPtr host_context_handle, hostfxr_delegate_type type, out IntPtr delegate_handle);
    public d_hostfxr_get_runtime_delegate hostfxr_get_runtime_delegate;
    
    #endregion
}