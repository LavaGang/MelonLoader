using System;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly)]
public class MelonIncompatibleAssembliesAttribute(params string[] assemblyNames) : Attribute
{
    /// <summary>
    /// The (simple) assembly names of the mods that are incompatible.
    /// </summary>
    public string[] AssemblyNames { get; internal set; } = assemblyNames;
}