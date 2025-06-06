#if ANDROID
using System;

namespace MelonLoader.Java;

public static partial class JNI
{
    public static class TypeSignature
    {
        public const string Bool = "Z";

        public const string Byte = "B";

        public const string Char = "C";

        public const string Short = "S";

        public const string Int = "I";

        public const string Long = "J";

        public const string Float = "F";

        public const string Double = "D";
    }

    public static string GetTypeSignature<T>()
    {
        Type t = typeof(T);

        if (t == typeof(bool))
        {
            return TypeSignature.Bool;
        }
        else if (t == typeof(sbyte))
        {
            return TypeSignature.Byte;
        }
        else if (t == typeof(char))
        {
            return TypeSignature.Char;
        }
        else if (t == typeof(short))
        {
            return TypeSignature.Short;
        }
        else if (t == typeof(int))
        {
            return TypeSignature.Int;
        }
        else if (t == typeof(long))
        {
            return TypeSignature.Long;
        }
        else if (t == typeof(float))
        {
            return TypeSignature.Float;
        }
        else if (t == typeof(double))
        {
            return TypeSignature.Double;
        }
        else
        {
            throw new ArgumentException($"GetTypeSignature Type {t} not supported.");
        }
    }
}
#endif
