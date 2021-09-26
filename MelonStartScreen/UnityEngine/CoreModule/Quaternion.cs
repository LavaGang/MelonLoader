using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityEngine
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Quaternion
    {
        [FieldOffset(0)]
        public float x;
        [FieldOffset(4)]
        public float y;
        [FieldOffset(8)]
        public float z;
        [FieldOffset(12)]
        public float w;

        static Quaternion()
        {
            Il2CppClassPointerStore<Quaternion>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Quaternion");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Quaternion>.NativeClassPtr);
        }

        public static Quaternion identity => new Quaternion();
    }
}
