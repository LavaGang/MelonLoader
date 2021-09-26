using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Material : Il2CppObjectBase
    {
        private static IntPtr m_SetPass;

        static Material()
        {
            Il2CppClassPointerStore<Material>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Material");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Material>.NativeClassPtr);

            m_SetPass = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Material>.NativeClassPtr, "SetPass", "System.Boolean", "System.Int32");
        }

        public Material(IntPtr ptr) : base(ptr) { }

        public unsafe bool SetPass(int pass)
        {
            IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
            void** ptr = stackalloc void*[1];
            ptr[0] = &pass;
            IntPtr returnedException = default;
            IntPtr obj = IL2CPP.il2cpp_runtime_invoke(m_SetPass, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)ptr, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return *(bool*)IL2CPP.il2cpp_object_unbox(obj);
        }
    }
}
