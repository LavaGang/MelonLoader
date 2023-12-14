namespace MelonLoader.NativeHost
{
    unsafe struct HostImports
    {
        public delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void**, void> LoadAssemblyAndGetPtr;

        public delegate* unmanaged[Stdcall]<Stereo.StereoBool, void> Initialize;
    }
}