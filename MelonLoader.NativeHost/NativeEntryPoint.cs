using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using MelonLoader.InternalUtils;
using MelonLoader.Modules;

namespace MelonLoader.NativeHost;

internal static unsafe class NativeEntryPoint
{
    // The argument should first hold the bootstrap handle, and return the start function ptr
    [UnmanagedCallersOnly]
    private static void NativeEntry(nint* bootstrapHandlePtr, nint* loadLibFuncPtr, nint* getExportFuncPtr)
    {
        var currentAsm = typeof(NativeEntryPoint).Assembly;

        var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(currentAsm.Location);
        var type = asm.GetType("MelonLoader.NativeHost.NativeEntryPoint", true)!;
        var init = type.GetMethod(nameof(Initialize), BindingFlags.Static | BindingFlags.NonPublic)!;
        init.Invoke(null, [(nint)bootstrapHandlePtr, (nint)loadLibFuncPtr, (nint)getExportFuncPtr]);
    }

    private unsafe static void Initialize(nint* bootstrapHandlePtr, nint* loadLibFuncPtr, nint* getExportFuncPtr)
    {
        AssemblyLoadContext.Default.Resolving += OnResolveAssembly;
        CallInit(bootstrapHandlePtr, loadLibFuncPtr, getExportFuncPtr);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CallInit(nint* bootstrapHandlePtr, nint* loadLibFuncPtr, nint* getExportFuncPtr)
    {
        var bootstrapHandle = *bootstrapHandlePtr;
        var loadLibFunc = *loadLibFuncPtr;
        var getExportFunc = *getExportFuncPtr;

        BootstrapInterop.Stage1(bootstrapHandle, loadLibFunc, getExportFunc, true);
        ModuleInterop.StartEngine();
    }

    private static Assembly? OnResolveAssembly(AssemblyLoadContext alc, AssemblyName name)
    {
        var ourDir = Path.GetDirectoryName(typeof(NativeEntryPoint).Assembly.Location)!;
        var potentialDllPath = Path.Combine(ourDir, name.Name + ".dll");
        return File.Exists(potentialDllPath) ? alc.LoadFromAssemblyPath(potentialDllPath) : null;
    }
}