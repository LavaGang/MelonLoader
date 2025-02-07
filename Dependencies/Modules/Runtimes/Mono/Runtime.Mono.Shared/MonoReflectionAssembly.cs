using System.Runtime.InteropServices;

namespace MelonLoader.Runtime.Mono
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MonoReflectionAssembly
    {
        // MonoObject
        public nint VTable;
        public nint Sync;

        public nint Assembly;

        public nint Evidence;
    }
}
