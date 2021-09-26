using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal sealed class GL
    {
        private static readonly IntPtr m_get_sRGBWrite;

        static GL()
        {
            Il2CppClassPointerStore<GL>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "GL");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<GL>.NativeClassPtr);

            m_get_sRGBWrite = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<GL>.NativeClassPtr, "get_sRGBWrite", "System.Boolean");
        }

        public unsafe static bool sRGBWrite
        {
            get
            {
                IntPtr returnedException = default;
                IntPtr obj = IL2CPP.il2cpp_runtime_invoke(m_get_sRGBWrite, IntPtr.Zero, (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return *(bool*)IL2CPP.il2cpp_object_unbox(obj);
            }
            // set
        }
    }
}
