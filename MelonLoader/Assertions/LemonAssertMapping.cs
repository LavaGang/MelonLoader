using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MelonLoader.Assertions;

public static class LemonAssertMapping
{
    internal static Dictionary<Type, Delegate> IsNull = [];
    internal static Dictionary<Type, Delegate> IsEqual = [];

    internal static void Setup()
    {
        RegisterIsNull<object>(IsNull_object);
        RegisterIsNull<string>(IsNull_string);
        RegisterIsEqual<object>(IsEqual_object);
    }

    public static void RegisterIsNull<T>(Func<T, bool> method)
        => Register<T>(method, ref IsNull);

    public static void RegisterIsEqual<T>(Func<T, T, bool> method)
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

    #region Obsolete

    [Obsolete("Use RegisterIsNull instead. This will be removed in a future version.", true)]
    [SuppressMessage("Naming", "CA1707: Identifiers should not contain underscores", Justification = "Reason for deprecation")]
    public static void Register_IsNull<T>(Func<T, bool> method)
        => Register<T>(method, ref IsNull);

    [Obsolete("Use RegisterIsNull instead. This will be removed in a future version.", true)]
    [SuppressMessage("Naming", "CA1707: Identifiers should not contain underscores", Justification = "Reason for deprecation")]
    public static void Register_IsEqual<T>(Func<T, T, bool> method)
        => Register<T>(method, ref IsEqual);

    #endregion
}
