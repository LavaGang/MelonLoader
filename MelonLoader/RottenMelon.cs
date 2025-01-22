using System;

namespace MelonLoader;

/// <summary>
/// An info class for broken Melons.
/// </summary>
public sealed class RottenMelon(Type type, string errorMessage, Exception exception = null)
{
    public readonly MelonAssembly assembly = MelonAssembly.LoadMelonAssembly(null, type.Assembly);
    public readonly Type type = type;
    public readonly string errorMessage = errorMessage;
    public readonly Exception exception = exception;
}
