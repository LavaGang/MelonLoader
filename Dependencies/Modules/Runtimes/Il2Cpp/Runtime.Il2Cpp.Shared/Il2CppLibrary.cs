using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Runtime.Il2Cpp
{
    public class Il2CppLibrary
    {
        public static Il2CppLibrary Instance { get; private set; }
        public static MelonNativeLibrary<Il2CppLibrary> Lib {  get; private set; }
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

        #region Internal Call

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void dil2cpp_add_internal_call(IntPtr signature, IntPtr funcPtr);
        public dil2cpp_add_internal_call il2cpp_add_internal_call;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr dil2cpp_resolve_icall(IntPtr signature);
        public dil2cpp_resolve_icall il2cpp_resolve_icall;

        #endregion

        #region Il2Cpp Method

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_il2cpp_method_get_name(IntPtr method);
        public d_il2cpp_method_get_name il2cpp_method_get_name;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe delegate IntPtr d_il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        public d_il2cpp_runtime_invoke il2cpp_runtime_invoke;

        #endregion
    }
}
