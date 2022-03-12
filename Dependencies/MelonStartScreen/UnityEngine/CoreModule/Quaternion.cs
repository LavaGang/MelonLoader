using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
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
            InternalClassPointerStore<Quaternion>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Quaternion");
        }

        public static Quaternion identity => new Quaternion();
    }
}
