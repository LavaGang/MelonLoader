using System;
using System.Reflection;

namespace MelonLoader.Wrappers
{
    internal static class WrapUtils
    {
        private readonly static Type refArray;

        static WrapUtils()
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                var asm = Assembly.Load("UnhollowerBaseLib");
                if (asm == null)
                {
                    Console.Module.Logger.Error("Failed to load assembly UnhollowerBaseLib");
                    return;
                }

                refArray = asm.GetType("UnhollowerBaseLib.Il2CppReferenceArray`1");
                if (refArray == null)
                {
                    Console.Module.Logger.Error("Failed to resolve type of Il2CppReferenceArray");
                }

                return;
            }
        }

        public static object ToIl2CppReferenceArray<T>(this T[] array)
            => Activator.CreateInstance(refArray.MakeGenericType(typeof(T)), new object[] { array });
    }
}
