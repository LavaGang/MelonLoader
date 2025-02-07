using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Runtime.Il2Cpp
{
    public class Il2CppLibrary
    {
        #region Public Members

        public static Il2CppLibrary Instance { get; private set; }
        public static MelonNativeLibrary<Il2CppLibrary> Lib {  get; private set; }

        #endregion

        #region Public Methods

        public static void Load(string filePath)
        {
            if (Instance != null)
                return;

            IntPtr libPtr = MelonNativeLibrary.LoadLib(filePath);
            if (libPtr == IntPtr.Zero)
                throw new Exception($"Failed to load {filePath}");

            Lib = new(libPtr, false);
            Instance = Lib.Instance;
        }

        #endregion

        #region Il2Cpp Domain

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_il2cpp_init_version(IntPtr name, IntPtr version);
        public d_il2cpp_init_version il2cpp_init_version { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_il2cpp_domain_get();
        public d_il2cpp_domain_get il2cpp_domain_get { get; private set; }

        #endregion

        #region Il2Cpp Internal Calls

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void dil2cpp_add_internal_call(IntPtr signature, IntPtr funcPtr);
        public dil2cpp_add_internal_call il2cpp_add_internal_call { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate IntPtr dil2cpp_resolve_icall(IntPtr signature);
        public dil2cpp_resolve_icall il2cpp_resolve_icall { get; private set; }

        #endregion

        #region Il2Cpp Method

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_il2cpp_method_get_name(IntPtr method);
        public d_il2cpp_method_get_name il2cpp_method_get_name { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe delegate IntPtr d_il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        public d_il2cpp_runtime_invoke il2cpp_runtime_invoke { get; private set; }

        #endregion
    }
}
