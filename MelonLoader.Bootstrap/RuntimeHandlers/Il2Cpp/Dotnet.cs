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

    private const string ANDROID_DOTNET_VERSION = "8.0.6";

    public static bool LoadHostfxr()
    {
        var path = GetHostfxrPath();
        return path != null && NativeLibrary.TryLoad(path, out _);
    }

    private static string? GetHostfxrPath()
    {
#if ANDROID
        if (string.IsNullOrEmpty(MelonLoader.Bootstrap.Proxy.Android.AndroidBootstrap.DotnetDir))
        {
            MelonDebug.Log("DotnetDir is not set, cannot find hostfxr.");
            return null;
        }

        return Path.Combine(MelonLoader.Bootstrap.Proxy.Android.AndroidBootstrap.DotnetDir, "host", "fxr", ANDROID_DOTNET_VERSION, "libhostfxr.so");
#else
        var buffer = new StringBuilder(1024);
        var bufferSize = (nint)buffer.Capacity;
        var result = get_hostfxr_path(buffer, ref bufferSize, 0);
        return result != 0 ? null : buffer.ToString();
#endif
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

    public static TDelegate? LoadAssemblyAndGetFunctionUco<TDelegate>(nint context, string assemblyPath, string typeName, string methodName) where TDelegate : Delegate
    {
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

    [LibraryImport("hostfxr", StringMarshalling = hostfxrStringMarsh)]
    private static partial int hostfxr_initialize_for_runtime_config(string runtimeConfigPath, nint parameters, ref nint hostContextHandle);

    [LibraryImport("hostfxr")]
    private static partial int hostfxr_get_runtime_delegate(nint context, HostfxrDelegateType type, ref LoadAssemblyAndGetFunctionPointerFn? del);

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
