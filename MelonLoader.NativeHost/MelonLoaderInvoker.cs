using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader.NativeHost
{
    internal class MelonLoaderInvoker
    {
        internal static unsafe void Initialize()
        {
            BootstrapInterop.HookAttach = Marshal.GetDelegateForFunctionPointer<BootstrapInterop.NativeHookFn>(NativeEntryPoint.Functions.HookAttach);
            BootstrapInterop.HookDetach = Marshal.GetDelegateForFunctionPointer<BootstrapInterop.NativeHookFn>(NativeEntryPoint.Functions.HookDetach);

            Core.Initialize();
        }

        internal static void Start() => Core.Start();
    }
}
