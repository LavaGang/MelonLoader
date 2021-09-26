using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Font : Il2CppObjectBase
    {
        private static IntPtr m_get_material;

        static Font()
        {
            Il2CppClassPointerStore<Font>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.TextRenderingModule.dll", "UnityEngine", "Font");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Font>.NativeClassPtr);

            m_get_material = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Font>.NativeClassPtr, "get_material", "UnityEngine.Material");
        }

        public Font(IntPtr ptr) : base(ptr) { }

        public unsafe Material material
        {
            get
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(Font.m_get_material, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return (intPtr != IntPtr.Zero) ? new Material(intPtr) : null;
            }
            // set
        }
    }
}
