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

public unsafe struct HostImports
{
    public delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, out IntPtr, void> LoadAssemblyAndGetPtr;
    public delegate* unmanaged[Stdcall]<IntPtr, int, int> LoadAssemblyFromByteArray;
    public delegate* unmanaged[Stdcall]<int, IntPtr, int> GetTypeByName;
    public delegate* unmanaged[Stdcall]<int, int, IntPtr*, IntPtr*, int> ConstructType;
    public delegate* unmanaged[Stdcall]<int, IntPtr, int, int, IntPtr*, IntPtr*, int> InvokeMethod;

    public delegate* unmanaged[Stdcall]<StereoBool, void> Initialize;
}

public enum StereoBool : byte
{
    False = 0,
    True = 1
}
    
public struct HostExports
{
    public IntPtr _hookAttach;
    public IntPtr _hookDetach;
    public IntPtr _writeLogFile;
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
    
    public struct hostfxr_initialize_parameters
    {
        public UIntPtr size;
        public IntPtr host_path;
        public IntPtr dotnet_root;
    };
    
    #endregion
    
    #region Exports
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_hostfxr_get_runtime_delegate(IntPtr host_context_handle, hostfxr_delegate_type type, out IntPtr delegate_handle);
    public d_hostfxr_get_runtime_delegate hostfxr_get_runtime_delegate;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate int d_hostfxr_initialize_for_runtime_config(IntPtr runtime_config_path, IntPtr init_params, out IntPtr host_context_handle);
    public d_hostfxr_initialize_for_runtime_config hostfxr_initialize_for_runtime_config;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d_hostfxr_close(IntPtr host_context_handle);
    public d_hostfxr_close hostfxr_close;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int d_hostfxr_initialize_for_dotnet_command_line(int argc, IntPtr argv, hostfxr_initialize_parameters* parameters, out IntPtr host_context_handle);
    public d_hostfxr_initialize_for_dotnet_command_line hostfxr_initialize_for_dotnet_command_line;
    
    #endregion
}