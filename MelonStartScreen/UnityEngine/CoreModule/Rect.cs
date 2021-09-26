using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityEngine
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
            Il2CppClassPointerStore<Rect>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Rect");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Rect>.NativeClassPtr);
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
