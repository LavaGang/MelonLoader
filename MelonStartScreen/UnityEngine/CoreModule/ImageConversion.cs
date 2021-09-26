using MelonLoader;
using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityEngine
{
    internal static class ImageConversion
    {
        private delegate bool ImageConversion_LoadImage_Delegate(IntPtr tex, IntPtr data, bool markNonReadable);

        private static IntPtr method_LoadImage_ptr;
        private static ImageConversion_LoadImage_Delegate ImageConversion_LoadImage;

        public unsafe static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (method_LoadImage_ptr == IntPtr.Zero)
            {
                method_LoadImage_ptr = IL2CPP.il2cpp_resolve_icall("UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
                if (method_LoadImage_ptr == IntPtr.Zero)
                {
                    MelonLogger.Error("Failed to resolve icall UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
                    return false;
                }
                if (method_LoadImage_ptr != IntPtr.Zero)
                    ImageConversion_LoadImage = Marshal.GetDelegateForFunctionPointer<ImageConversion_LoadImage_Delegate>(method_LoadImage_ptr);
            }
            if (ImageConversion_LoadImage != null)
            {

                IntPtr dataPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<byte>.NativeClassPtr, (uint)data.Length);
                for (var i = 0; i < data.Length; i++)
                {
                    IntPtr arrayStartPointer = IntPtr.Add(dataPtr, 4 * IntPtr.Size);
                    ((byte*)arrayStartPointer.ToPointer())[i] = data[i];
                }

                return ImageConversion_LoadImage(tex.Pointer, dataPtr, markNonReadable);
            }
            MelonLogger.Error("Failed to run UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
            return false;
        }
    }
}
