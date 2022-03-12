using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class Screen
    {
        private static IntPtr m_get_width;
        private static IntPtr m_get_height;

        static Screen()
        {
            InternalClassPointerStore<Screen>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Screen");
            m_get_width = UnityInternals.GetMethod(InternalClassPointerStore<Screen>.NativeClassPtr, "get_width", "System.Int32");
            m_get_height = UnityInternals.GetMethod(InternalClassPointerStore<Screen>.NativeClassPtr, "get_height", "System.Int32");
        }

        public unsafe static int width
        {
            get
            {
                IntPtr* param = null;
                IntPtr returnedException = IntPtr.Zero;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_width, IntPtr.Zero, (void**)param, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return *(int*)UnityInternals.object_unbox(intPtr);
            }
        }

        public unsafe static int height
        {
            get
            {
                IntPtr* param = null;
                IntPtr returnedException = IntPtr.Zero;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_height, IntPtr.Zero, (void**)param, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return *(int*)UnityInternals.object_unbox(intPtr);
            }
        }
    }
}
