using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Runtime.Mono
{
    public class MonoLibrary
    {
        public static bool IsBleedingEdge { get; private set; } = true;

        public static MonoLibrary Instance { get; private set; }
        public static MelonNativeLibrary<MonoLibrary> Lib { get; private set; }
        public static void Load(string filePath)
        {
            if (Instance != null)
                return;

            IntPtr libPtr = MelonNativeLibrary.LoadLib(filePath);
            if (libPtr == IntPtr.Zero)
                throw new Exception($"Failed to load {filePath}");

            Lib = new(libPtr);
            Instance = Lib.Instance;
        }

        #region Mono Method

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_method_get_name(IntPtr method);
        public d_mono_method_get_name mono_method_get_name;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe delegate IntPtr d_mono_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        public d_mono_runtime_invoke mono_runtime_invoke;

        #endregion
    }
}
