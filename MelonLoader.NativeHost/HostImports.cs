namespace MelonLoader.NativeHost
{
    unsafe struct HostImports
    {
        public delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void**, void> LoadAssemblyAndGetPtr;

        public delegate* unmanaged[Stdcall]<IntPtr, int, int> LoadAssemblyFromBytes;
        public delegate* unmanaged[Stdcall]<int, IntPtr, int> GetTypeFromAssembly;
        public delegate* unmanaged[Stdcall]<int, int, IntPtr*, IntPtr*, int> ConstructType;
        public delegate* unmanaged[Stdcall]<int, IntPtr, int, int, IntPtr*, IntPtr*, int> InvokeMethod;
        public delegate* unmanaged[Stdcall]<int, IntPtr, int, IntPtr*, nuint> GetPointerToUcoMethod;

        public delegate* unmanaged[Stdcall]<void> Initialize;
        public delegate* unmanaged[Stdcall]<void> PreStart;
        public delegate* unmanaged[Stdcall]<void> Start;
    }
}
