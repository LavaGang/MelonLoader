using System;
using System.Diagnostics.CodeAnalysis;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class MelonProcessAttribute : Attribute
{
    public MelonProcessAttribute(string exeName = null)
        => ExecutableName = RemoveExtension(exeName);

    /// <summary>
    /// Name of the Game's Executable without the '.exe' extension.
    /// </summary>
    public string ExecutableName { get; internal set; }

    /// <summary>
    /// Name of the Game's Executable without the '.exe' extension.
    /// </summary>
    [Obsolete("Use ExecutableName instead.", true)]
    [SuppressMessage("Naming", "CA1707: Identifiers should not contain underscores", Justification = "It's deprecated")]
    public string EXE_Name => ExecutableName;

    /// <summary>
    /// If the Attribute is set as Universal or not.
    /// </summary>
    public bool Universal
        => string.IsNullOrEmpty(ExecutableName);

    /// <summary>
    /// Checks if the Attribute is compatible with <paramref name="processName"/> or not.
    /// </summary>
    public bool IsCompatible(string processName)
        => Universal || string.IsNullOrEmpty(processName) || (RemoveExtension(processName) == ExecutableName);

    private string RemoveExtension(string name)
        => name == null ? null : (name.EndsWith(".exe") ? name[..^4] : name);

}