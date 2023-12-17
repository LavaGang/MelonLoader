using System;
using System.IO;
using System.Runtime.InteropServices;
using MelonLoader.NativeUtils;
using MelonLoader.Utils;

namespace MelonLoader.CoreCLR.Bootstrap;

public static class DotnetLoader
{
    #region Private Members

    private static MelonNativeDetour<HostFxrLibrary.d_hostfxr_get_runtime_delegate> _hostfxrGetRuntimeDelegateDetour;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int DLoadAssemblyAndGetFunctionPointer(IntPtr assemblyPath, IntPtr typeName, IntPtr methodName, IntPtr delegateTypeName, IntPtr reserved, out IntPtr delegateHandle);
    private static DLoadAssemblyAndGetFunctionPointer _loadAssemblyAndGetFunctionPointer;
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void DLoadStage1(HostImports* imports);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void DLoadStage2(HostImports* imports, HostExports* exports);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void DInitialize(byte firstRun);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void DStartup();


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
        
        _hostfxrGetRuntimeDelegateDetour = new MelonNativeDetour<HostFxrLibrary.d_hostfxr_get_runtime_delegate>(HostFxrLibrary.Instance.hostfxr_get_runtime_delegate, h_hostfxr_get_runtime_delegate);
    }
    
    private static bool CheckExports()
    {
        var listOfExports = new (string, Delegate)[]
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

    private static unsafe void ReloadIntoDefaultAlc()
    {
        var runtimeDir = Path.Combine(MelonEnvironment.MelonLoaderDirectory, "net6");
        var bootstrapPath = Path.Combine(runtimeDir, "MelonLoader.NativeHost.dll");
        var sharedPath = Path.Combine(runtimeDir, "MelonLoader.Shared.dll");
        var typeName = "MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost";
        var methodName = "LoadStage1";
        
        if (!File.Exists(bootstrapPath))
        {
            MelonAssertion.ThrowInternalFailure($"Failed to find {bootstrapPath}!");
            return;
        }
        
        MelonDebug.Msg("[Dotnet] Invoking LoadStage1");
        var init = LoadAssemblyAndGetFunctionPointer<DLoadStage1>(bootstrapPath, typeName, methodName, HostFxrLibrary.UNMANAGEDCALLERSONLY_METHOD);
        if (init == null)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to get MelonLoader.NativeHost.NativeEntryPoint.LoadStage1!");
            return;
        }
        
        var imports = new HostImports();
        init(&imports);
        
        MelonDebug.Msg("[Dotnet] Reloading NativeHost into correct load context and getting LoadStage2 pointer");
        
        imports.LoadAssemblyAndGetPtr(bootstrapPath.ToUnicodePointer(), typeName.ToUnicodePointer(), "LoadStage2".ToUnicodePointer(), out var loadStage2);
        
        if (loadStage2 == IntPtr.Zero)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to get LoadStage2!");
            return;
        }
        
        var exports = new HostExports
        {
            _writeLogFile = (IntPtr)BootstrapInterop.WriteLogToFile,
            _hookAttach = (IntPtr)BootstrapInterop.HookDetach,
            _hookDetach = (IntPtr)BootstrapInterop.HookDetach
        };
        
        MelonDebug.Msg("[Dotnet] Invoking LoadStage2");
        var loadStage2Delegate = Marshal.GetDelegateForFunctionPointer<DLoadStage2>(loadStage2);
        loadStage2Delegate(&imports, &exports);
        
        MelonDebug.Msg("[Dotnet] Invoking Initialize");
        imports.Initialize(StereoBool.False);

        var asmBuffer = File.ReadAllBytes(sharedPath);
        int asmHandle = 0;
        fixed (byte* p = asmBuffer)
        {
            asmHandle = imports.LoadAssemblyFromByteArray((IntPtr)p, asmBuffer.Length);
        }

        if (asmHandle < 0)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to load {sharedPath}! Status Code: {asmHandle}");
        }
        
        var coreTypeHandle = imports.GetTypeByName(asmHandle, "MelonLoader.Core".ToUnicodePointer());
        
        MelonDebug.Msg(coreTypeHandle);
        
        if (coreTypeHandle < 0)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to get MelonLoader.Core! Status Code: {coreTypeHandle}");
            return;
        }
        
        MelonDebug.Msg("Invoking MelonLoader.Core.Startup");
        var ret = imports.InvokeMethod(coreTypeHandle, "Startup".ToUnicodePointer(), -1, 0, null, null);
        
        if (ret != 0)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to invoke MelonLoader.Core.Startup! Status Code: {ret}");
            return;
        }
        
        MelonDebug.Msg("Invoking MelonLoader.Core.OnApplicationPreStart");
        ret = imports.InvokeMethod(coreTypeHandle, "OnApplicationPreStart".ToUnicodePointer(), -1, 0, null, null);
        
        if (ret != 0)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to invoke MelonLoader.Core.OnApplicationPreStart! Status Code: {ret}");
            return;
        }
        
        MelonDebug.Msg("Invoking MelonLoader.Core.OnApplicationStart");
        ret = imports.InvokeMethod(coreTypeHandle, "OnApplicationStart".ToUnicodePointer(), -1, 0, null, null);
        
        if (ret != 0)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to invoke MelonLoader.Core.OnApplicationStart! Status Code: {ret}");
            return;
        }
    }
    
    #endregion
    
    #region Hooks

    private static unsafe int h_hostfxr_get_runtime_delegate(IntPtr hostContextHandle, hostfxr_delegate_type type, out IntPtr delegateHandle)
    {
        int result = _hostfxrGetRuntimeDelegateDetour.Trampoline(hostContextHandle, type, out delegateHandle);
        _hostfxrGetRuntimeDelegateDetour.Detach();
        
        if (result != 0)
            return result;
        
        if (delegateHandle == IntPtr.Zero)
            return result;
        
        MelonDebug.Msg("Getting load_assembly_and_get_function_pointer");
        
        _loadAssemblyAndGetFunctionPointer = Marshal.GetDelegateForFunctionPointer<DLoadAssemblyAndGetFunctionPointer>(delegateHandle);
        if (_loadAssemblyAndGetFunctionPointer == null)
            return result;
        
        ReloadIntoDefaultAlc();
        
        return result;
    }
    
    private static T LoadAssemblyAndGetFunctionPointer<T>(string assemblyPath, string typeName, string methodName, IntPtr delegateTypeName) where T : Delegate
    {
        if (_loadAssemblyAndGetFunctionPointer == null)
        {
            MelonAssertion.ThrowInternalFailure("load_fn is null!");
            return default;
        }
        
        int res = _loadAssemblyAndGetFunctionPointer(assemblyPath.ToAutoPointer(), typeName.ToAutoPointer(), methodName.ToAutoPointer(), delegateTypeName, IntPtr.Zero, out var delegateHandle);
        if (res != 0)
        {
            MelonAssertion.ThrowInternalFailure($"Failed to load {assemblyPath}! Status Code: 0x{(res):X}");
            return default;
        }

        if (Marshal.ReadIntPtr(delegateHandle) != IntPtr.Zero)
            return (T)Marshal.GetDelegateForFunctionPointer(delegateHandle, typeof(T));
        
        
        MelonAssertion.ThrowInternalFailure($"Failed to get {typeName}.{methodName}!");
        return default;

    }
    
    #endregion
}