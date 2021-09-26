using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Texture2D : Texture
    {
        private delegate void SetPixelsImplDelegate(IntPtr @this, int x, int y, int w, int h, IntPtr pixel, int miplevel, int frame);

        private static readonly IntPtr m_get_whiteTexture;
        private static readonly IntPtr m_ctor;
        private static readonly SetPixelsImplDelegate m_SetPixelsImpl;
        private static readonly IntPtr m_Apply;

        static Texture2D()
        {
            Il2CppClassPointerStore<Texture2D>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Texture2D");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Texture2D>.NativeClassPtr);

            m_ctor = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Texture2D>.NativeClassPtr, ".ctor", "System.Void", "System.Int32", "System.Int32");

            m_get_whiteTexture = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Texture2D>.NativeClassPtr, "get_whiteTexture", "UnityEngine.Texture2D");

            m_SetPixelsImpl = IL2CPP.ResolveICall<SetPixelsImplDelegate>("UnityEngine.Texture2D::SetPixelsImpl");
            m_Apply = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Texture2D>.NativeClassPtr, "Apply", "System.Void");
        }

        public Texture2D(IntPtr ptr) : base(ptr) { }

        public unsafe static Texture2D whiteTexture
        {
            get
            {
                IntPtr returnedException = IntPtr.Zero;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_whiteTexture, IntPtr.Zero, (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                return intPtr == IntPtr.Zero ? null : new Texture2D(intPtr);
            }
        }

        public unsafe Texture2D(int width, int height) : base(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<Texture2D>.NativeClassPtr))
        {
            void** args = stackalloc void*[2];
            args[0] = &width;
            args[1] = &height;
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_ctor, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), args, ref returnedException);
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
            IntPtr pixelArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<Color>.NativeClassPtr, (uint)pixel.Length);
            for (var i = 0; i < pixel.Length; i++)
            {
                IntPtr arrayStartPointer = IntPtr.Add(pixelArrayPtr, 4 * IntPtr.Size);
                ((Color*)arrayStartPointer.ToPointer())[i] = pixel[i];
            }

            m_SetPixelsImpl(IL2CPP.Il2CppObjectBaseToPtrNotNull(this), x, y, w, h, pixelArrayPtr, miplevel, frame);
        }

        public unsafe void Apply()
        {
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_Apply, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }
    }
}
