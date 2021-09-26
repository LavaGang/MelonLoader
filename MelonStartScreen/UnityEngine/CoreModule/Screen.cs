using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Screen
    {
        private static IntPtr m_get_width;
        private static IntPtr m_get_height;

        static Screen()
        {
            Il2CppClassPointerStore<Screen>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Screen");
            m_get_width = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Screen>.NativeClassPtr, "get_width", "System.Int32");
            m_get_height = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Screen>.NativeClassPtr, "get_height", "System.Int32");
        }

        public unsafe static int width
        {
            get
            {
                IntPtr* param = null;
                IntPtr returnedException = IntPtr.Zero;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_width, IntPtr.Zero, (void**)param, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return *(int*)IL2CPP.il2cpp_object_unbox(intPtr);
            }
        }

        public unsafe static int height
        {
            get
            {
                IntPtr* param = null;
                IntPtr returnedException = IntPtr.Zero;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_height, IntPtr.Zero, (void**)param, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return *(int*)IL2CPP.il2cpp_object_unbox(intPtr);
            }
        }
    }
}
