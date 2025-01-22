using System;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class MelonGameVersionAttribute(string version = null) : Attribute
{

    /// <summary>
    /// Version of the Game.
    /// </summary>
    public string Version { get; internal set; } = version;

    /// <summary>
    /// If the Attribute is set as Universal or not.
    /// </summary>
    public bool Universal { get => string.IsNullOrEmpty(Version); }
}