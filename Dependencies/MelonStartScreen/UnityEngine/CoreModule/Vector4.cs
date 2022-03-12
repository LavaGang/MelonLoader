using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
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
            InternalClassPointerStore<Vector4>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Vector4");
        }

        public static explicit operator Vector2(Vector4 src) => new Vector2(src.x, src.y);
    }
}
