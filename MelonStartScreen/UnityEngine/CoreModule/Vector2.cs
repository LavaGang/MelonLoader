using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityEngine
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Vector2
    {
        [FieldOffset(0)]
        public float x;
        [FieldOffset(4)]
        public float y;

        static Vector2()
        {
            Il2CppClassPointerStore<Vector2>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Vector2");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Vector2>.NativeClassPtr);
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
