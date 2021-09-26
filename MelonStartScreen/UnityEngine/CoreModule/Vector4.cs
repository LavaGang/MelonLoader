using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityEngine
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Vector4
    {
        [FieldOffset(0)]
        public float x;
        [FieldOffset(4)]
        public float y;
        [FieldOffset(8)]
        public float z;
        [FieldOffset(12)]
        public float w;

        static Vector4()
        {
            Il2CppClassPointerStore<Vector4>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Vector4");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Vector4>.NativeClassPtr);
        }
    }
}
