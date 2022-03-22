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

        [UnmanagedCallersOnly]
        static void PreStart()
        {
            Core.PreStart();
        }

        [UnmanagedCallersOnly]
        static void Start()
        {
            Core.Start();
        }
    }
}