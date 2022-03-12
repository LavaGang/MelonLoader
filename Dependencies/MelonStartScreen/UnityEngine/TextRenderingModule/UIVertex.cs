using MelonLoader;
using MelonLoader.MelonStartScreen.NativeUtils;
using System;
using System.Runtime.InteropServices;

namespace MelonUnityEngine
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UIVertex_2020
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector4 tangent;
        public Color32 color;
        public Vector4 uv0;
        public Vector4 uv1;
        public Vector4 uv2;
        public Vector4 uv3;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct UIVertex_2018
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector4 tangent;
        public Color32 color;
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct UIVertex_2017
    {
        public Vector3 position;
        public Vector3 normal;
        public Color32 color;
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
        public Vector4 tangent;
    }

    internal struct UIVertexWrapper
    {
        private static readonly int mode = -1;
        public static readonly int sizeOfElement = 0;

        private IntPtr ptr;

        unsafe static UIVertexWrapper()
        {
            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2020.2.0", "2021.1.0" }))
            {
                mode = 2;
                sizeOfElement = sizeof(UIVertex_2020);
            }
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2018.1.0" }))
            {
                mode = 1;
                sizeOfElement = sizeof(UIVertex_2018);
            }
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2017.2.0" }))
            {
                mode = 0;
                sizeOfElement = sizeof(UIVertex_2017);
            }
        }

        public UIVertexWrapper(IntPtr ptr)
        {
            this.ptr = ptr;
        }

        public unsafe Vector3 position =>
            mode == 2 ? (*(UIVertex_2020*)ptr).position :
            mode == 1 ? (*(UIVertex_2018*)ptr).position :
            mode == 0 ? (*(UIVertex_2017*)ptr).position :
            throw new Exception("UIVertex mode not set");

        public unsafe Vector3 normal =>
            mode == 2 ? (*(UIVertex_2020*)ptr).normal :
            mode == 1 ? (*(UIVertex_2018*)ptr).normal :
            mode == 0 ? (*(UIVertex_2017*)ptr).normal :
            throw new Exception("UIVertex mode not set");

        public unsafe Vector4 tangent =>
            mode == 2 ? (*(UIVertex_2020*)ptr).tangent :
            mode == 1 ? (*(UIVertex_2018*)ptr).tangent :
            mode == 0 ? (*(UIVertex_2017*)ptr).tangent :
            throw new Exception("UIVertex mode not set");

        public unsafe Color32 color =>
            mode == 2 ? (*(UIVertex_2020*)ptr).color :
            mode == 1 ? (*(UIVertex_2018*)ptr).color :
            mode == 0 ? (*(UIVertex_2017*)ptr).color :
            throw new Exception("UIVertex mode not set");

        public unsafe Vector2 uv0 =>
            mode == 2 ? (Vector2)(*(UIVertex_2020*)ptr).uv0 :
            mode == 1 ? (*(UIVertex_2018*)ptr).uv0 :
            mode == 0 ? (*(UIVertex_2017*)ptr).uv0 :
            throw new Exception("UIVertex mode not set");
    }
}
