#pragma warning disable CS0649 //Field is never assigned (native struct)

namespace MelonLoader.NativeHost
{
    internal unsafe struct HostExports
    {
        internal delegate* unmanaged<void*, void*, void*> HookAttach;
        internal delegate* unmanaged<void*, void> HookDetach;
        internal delegate* unmanaged[Stdcall]<byte*, int, void> WriteLogToFile;
    }
}