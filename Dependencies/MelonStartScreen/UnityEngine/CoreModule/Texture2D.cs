using MelonLoader;
using MelonLoader.MelonStartScreen.NativeUtils;
using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class Texture2D : Texture
    {
        private delegate void SetPixelsImplDelegate_2017(IntPtr @this, int x, int y, int w, int h, IntPtr pixel, int miplevel);
        private delegate void SetPixelsImplDelegate_2018(IntPtr @this, int x, int y, int w, int h, IntPtr pixel, int miplevel, int frame);

        private static readonly IntPtr m_get_whiteTexture;
        private static readonly IntPtr m_ctor;
        private static readonly SetPixelsImplDelegate_2017 m_SetPixelsImpl_2017;
        private static readonly SetPixelsImplDelegate_2018 m_SetPixelsImpl_2018;
        private static readonly IntPtr m_Apply;

        private static readonly int type_SetPixelsImpl = -1;

        static Texture2D()
        {
            InternalClassPointerStore<Texture2D>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Texture2D");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Texture2D>.NativeClassPtr);

            m_ctor = UnityInternals.GetMethod(InternalClassPointerStore<Texture2D>.NativeClassPtr, ".ctor", "System.Void", "System.Int32", "System.Int32");

            m_get_whiteTexture = UnityInternals.GetMethod(InternalClassPointerStore<Texture2D>.NativeClassPtr, "get_whiteTexture", "UnityEngine.Texture2D");

            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2018.1.0" })) {
                type_SetPixelsImpl = 1;
                m_SetPixelsImpl_2018 = UnityInternals.ResolveICall<SetPixelsImplDelegate_2018>("UnityEngine.Texture2D::SetPixelsImpl");
            }
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2017.1.0" }))
            {
                type_SetPixelsImpl = 0;
                m_SetPixelsImpl_2017 = UnityInternals.ResolveICall<SetPixelsImplDelegate_2017>("UnityEngine.Texture2D::SetPixels");
            }

            m_Apply = UnityInternals.GetMethod(InternalClassPointerStore<Texture2D>.NativeClassPtr, "Apply", "System.Void");
        }

        public Texture2D(IntPtr ptr) : base(ptr) { }

        public unsafe static Texture2D whiteTexture
        {
            get
            {
                IntPtr returnedException = IntPtr.Zero;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_whiteTexture, IntPtr.Zero, (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return intPtr == IntPtr.Zero ? null : new Texture2D(intPtr);
            }
        }

        public unsafe Texture2D(int width, int height) : base(UnityInternals.object_new(InternalClassPointerStore<Texture2D>.NativeClassPtr))
        {
            void** args = stackalloc void*[2];
            args[0] = &width;
            args[1] = &height;
            IntPtr returnedException = default;
            UnityInternals.runtime_invoke(m_ctor, UnityInternals.ObjectBaseToPtrNotNull(this), args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        public unsafe void SetPixels(Color[] colors)
        {
            SetPixels(0, 0, width, height, colors, 0);
        }

        public unsafe void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, int miplevel = 0)
        {
            SetPixelsImpl(x, y, blockWidth, blockHeight, colors, miplevel, 0);
        }

        public unsafe void SetPixelsImpl(int x, int y, int w, int h, Color[] pixel, int miplevel, int frame)
        {
            IntPtr pixelArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Color>.NativeClassPtr, (uint)pixel.Length);
            for (var i = 0; i < pixel.Length; i++)
            {
                IntPtr arrayStartPointer = (IntPtr)((long)pixelArrayPtr + 4 * IntPtr.Size);
                ((Color*)arrayStartPointer.ToPointer())[i] = pixel[i];
            }

            if (type_SetPixelsImpl == 0)
                m_SetPixelsImpl_2017(UnityInternals.ObjectBaseToPtrNotNull(this), x, y, w, h, pixelArrayPtr, miplevel);
            else if (type_SetPixelsImpl == 1)
                m_SetPixelsImpl_2018(UnityInternals.ObjectBaseToPtrNotNull(this), x, y, w, h, pixelArrayPtr, miplevel, frame);
        }

        public unsafe void Apply()
        {
            IntPtr returnedException = default;
            UnityInternals.runtime_invoke(m_Apply, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }
    }
}
