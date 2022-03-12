using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Color32
    {
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
            InternalClassPointerStore<Color32>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Color32");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Color32>.NativeClassPtr);
        }

        public Color32(byte r, byte g, byte b, byte a)
        {
            this.rgba = 0;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator Color(Color32 c)
        {
            return new Color(c.r / 255f, c.g / 255f, c.b / 255f, c.a / 255f);
        }
    }
}
