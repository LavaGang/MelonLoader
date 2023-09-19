namespace MelonLoader.NativeHost
{
    unsafe struct HostImports
    {
        public delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void**, void> LoadAssemblyAndGetPtr;

        public delegate* unmanaged[Stdcall]<void> Initialize;
    }
}