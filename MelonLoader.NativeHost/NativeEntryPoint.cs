using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace MelonLoader.NativeHost;

internal static class NativeEntryPoint
{
    // Prevent GC
    private static Action? startDel;

    internal static FunctionExchange Functions { get; private set; }

    [UnmanagedCallersOnly]
    private unsafe static void NativeEntry(FunctionExchange* exchange)
    {
        var currentAsm = typeof(NativeEntryPoint).Assembly;

        var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(currentAsm.Location);
        var type = asm.GetType("MelonLoader.NativeHost.NativeEntryPoint", true)!;
        var init = type.GetMethod(nameof(Initialize), BindingFlags.Static | BindingFlags.NonPublic)!;
        init.Invoke(null, [ (nint)exchange ]);
    }

    private unsafe static void Initialize(FunctionExchange* exchange)
    {
        startDel = Start;
        exchange->Start = Marshal.GetFunctionPointerForDelegate(startDel);

        Functions = *exchange;

        AssemblyLoadContext.Default.Resolving += OnResolveAssembly;
        
        //Have to invoke through a proxy so that we don't load MelonLoader.dll before the above line
        try
        {
            MelonLoaderInvoker.Initialize();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[NewEntryPoint] Caught exception invoking Initialize! " + ex);
            Thread.Sleep(5000);
            Environment.Exit(1);
        }
    }

    private static void Start()
    {
        try
        {
            MelonLoaderInvoker.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[NewEntryPoint] Caught exception invoking Start! " + ex);
            Thread.Sleep(5000);
            Environment.Exit(1);
        }
    }

    private static Assembly? OnResolveAssembly(AssemblyLoadContext alc, AssemblyName name)
    {
        var ourDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        var potentialDllPath = Path.Combine(ourDir, name.Name + ".dll");
        if (File.Exists(potentialDllPath))
            return alc.LoadFromAssemblyPath(potentialDllPath);

        return null;
    }
}