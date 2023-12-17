namespace MelonLoader.NativeHost
{
    unsafe struct HostImports
    {
        public delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void**, void> LoadAssemblyAndGetPtr;
        public delegate* unmanaged[Stdcall]<IntPtr, int, int> LoadAssemblyFromByteArray;
        public delegate* unmanaged[Stdcall]<int, IntPtr, int> GetTypeByName;
        public delegate* unmanaged[Stdcall]<int, int, IntPtr*, IntPtr*, int> ConstructType;
        public delegate* unmanaged[Stdcall]<int, IntPtr, int, int, IntPtr*, IntPtr*, int> InvokeMethod;

        public delegate* unmanaged[Stdcall]<Stereo.StereoBool, void> Initialize;
    }
}