using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
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
            InternalClassPointerStore<Vector2>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Vector2");
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
