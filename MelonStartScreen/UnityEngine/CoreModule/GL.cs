using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal sealed class GL
    {
        private static readonly IntPtr m_get_sRGBWrite;

        static GL()
        {
            InternalClassPointerStore<GL>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "GL");
            UnityInternals.runtime_class_init(InternalClassPointerStore<GL>.NativeClassPtr);

            m_get_sRGBWrite = UnityInternals.GetMethod(InternalClassPointerStore<GL>.NativeClassPtr, "get_sRGBWrite", "System.Boolean");
        }

        public unsafe static bool sRGBWrite
        {
            get
            {
                IntPtr returnedException = default;
                IntPtr obj = UnityInternals.runtime_invoke(m_get_sRGBWrite, IntPtr.Zero, (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return *(bool*)UnityInternals.object_unbox(obj);
            }
            // set
        }
    }
}
