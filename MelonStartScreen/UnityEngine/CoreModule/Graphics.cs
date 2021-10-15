using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Graphics : InternalObjectBase
    {
        private delegate IntPtr Internal_DrawMeshNow1_InjectedDelegate(IntPtr mesh, int subsetIndex, ref Vector3 position, ref Quaternion rotation);

        private static readonly IntPtr m_DrawTexture;
        private static readonly Internal_DrawMeshNow1_InjectedDelegate fd_Internal_DrawMeshNow1_Injected;

        static Graphics()
        {
            InternalClassPointerStore<Graphics>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Graphics");
            m_DrawTexture = UnityInternals.GetMethod(InternalClassPointerStore<Graphics>.NativeClassPtr, "DrawTexture", "System.Void", new string[] { "UnityEngine.Rect", "UnityEngine.Texture", "UnityEngine.Material", "System.Int32" });

            fd_Internal_DrawMeshNow1_Injected = UnityInternals.ResolveICall<Internal_DrawMeshNow1_InjectedDelegate>("UnityEngine.Graphics::Internal_DrawMeshNow1_Injected");
        }

        public Graphics(IntPtr ptr) : base(ptr) { }

        public unsafe static void DrawTexture(Rect screenRect, Texture2D texture, IntPtr material = default, int pass = -1)
        {
            void** args = stackalloc void*[4];
            args[0] = &screenRect;
            args[1] = (void*)UnityInternals.ObjectBaseToPtrNotNull(texture);
            args[2] = (void*)material;
            args[3] = &pass;
            IntPtr returnedException = default;
            UnityInternals.runtime_invoke(m_DrawTexture, IntPtr.Zero, args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation) =>
            DrawMeshNow(mesh, position, rotation, -1);

        public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");
            Internal_DrawMeshNow1(mesh, materialIndex, position, rotation);
        }

        private static void Internal_DrawMeshNow1(Mesh mesh, int subsetIndex, Vector3 position, Quaternion rotation) =>
            Internal_DrawMeshNow1_Injected(mesh, subsetIndex, ref position, ref rotation);

        private static void Internal_DrawMeshNow1_Injected(Mesh mesh, int subsetIndex, ref Vector3 position, ref Quaternion rotation) =>
            fd_Internal_DrawMeshNow1_Injected(UnityInternals.ObjectBaseToPtr(mesh), subsetIndex, ref position, ref rotation);
    }
}
