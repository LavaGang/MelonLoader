namespace MelonLoader.Bootstrap
{
    public static unsafe class BootstrapInterop
    {
        public static delegate* unmanaged<void*, void*, void*> HookAttach;
        public static delegate* unmanaged<void*, void> HookDetach;
    }
}