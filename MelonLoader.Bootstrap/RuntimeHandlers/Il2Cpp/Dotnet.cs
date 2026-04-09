using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal static partial class Dotnet
{
    private const CharSet hostfxrCharSet =
#if WINDOWS
        CharSet.Unicode;
#else
        CharSet.Ansi;
#endif
    private const StringMarshalling hostfxrStringMarsh =
#if WINDOWS
        StringMarshalling.Utf16;
#else
        StringMarshalling.Utf8;
#endif

    private static nint _module;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = hostfxrCharSet)]
    private delegate int hostfxr_initialize_for_runtime_config_Fn(string runtimeConfigPath, nint parameters, ref nint hostContextHandle);
    private static hostfxr_initialize_for_runtime_config_Fn? hostfxr_initialize_for_runtime_config;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int hostfxr_get_runtime_delegate_Fn(nint context, HostfxrDelegateType type, ref LoadAssemblyAndGetFunctionPointerFn? del);
    private static hostfxr_get_runtime_delegate_Fn? hostfxr_get_runtime_delegate;

    public static bool LoadHostfxr()
    {
        var path = LoaderConfig.Current.Loader.HostFXRPathOverride;

        if (string.IsNullOrEmpty(path)
            || string.IsNullOrWhiteSpace(path))
            path = GetHostfxrPath();

        if (string.IsNullOrEmpty(path)
            || string.IsNullOrWhiteSpace(path))
            return false;

        MelonDebug.Log($"HostFXR Path: {path}");

        if (!NativeLibrary.TryLoad(path, out _module))
            return false;

        nint hostfxr_initialize_for_runtime_config_ptr = NativeLibrary.GetExport(_module, nameof(hostfxr_initialize_for_runtime_config));
        if (hostfxr_initialize_for_runtime_config_ptr == nint.Zero)
            return false;
        hostfxr_initialize_for_runtime_config = Marshal.GetDelegateForFunctionPointer<hostfxr_initialize_for_runtime_config_Fn>(hostfxr_initialize_for_runtime_config_ptr);

        nint hostfxr_get_runtime_delegate_ptr = NativeLibrary.GetExport(_module, nameof(hostfxr_get_runtime_delegate));
        if (hostfxr_get_runtime_delegate_ptr == nint.Zero)
            return false;
        hostfxr_get_runtime_delegate = Marshal.GetDelegateForFunctionPointer<hostfxr_get_runtime_delegate_Fn>(hostfxr_get_runtime_delegate_ptr);

        return true;
    }

    private static string? GetHostfxrPath()
    {
        var buffer = new StringBuilder(1024);
        var bufferSize = (nint)buffer.Capacity;
        var result = get_hostfxr_path(buffer, ref bufferSize, 0);
        return result != 0 ? null : buffer.ToString();
    }

    public static bool InitializeForRuntimeConfig(string runtimeConfigPath, out nint context)
    {
        if (hostfxr_initialize_for_runtime_config == null)
        {
            context = 0;
            return false;
        }

        nint ctx = 0;
        ConsoleHandler.NullHandles(); // Prevent it from logging its own stuff
        var status = hostfxr_initialize_for_runtime_config(runtimeConfigPath, 0, ref ctx);
        ConsoleHandler.ResetHandles();

        if (status != 0)
        {
            context = status;
            return false;
        }

        context = ctx;
        return true;
    }

    public static TDelegate? LoadAssemblyAndGetFunctionUco<TDelegate>(nint context, string assemblyPath, string typeName, string methodName) where TDelegate : Delegate
    {
        if (hostfxr_get_runtime_delegate == null)
            return null;

        LoadAssemblyAndGetFunctionPointerFn? loadAssemblyAndGetFunctionPointer = null;
        hostfxr_get_runtime_delegate(context, HostfxrDelegateType.HdtLoadAssemblyAndGetFunctionPointer, ref loadAssemblyAndGetFunctionPointer);
        if (loadAssemblyAndGetFunctionPointer == null)
            return null;

        nint funcPtr = 0;
        loadAssemblyAndGetFunctionPointer(assemblyPath, typeName, methodName, -1, 0, ref funcPtr);
        if (funcPtr == 0)
            return null;

        return Marshal.GetDelegateForFunctionPointer<TDelegate>(funcPtr);
    }

    [DllImport("*", CharSet = hostfxrCharSet)]
    private static extern int get_hostfxr_path(StringBuilder buffer, ref nint bufferSize, nint parameters);

    private enum HostfxrDelegateType
    {
        HdtComActivation,
        HdtLoadInMemoryAssembly,
        HdtWinrtActivation,
        HdtComRegister,
        HdtComUnregister,
        HdtLoadAssemblyAndGetFunctionPointer,
        HdtGetFunctionPointer,
        HdtLoadAssembly,
        HdtLoadAssemblyBytes,
    };

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = hostfxrCharSet)]
    private delegate void LoadAssemblyAndGetFunctionPointerFn(string assemblyPath, string typeName, string methodName, nint delegateTypeName, nint reserved, ref nint funcPtr);
}
