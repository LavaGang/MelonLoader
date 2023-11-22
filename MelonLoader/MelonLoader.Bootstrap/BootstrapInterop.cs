namespace MelonLoader.Bootstrap
{
    public static unsafe class BootstrapInterop
    {
        public static delegate* unmanaged<void*, void*, void*> NativeHookAttach;
        public static delegate* unmanaged<void*, void> NativeHookDetach;
    }
}