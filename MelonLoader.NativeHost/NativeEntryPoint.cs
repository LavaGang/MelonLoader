using System.Runtime.InteropServices;

namespace MelonLoader.NativeHost
{
    public static class NativeEntryPoint
    {
        [UnmanagedCallersOnly]
        static void Initialize()
        {
            Core.Initialize();
        }
    }
}