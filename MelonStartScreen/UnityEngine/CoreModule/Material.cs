using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Material : InternalObjectBase
    {
        private static unsafe delegate* unmanaged[Cdecl]<IntPtr, int, bool> m_SetPass;

        unsafe static Material()
        {
            InternalClassPointerStore<Material>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Material");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Material>.NativeClassPtr);

            //m_SetPass = UnityInternals.GetMethod(InternalClassPointerStore<Material>.NativeClassPtr, "SetPass", "System.Boolean", "System.Int32");
            m_SetPass = (delegate* unmanaged[Cdecl]<IntPtr, int, bool>)UnityInternals.ResolveICall("UnityEngine.Material::SetPass");
        }

        public Material(IntPtr ptr) : base(ptr) { }

        public unsafe bool SetPass(int pass)
        {
            return m_SetPass(UnityInternals.ObjectBaseToPtrNotNull(this), pass);
        }
    }
}
