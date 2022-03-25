namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize()
        {
            BootstrapInterop.HookAttach = NativeEntryPoint.Exports.HookAttach;
            BootstrapInterop.HookDetach = NativeEntryPoint.Exports.HookDetach;

            Core.Initialize();
        }

        internal static void PreStart() => Core.PreStart();
        internal static void Start() => Core.Start();
    }
}
