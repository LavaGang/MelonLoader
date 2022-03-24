namespace MelonLoader.NativeHost
{
    unsafe struct HostExports
    {
        internal delegate* unmanaged<void**, void*, void> HookAttach;
    }
}
