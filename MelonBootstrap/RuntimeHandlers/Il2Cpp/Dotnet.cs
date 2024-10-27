using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal static partial class Dotnet
{
    public static bool LoadHostfxr()
    {
        var path = GetHostfxrPath();
        if (path == null)
            return false;

        return NativeLibrary.TryLoad(path, out _);
    }

    private static string? GetHostfxrPath()
    {
        var buffer = new StringBuilder(1024);
        var bufferSize = (nint)buffer.Capacity;
        var result = get_hostfxr_path(buffer, ref bufferSize, 0);
        if (result != 0)
            return null;

        return buffer.ToString();
    }

    public static bool InitializeForRuntimeConfig(string runtimeConfigPath, out nint context)
    {
        nint ctx = 0;
        ConsoleHandler.NullHandles(); // Prevent it from logging its own stuff
        var status = hostfxr_initialize_for_runtime_config(runtimeConfigPath, 0, ref ctx);
        ConsoleHandler.ResetHandles();

        if (status != 0)
        {
            context = 0;
            return false;
        }

        context = ctx;
        return true;
    }

    public static TDelegate? LoadAssemblyAndGetFunctionUCO<TDelegate>(nint context, string assemblyPath, string typeName, string methodName) where TDelegate : Delegate
    {
        load_assembly_and_get_function_pointer_fn? loadAssemblyAndGetFunctionPointer = null;
        hostfxr_get_runtime_delegate(context, hostfxr_delegate_type.hdt_load_assembly_and_get_function_pointer, ref loadAssemblyAndGetFunctionPointer);
        if (loadAssemblyAndGetFunctionPointer == null)
            return null;

        nint funcPtr = 0;
        loadAssemblyAndGetFunctionPointer(assemblyPath, typeName, methodName, -1, 0, ref funcPtr);
        if (funcPtr == 0)
            return null;

        return Marshal.GetDelegateForFunctionPointer<TDelegate>(funcPtr);
    }

    [DllImport("*", EntryPoint = "get_hostfxr_path", CharSet = CharSet.Unicode)]
    private static extern int get_hostfxr_path(StringBuilder buffer, ref nint bufferSize, nint parameters);

    [LibraryImport("hostfxr", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int hostfxr_initialize_for_runtime_config(string runtimeConfigPath, nint parameters, ref nint hostContextHandle);

    [LibraryImport("hostfxr")]
    private static partial int hostfxr_get_runtime_delegate(nint context, hostfxr_delegate_type type, ref load_assembly_and_get_function_pointer_fn? del);

    private enum hostfxr_delegate_type
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
    };

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    private delegate void load_assembly_and_get_function_pointer_fn(string assemblyPath, string typeName, string methodName, nint delegateTypeName, nint reserved, ref nint funcPtr);
}
