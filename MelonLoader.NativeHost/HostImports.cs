namespace MelonLoader.NativeHost
{
    unsafe struct HostImports
    {
        public delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void**, void> LoadAssemblyAndGetPtr;

        public delegate* unmanaged[Stdcall]<void> Initialize;
        public delegate* unmanaged[Stdcall]<void> PreStart;
        public delegate* unmanaged[Stdcall]<void> Start;
    }
}
