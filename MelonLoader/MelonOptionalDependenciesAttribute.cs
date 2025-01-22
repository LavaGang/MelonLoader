using System;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly)]
public class MelonOptionalDependenciesAttribute(params string[] assemblyNames) : Attribute
{
    /// <summary>
    /// The (simple) assembly names of the dependencies that should be regarded as optional.
    /// </summary>
    public string[] AssemblyNames { get; internal set; } = assemblyNames;
}