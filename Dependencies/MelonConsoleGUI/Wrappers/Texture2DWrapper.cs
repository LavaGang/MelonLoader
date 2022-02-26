using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MelonLoader.Wrappers
{
    internal static class Texture2DWrapper
    {
        private readonly static MethodInfo setPixelsMethod;

        public readonly static LemonAction<Texture2D, Color[]> SetPixels;

        static Texture2DWrapper()
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                setPixelsMethod = typeof(Texture2D).GetMethods().FirstOrDefault(x => x.Name == "SetPixels" && x.GetParameters().Length == 1);

                SetPixels = Il2CppSetPixels;
            }
            else
            {
                SetPixels = MonoSetPixels;
            }
        }

        private static void Il2CppSetPixels(Texture2D texture, Color[] pixels)
            => setPixelsMethod.Invoke(texture, new object[] { pixels.ToIl2CppStructArray() });

        private static void MonoSetPixels(Texture2D texture, Color[] pixels)
            => texture.SetPixels(pixels);

        public static void SetPixelsWrapper(this Texture2D texture, Color[] pixels)
            => SetPixels(texture, pixels);
    }
}
