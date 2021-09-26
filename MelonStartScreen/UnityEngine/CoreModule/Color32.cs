using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityEngine
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Color32
    {
        private static readonly IntPtr m_op_Implicit;

        [FieldOffset(0)]
        public byte r;
        [FieldOffset(1)]
        public byte g;
        [FieldOffset(2)]
        public byte b;
        [FieldOffset(3)]
        public byte a;

        [FieldOffset(0)]
        public int rgba;

        static Color32()
        {
            Il2CppClassPointerStore<Color32>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Color32");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Color32>.NativeClassPtr);

            m_op_Implicit = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Color32>.NativeClassPtr, "op_Implicit", "UnityEngine.Color", "UnityEngine.Color32");
        }

        public unsafe static implicit operator Color(Color32 c)
        {
            void** ptr = stackalloc void*[1];
            ptr[0] = &c;
            IntPtr returnedException = default;
            IntPtr obj = IL2CPP.il2cpp_runtime_invoke(m_op_Implicit, IntPtr.Zero, ptr, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return *(Color*)IL2CPP.il2cpp_object_unbox(obj);
        }
    }
}
