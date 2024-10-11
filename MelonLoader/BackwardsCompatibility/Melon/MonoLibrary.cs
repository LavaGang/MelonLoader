#if !NET6_0_OR_GREATER

using System;

namespace MelonLoader.MonoInternals
{
    [Obsolete("MelonLoader.MonoInternals.MonoLibrary is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MonoLibrary instead.")]
    public class MonoLibrary : InternalUtils.MonoLibrary { }
}

#endif