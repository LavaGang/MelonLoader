using MelonLoader;
using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal static class ImageConversion
    {
        private delegate bool ImageConversion_LoadImage_Delegate(IntPtr tex, IntPtr data, bool markNonReadable);
        private static ImageConversion_LoadImage_Delegate ImageConversion_LoadImage;

        static ImageConversion()
        {
            IntPtr ptr = UnityInternals.ResolveICall("UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
            if (ptr != IntPtr.Zero)
                ImageConversion_LoadImage = (ImageConversion_LoadImage_Delegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(ImageConversion_LoadImage_Delegate));
            else
                MelonLogger.Error("Failed to resolve icall UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
        }

        public unsafe static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (ImageConversion_LoadImage == null)
            {
                MelonLogger.Error("Failed to run UnityEngine.ImageConversion::LoadImage(UnityEngine.Texture2D,System.Byte[],System.Boolean)");
                return false;
            }

            IntPtr dataPtr = UnityInternals.array_new(InternalClassPointerStore<byte>.NativeClassPtr, (uint)data.Length);
            for (var i = 0; i < data.Length; i++)
            {
                IntPtr arrayStartPointer = (IntPtr)((long)dataPtr + 4 * IntPtr.Size);
                ((byte*)arrayStartPointer.ToPointer())[i] = data[i];
            }

            return ImageConversion_LoadImage(tex.Pointer, dataPtr, markNonReadable);
        }
    }
}
