namespace MelonLoader.NativeHost
{
    unsafe struct HostExports
    {
        internal delegate* unmanaged<void**, void*, void> HookAttach;
        internal delegate* unmanaged<void**, void*, void> HookDetach;
    }
}
