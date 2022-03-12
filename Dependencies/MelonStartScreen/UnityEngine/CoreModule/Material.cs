using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class Material : UnityObject
    {
        private delegate bool d_SetPass(IntPtr @this, int pass);
        private static readonly d_SetPass m_SetPass;

        unsafe static Material()
        {
            InternalClassPointerStore<Material>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Material");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Material>.NativeClassPtr);

            //m_SetPass = UnityInternals.GetMethod(InternalClassPointerStore<Material>.NativeClassPtr, "SetPass", "System.Boolean", "System.Int32");
            m_SetPass = UnityInternals.ResolveICall<d_SetPass>("UnityEngine.Material::SetPass");
        }

        public Material(IntPtr ptr) : base(ptr) { }

        public unsafe bool SetPass(int pass)
        {
            return m_SetPass(UnityInternals.ObjectBaseToPtrNotNull(this), pass);
        }
    }
}
