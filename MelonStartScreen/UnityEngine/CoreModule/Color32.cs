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
            InternalClassPointerStore<Color32>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Color32");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Color32>.NativeClassPtr);

            m_op_Implicit = UnityInternals.GetMethod(InternalClassPointerStore<Color32>.NativeClassPtr, "op_Implicit", "UnityEngine.Color", "UnityEngine.Color32");
        }

        public Color32(byte r, byte g, byte b, byte a)
        {
            this.rgba = 0;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public unsafe static implicit operator Color(Color32 c)
        {
            void** ptr = stackalloc void*[1];
            ptr[0] = &c;
            IntPtr returnedException = default;
            IntPtr obj = UnityInternals.runtime_invoke(m_op_Implicit, IntPtr.Zero, ptr, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return *(Color*)UnityInternals.object_unbox(obj);
        }
    }
}
