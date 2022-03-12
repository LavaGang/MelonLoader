using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Rect
    {
        [FieldOffset(0)]
        public float m_XMin;
        [FieldOffset(4)]
        public float m_YMin;
        [FieldOffset(8)]
        public float m_Width;
        [FieldOffset(12)]
        public float m_Height;

        static Rect()
        {
            InternalClassPointerStore<Rect>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Rect");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Rect>.NativeClassPtr);
        }

        public Rect(int x, int y, int width, int height)
        {
            m_XMin = x;
            m_YMin = y;
            m_Width = width;
            m_Height = height;
        }
    }
}
