using MelonBootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonBootstrap.RuntimeHandlers.Il2Cpp;

internal class Il2CppLib(Il2CppLib.MethodGetNameFn methodGetName)
{
    private const string libName = "GameAssembly.dll"; // Gotta specify the file extension in lower-case, otherwise Il2CppInterop brainfarts itself

    public required nint Handle { get; init; }

    public required nint InitPtr { get; init; }
    public required nint RuntimeInvokePtr { get; init; }

    public static Il2CppLib? TryLoad()
    {
        if (!NativeLibrary.TryLoad(libName, out var hRuntime))
            return null;

        if (!NativeLibrary.TryGetExport(hRuntime, "il2cpp_init", out var initPtr))
            return null;

        if (!NativeLibrary.TryGetExport(hRuntime, "il2cpp_runtime_invoke", out var runtimeInvokePtr))
            return null;

        if (!NativeFunc.GetExport<MethodGetNameFn>(hRuntime, "il2cpp_method_get_name", out var methodGetName))
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
        if (method == 0)
            return null;

        return Marshal.PtrToStringAnsi(methodGetName(method));
    }

    public delegate nint InitFn(nint a);
    public delegate nint RuntimeInvokeFn(nint method, nint obj, nint args, nint exc);
    public delegate nint MethodGetNameFn(nint method);
}
