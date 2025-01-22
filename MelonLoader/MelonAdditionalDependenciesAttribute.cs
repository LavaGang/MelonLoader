using System;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Assembly)]
public class MelonAdditionalDependenciesAttribute(params string[] assemblyNames) : Attribute
{
    /// <summary>
    /// The (simple) assembly names of Additional Dependencies that aren't directly referenced but should still be regarded as important.
    /// </summary>
    public string[] AssemblyNames { get; internal set; } = assemblyNames;
}