using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.Runtime;

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

    private delegate void InitializeFn(ref nint bootstrapHandlePtr, ref nint loadLibPtr, ref nint getExportPtr);

    public static bool Initialize()
    {
        var managedDir = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "Dependencies", "net6");
        var runtimeConfigPath = Path.Combine(managedDir, "MelonLoader.runtimeconfig.json");
        var nativeHostPath = Path.Combine(managedDir, "MelonLoader.NativeHost.dll");

        if (!File.Exists(runtimeConfigPath))
        {
            Core.Logger.Error($"Runtime config not found at: '{runtimeConfigPath}'");
            return false;
        }

        if (!File.Exists(nativeHostPath))
        {
            Core.Logger.Error($"NativeHost not found at: '{runtimeConfigPath}'");
            return false;
        }

        MelonDebug.Log("Attempting to load hostfxr");
        if (!LoadHostfxr())
        {
            DotnetInstaller.AttemptInstall();
            if (!LoadHostfxr())
            {
                Core.Logger.Error("Failed to load Hostfxr");
                return false;
            }
        }

        MelonDebug.Log("Initializing domain");
        if (!InitializeForRuntimeConfig(runtimeConfigPath, out var context))
        {
            DotnetInstaller.AttemptInstall();
            if (!InitializeForRuntimeConfig(runtimeConfigPath, out context))
            {
                Core.Logger.Error($"Failed to initialize a .NET domain");
                return false;
            }
        }

        MelonDebug.Log("Loading NativeHost assembly");
        var initialize = LoadAssemblyAndGetFunctionUco<InitializeFn>(context, nativeHostPath, "MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost", "NativeEntry");
        if (initialize == null)
        {
            Core.Logger.Error($"Failed to load assembly from: '{nativeHostPath}'");
            return false;
        }

        MelonDebug.Log("Invoking NativeHost entry");

        var bootstrapHandlePtr = Core.LibraryHandle;

        NativeLoadLibFn loadLibFuncDel = NativeFunc.NativeLoadLib;
        NativeGetExportFn getExportFuncDel = NativeFunc.NativeGetExport;

        var loadLibFuncPtr = Marshal.GetFunctionPointerForDelegate(loadLibFuncDel);
        var getExportFuncPtr = Marshal.GetFunctionPointerForDelegate(getExportFuncDel);

        initialize(ref bootstrapHandlePtr, ref loadLibFuncPtr, ref getExportFuncPtr);

        return true;
    }

    public static bool LoadHostfxr()
    {
        var path = GetHostfxrPath();
        return path != null && NativeLibrary.TryLoad(path, out _);
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
