namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize()
        {
            BootstrapInterop.HookAttach = NativeEntryPoint.Exports.HookAttach;
            Core.Initialize();
        }

        internal static void PreStart() => Core.PreStart();
        internal static void Start() => Core.Start();
    }
}
