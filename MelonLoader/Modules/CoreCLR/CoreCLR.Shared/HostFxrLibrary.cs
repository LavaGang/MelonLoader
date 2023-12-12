using System;
using System.Runtime.InteropServices;
using MelonLoader.NativeUtils;

namespace MelonLoader.CoreCLR;

public class HostFxrLibrary
{
    #region Private Members

    private static HostFxrLibrary _instance;

    #endregion

    #region Public Members

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
    public delegate int d_hostfxr_get_runtime_delegate(IntPtr host_context_handle, IntPtr type, out IntPtr delegate_handle);
    public d_hostfxr_get_runtime_delegate hostfxr_get_runtime_delegate;
    
    #endregion
}