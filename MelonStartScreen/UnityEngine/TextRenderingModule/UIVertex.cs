using System.Runtime.InteropServices;

namespace UnityEngine
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct UIVertex
    {
        [FieldOffset(0)]
        public Vector3 position;
        [FieldOffset(12)]
        public Vector3 normal;
        [FieldOffset(24)]
        public Vector4 tangent;
        [FieldOffset(40)]
        public Color32 color;
        [FieldOffset(44)]
        public Vector2 uv0;
        [FieldOffset(52)]
        public Vector2 uv1;
        [FieldOffset(60)]
        public Vector2 uv2;
        [FieldOffset(68)]
        public Vector2 uv3;
    }
}
