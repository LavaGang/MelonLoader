using MelonLoader.MelonStartScreen.NativeUtils;

namespace UnityEngine.Rendering
{
    static class VertexAttribute
    {
        public static int Vertex = 0;
        public static int Normal = 1;

        [NativeFieldValue(01, NativeSignatureFlags.None, 7, "2017.1.0")]
        [NativeFieldValue(02, NativeSignatureFlags.None, 2, "2018.1.0")]
        public static int Tangent;

        [NativeFieldValue(01, NativeSignatureFlags.None, 2, "2017.1.0")]
        [NativeFieldValue(02, NativeSignatureFlags.None, 3, "2018.1.0")]
        public static int Color;

        [NativeFieldValue(01, NativeSignatureFlags.None, 3, "2017.1.0")]
        [NativeFieldValue(02, NativeSignatureFlags.None, 4, "2018.1.0")]
        public static int TexCoord0;
    }
}
