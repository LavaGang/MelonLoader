using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
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
            InternalClassPointerStore<Color>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Color");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Color>.NativeClassPtr);
            m_ToString = UnityInternals.GetMethod(InternalClassPointerStore<Color>.NativeClassPtr, "ToString", "System.String");
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
