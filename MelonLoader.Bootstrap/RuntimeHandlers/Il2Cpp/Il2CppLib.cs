using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal class Il2CppLib(Il2CppLib.MethodGetNameFn methodGetName)
{
    private const string libName = // Gotta specify the file extension in lower-case, otherwise Il2CppInterop brainfarts itself
#if WINDOWS
        "GameAssembly.dll";
#elif LINUX
        "GameAssembly.so";
#endif

    public required nint Handle { get; init; }

    public required nint InitPtr { get; init; }
    public required nint RuntimeInvokePtr { get; init; }

    public static Il2CppLib? TryLoad()
    {
        if (!NativeLibrary.TryLoad(libName, out var hRuntime)
            || !NativeLibrary.TryGetExport(hRuntime, "il2cpp_init", out var initPtr)
            || !NativeLibrary.TryGetExport(hRuntime, "il2cpp_runtime_invoke", out var runtimeInvokePtr)
            || !NativeFunc.GetExport<MethodGetNameFn>(hRuntime, "il2cpp_method_get_name", out var methodGetName))
            return null;

        return new(methodGetName)
        {
            Handle = hRuntime,
            InitPtr = initPtr,
            RuntimeInvokePtr = runtimeInvokePtr
        };
    }

    public string? GetMethodName(nint method)
    {
        return method == 0 ? null : Marshal.PtrToStringAnsi(methodGetName(method));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint InitFn(nint a);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint RuntimeInvokeFn(nint method, nint obj, nint args, nint exc);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint MethodGetNameFn(nint method);
}
