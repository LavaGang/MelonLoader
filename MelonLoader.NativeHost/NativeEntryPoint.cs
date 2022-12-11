using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace MelonLoader.NativeHost
{
    public static class NativeEntryPoint
    {
        internal static HostExports Exports;

        [UnmanagedCallersOnly]
        unsafe static void LoadStage1(HostImports* imports)
        {
            Console.WriteLine("[NewEntryPoint] Passing ptr to LoadAssemblyAndGetFuncPtr back to host...");
            imports->LoadAssemblyAndGetPtr = &StereoHostingApi.LoadAssemblyAndGetFuncPtr;
        }


        [UnmanagedCallersOnly]
        unsafe static void LoadStage2(HostImports* imports, HostExports* exports)
        {
            Console.WriteLine("[NewEntryPoint] Configuring imports...");

            imports->Initialize = &Initialize;
            imports->PreStart = &PreStart;
            imports->Start = &Start;

            Exports = *exports;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        static void Initialize()
        {
            bool isDefaultAlc = AssemblyLoadContext.Default == AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
            Console.WriteLine($"[NewEntryPoint] Initializing. In default load context: {isDefaultAlc}");

            AssemblyLoadContext.Default.Resolving += OnResolveAssembly;

            //Have to invoke through a proxy so that we don't load MelonLoader.dll before the above line
            try
            {
                MelonLoaderInvoker.Initialize();
            } catch(Exception ex)
            {
                Console.WriteLine("[NewEntryPoint] Caught exception invoking Initialize! " + ex);
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvStdcall)})]
        static void PreStart()
        {
            Console.WriteLine("[NewEntryPoint] PreStarting.");

            try
            {
                MelonLoaderInvoker.PreStart();
            } catch(Exception ex)
            {
                Console.WriteLine("[NewEntryPoint] Caught exception invoking PreStart! " + ex);
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        static void Start()
        {
            Console.WriteLine("[NewEntryPoint] Starting.");

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
}