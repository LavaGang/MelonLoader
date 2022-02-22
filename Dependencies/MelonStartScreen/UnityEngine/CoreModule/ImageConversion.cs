using MelonLoader;
using MelonLoader.MelonStartScreen;
using System;
using System.Runtime.InteropServices;
using UnhollowerMini;
using static MelonLoader.MelonStartScreen.Core;

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
                method_LoadImage_ptr = UnityInternals.ResolveICall("UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
                if (method_LoadImage_ptr == IntPtr.Zero)
                {
                    Core.Logger.Error("Failed to resolve icall UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
                    return false;
                }
                if (method_LoadImage_ptr != IntPtr.Zero)
                    ImageConversion_LoadImage = (ImageConversion_LoadImage_Delegate)Marshal.GetDelegateForFunctionPointer(method_LoadImage_ptr, typeof(ImageConversion_LoadImage_Delegate));
            }
            if (ImageConversion_LoadImage != null)
            {

                IntPtr dataPtr = UnityInternals.array_new(InternalClassPointerStore<byte>.NativeClassPtr, (uint)data.Length);
                for (var i = 0; i < data.Length; i++)
                {
                    IntPtr arrayStartPointer = (IntPtr)((long)dataPtr + 4 * IntPtr.Size);
                    ((byte*)arrayStartPointer.ToPointer())[i] = data[i];
                }

                return ImageConversion_LoadImage(tex.Pointer, dataPtr, markNonReadable);
            }
            Core.Logger.Error("Failed to run UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
            return false;
        }
    }
}
