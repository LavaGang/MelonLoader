using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonAdditionalDependenciesAttribute : Attribute
    {
        /// <summary>
        /// Additional (simple) assembly names of the dependencies that should be regarded.
        /// </summary>
        public string[] AssemblyNames { get; internal set; }

        public MelonAdditionalDependenciesAttribute(params string[] assemblyNames) { AssemblyNames = assemblyNames; }
    }
}