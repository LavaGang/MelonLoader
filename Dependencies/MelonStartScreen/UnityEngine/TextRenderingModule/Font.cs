using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class Font : UnityObject
    {
        private static IntPtr m_get_material;

        static Font()
        {
            InternalClassPointerStore<Font>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.TextRenderingModule.dll", "UnityEngine", "Font");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Font>.NativeClassPtr);

            m_get_material = UnityInternals.GetMethod(InternalClassPointerStore<Font>.NativeClassPtr, "get_material", "UnityEngine.Material");
        }

        public Font(IntPtr ptr) : base(ptr) { }

        public unsafe Material material
        {
            get
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(Font.m_get_material, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return (intPtr != IntPtr.Zero) ? new Material(intPtr) : null;
            }
            // set
        }
    }
}
