using System;
using System.Collections.Generic;

namespace MelonLoader.TinyJSON;

[Obsolete("Please use Newtonsoft.Json or System.Text.Json instead. This will be removed in a future version.", true)]
public static class Extensions
{
    public static bool AnyOfType<TSource>(this IEnumerable<TSource> source, Type expectedType)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (expectedType == null)
        {
            throw new ArgumentNullException(nameof(expectedType));
        }

        foreach (var item in source)
        {
            if (expectedType.IsInstanceOfType(item))
            {
                return true;
            }
        }

        return false;
    }
}
