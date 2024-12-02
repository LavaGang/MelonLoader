using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace MelonLoader.NativeHost;

internal static unsafe class NativeEntryPoint
{
    // Prevent GC
    private static Action? startDel;

    // The argument should first hold the bootstrap handle, and return the start function ptr
    [UnmanagedCallersOnly]
    private static void NativeEntry(nint* startFunc)
    {
        var currentAsm = typeof(NativeEntryPoint).Assembly;

        var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(currentAsm.Location);
        var type = asm.GetType("MelonLoader.NativeHost.NativeEntryPoint", true)!;
        var init = type.GetMethod(nameof(Initialize), BindingFlags.Static | BindingFlags.NonPublic)!;
        init.Invoke(null, [ (nint)startFunc]);
    }

    private unsafe static void Initialize(nint* startFunc)
    {
        AssemblyLoadContext.Default.Resolving += OnResolveAssembly;
        
        //Have to invoke through a proxy so that we don't load MelonLoader.dll before the above line
        CallInit(startFunc);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CallInit(nint* startFunc)
    {
        var bootstrapHandle = *startFunc;

        startDel = BootstrapInterop.Start;
        *startFunc = Marshal.GetFunctionPointerForDelegate(startDel);

        BootstrapInterop.Initialize(bootstrapHandle);
    }

    private static Assembly? OnResolveAssembly(AssemblyLoadContext alc, AssemblyName name)
    {
        var ourDir = Path.GetDirectoryName(typeof(NativeEntryPoint).Assembly.Location)!;

        var potentialDllPath = Path.Combine(ourDir, name.Name + ".dll");

        return File.Exists(potentialDllPath) ? alc.LoadFromAssemblyPath(potentialDllPath) : null;
    }
}