using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader.NativeHost
{
    public static class NativeEntryPoint
    {
        private static MelonLogger.Instance loggerInstance = new MelonLogger.Instance("NewEntryPoint");

        [UnmanagedCallersOnly]
        unsafe static void Initialize(HostImports* imports)
        {
            loggerInstance.Msg("Initializing. Imports: " + (IntPtr) imports);
            imports->PreStart = &PreStart;
            imports->Start = &Start;
            Core.Initialize();
        }

        [UnmanagedCallersOnly(CallConvs = new[] {typeof(CallConvStdcall)})]
        static void PreStart()
        {
            loggerInstance.Msg("PreStarting.");

            try
            {
                Core.PreStart();
            } catch(Exception ex)
            {
                loggerInstance.Error("Caught exception invoking PreStart!", ex);
                Thread.Sleep(2000);
                Environment.Exit(1);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        static void Start()
        {
            loggerInstance.Msg("Starting.");

            try
            {
                Core.Start();
            }
            catch (Exception ex)
            {
                loggerInstance.Error("Caught exception invoking Start!", ex);
                Thread.Sleep(2000);
                Environment.Exit(1);
            }
        }
    }
}