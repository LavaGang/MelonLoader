using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityEngine
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Color
    {
        private static readonly IntPtr m_ToString;

        [FieldOffset(0)]
        public float r;
        [FieldOffset(4)]
        public float g;
        [FieldOffset(8)]
        public float b;
        [FieldOffset(12)]
        public float a;

        static Color()
        {
            Il2CppClassPointerStore<Color>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Color");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Color>.NativeClassPtr);
            m_ToString = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Color>.NativeClassPtr, "ToString", "System.String");
        }

        public Color(float r, float g, float b, float a = 1f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}
