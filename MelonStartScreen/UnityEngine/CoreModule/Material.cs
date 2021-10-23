using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Material : InternalObjectBase
    {
        private static IntPtr m_SetPass;

        static Material()
        {
            InternalClassPointerStore<Material>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Material");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Material>.NativeClassPtr);

            m_SetPass = UnityInternals.GetMethod(InternalClassPointerStore<Material>.NativeClassPtr, "SetPass", "System.Boolean", "System.Int32");
        }

        public Material(IntPtr ptr) : base(ptr) { }

        public unsafe bool SetPass(int pass)
        {
            UnityInternals.ObjectBaseToPtrNotNull(this);
            void** ptr = stackalloc void*[1];
            ptr[0] = &pass;
            IntPtr returnedException = default;
            IntPtr obj = UnityInternals.runtime_invoke(m_SetPass, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)ptr, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return *(bool*)UnityInternals.object_unbox(obj);
        }
    }
}
