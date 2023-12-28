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
        static unsafe void LoadStage1(HostImports* imports)
        {
            Console.WriteLine("[NewEntryPoint] Passing ptr to LoadAssemblyAndGetFuncPtr back to host...");
            
            //hack because [UnmanagedCallersOnly] doesn't support out parameters
            delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void**, void> ptr = &Stereo.LoadAssemblyAndGetFuncPtr;
            imports->LoadAssemblyAndGetPtr = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, out IntPtr, void>)ptr;
            imports->LoadAssemblyFromByteArray = &Stereo.LoadAssemblyFromByteArray;
            imports->GetTypeByName = &Stereo.GetTypeByName;
            imports->ConstructType = &Stereo.ConstructType;
            imports->InvokeMethod = &Stereo.InvokeMethod;
        }

        [UnmanagedCallersOnly]
        static unsafe void LoadStage2(HostImports* imports, HostExports* exports)
        {
            Console.WriteLine("[NewEntryPoint] Configuring imports...");
            imports->Initialize = &Initialize;
            Exports = *exports;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        static void Initialize(Stereo.StereoBool firstRunBool)
        {
            bool firstRun = firstRunBool == Stereo.StereoBool.True;
            bool isDefaultAlc = AssemblyLoadContext.Default == AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
            Console.WriteLine($"[NewEntryPoint] Initializing. In default load context: {isDefaultAlc}");

            AssemblyLoadContext.Default.Resolving += OnResolveAssembly;

            //Have to invoke through a proxy so that we don't load MelonLoader.dll before the above line
            try
            {
                MelonLoaderInvoker.Initialize(firstRun);
            } catch(Exception ex)
            {
                Console.WriteLine("[NewEntryPoint] Caught exception invoking Initialize! " + ex);
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
        }

        public static Assembly? TryLoadAssembly(AssemblyLoadContext alc, string path)
        {
            if (File.Exists(path))
                return alc.LoadFromAssemblyPath(path);
            return null;
        }

        private static Assembly? OnResolveAssembly(AssemblyLoadContext alc, AssemblyName name)
        {
            // Get Assembly File Name
            var filename = name.Name + ".dll";

            // Check Runtime Directory
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, filename);
            return TryLoadAssembly(alc, filePath);
        }
    }
}