using MelonLoader;
using MelonLoader.MelonStartScreen.NativeUtils;
using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class Graphics : InternalObjectBase
    {
        private delegate IntPtr Internal_DrawMeshNow1_InjectedDelegate(IntPtr mesh, int subsetIndex, ref Vector3 position, ref Quaternion rotation);
        private delegate void Internal_DrawTextureDelegate(IntPtr args);

        private static readonly Internal_DrawTextureDelegate fd_Internal_DrawTexture;
        private static readonly Internal_DrawMeshNow1_InjectedDelegate fd_Internal_DrawMeshNow1_Injected;

        private static readonly int m_DrawTexture_Internal_struct = -1;

        unsafe static Graphics()
        {
            InternalClassPointerStore<Graphics>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Graphics");
            fd_Internal_DrawTexture = UnityInternals.ResolveICall<Internal_DrawTextureDelegate>("UnityEngine.Graphics::Internal_DrawTexture");

            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2018.2.0", "2019.1.0" }))
                fd_Internal_DrawMeshNow1_Injected = UnityInternals.ResolveICall<Internal_DrawMeshNow1_InjectedDelegate>("UnityEngine.Graphics::Internal_DrawMeshNow1_Injected");
            else
                fd_Internal_DrawMeshNow1_Injected = UnityInternals.ResolveICall<Internal_DrawMeshNow1_InjectedDelegate>("UnityEngine.Graphics::INTERNAL_CALL_Internal_DrawMeshNow1");

            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2019.3.0", "2020.1.0" }))
                m_DrawTexture_Internal_struct = 3;
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2018.2.0", "2019.1.0" }))
                m_DrawTexture_Internal_struct = 2;
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2017.3.0", "2018.1.0" }))
                m_DrawTexture_Internal_struct = 1;
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2017.2.0" }))
                m_DrawTexture_Internal_struct = 0;
        }

        public Graphics(IntPtr ptr) : base(ptr) { }

        public unsafe static void DrawTexture(Rect screenRect, Texture2D texture)
        {
            if ((texture == null) || (texture.Pointer == IntPtr.Zero))
                return;

            if (m_DrawTexture_Internal_struct == 0)
            {
                Internal_DrawTextureArguments_2017 args = default;
                args.screenRect = screenRect;
                args.sourceRect = new Rect(0, 0, 1, 1);
                args.color = new Color32(128, 128, 128, 128);
                args.texture = UnityInternals.ObjectBaseToPtrNotNull(texture);
                fd_Internal_DrawTexture((IntPtr)(&args));
            }
            else if (m_DrawTexture_Internal_struct == 1)
            {
                Internal_DrawTextureArguments_2018 args = default;
                args.screenRect = screenRect;
                args.sourceRect = new Rect(0, 0, 1, 1);
                args.color = new Color32(128, 128, 128, 128);
                args.texture = UnityInternals.ObjectBaseToPtrNotNull(texture);
                fd_Internal_DrawTexture((IntPtr)(&args));
            }
            else if (m_DrawTexture_Internal_struct == 2)
            {
                Internal_DrawTextureArguments_2019 args = default;
                args.screenRect = screenRect;
                args.sourceRect = new Rect(0, 0, 1, 1);
                args.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                args.texture = UnityInternals.ObjectBaseToPtrNotNull(texture);
                fd_Internal_DrawTexture((IntPtr)(&args));
            }
            else if (m_DrawTexture_Internal_struct == 3)
            {
                Internal_DrawTextureArguments_2020 args = default;
                args.screenRect = screenRect;
                args.sourceRect = new Rect(0, 0, 1, 1);
                args.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                args.leftBorderColor = new Color(0, 0, 0, 1);
                args.topBorderColor = new Color(0, 0, 0, 1);
                args.rightBorderColor = new Color(0, 0, 0, 1);
                args.bottomBorderColor = new Color(0, 0, 0, 1);
                args.smoothCorners = true;
                args.texture = UnityInternals.ObjectBaseToPtrNotNull(texture);
                fd_Internal_DrawTexture((IntPtr)(&args));
            }
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

        private static void Internal_DrawMeshNow1_Injected(Mesh mesh, int subsetIndex, ref Vector3 position, ref Quaternion rotation)
        {
            if ((mesh == null) || (mesh.Pointer == IntPtr.Zero))
                return;
            fd_Internal_DrawMeshNow1_Injected(UnityInternals.ObjectBaseToPtr(mesh), subsetIndex, ref position, ref rotation);
        }
    }
}
