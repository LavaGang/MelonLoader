using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Runtime.Il2Cpp
{
    public class Il2CppLibrary
    {
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
