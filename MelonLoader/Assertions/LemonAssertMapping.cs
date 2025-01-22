using System;
using System.Collections.Generic;

namespace MelonLoader.Assertions;

public static class LemonAssertMapping
{
    internal static Dictionary<Type, Delegate> IsNull = [];
    internal static Dictionary<Type, Delegate> IsEqual = [];

    internal static void Setup()
    {
        Register_IsNull<object>(IsNull_object);
        Register_IsNull<string>(IsNull_string);
        Register_IsEqual<object>(IsEqual_object);
    }

    public static void Register_IsNull<T>(Func<T, bool> method)
        => Register<T>(method, ref IsNull);
    public static void Register_IsEqual<T>(Func<T, T, bool> method)
        => Register<T>(method, ref IsEqual);
    private static void Register<T>(Delegate method, ref Dictionary<Type, Delegate> tbl)
    {
        if (method == null)
            throw new NullReferenceException(nameof(method));
        var inputType = typeof(T);
        if (tbl.ContainsKey(inputType))
            return;
        lock (tbl)
            tbl[inputType] = method;
    }

    private static bool IsNull_object(object obj)
        => obj == null;
    private static bool IsNull_string(string obj)
        => string.IsNullOrEmpty(obj);
    private static bool IsEqual_object(object obj, object obj2)
    {
        return obj == null ? obj2 == null : obj2 == null ? obj == null : obj.Equals(obj2);
    }
}
