#if !NET6_0_OR_GREATER

using System;

namespace MelonLoader.MonoInternals
{
    [Obsolete("MelonLoader.MonoInternals.MonoLibrary is Only Here for Compatibility Reasons. Please use MelonLoader.Utils.MonoLibrary instead.")]
    public class MonoLibrary : Utils.MonoLibrary { }
}

#endif