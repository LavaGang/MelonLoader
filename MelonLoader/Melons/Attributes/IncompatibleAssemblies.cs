using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonIncompatibleAssembliesAttribute : Attribute
    {
        /// <summary>
        /// The (simple) assembly names of the mods that are incompatible.
        /// </summary>
        public string[] AssemblyNames { get; internal set; }

        public MelonIncompatibleAssembliesAttribute(params string[] assemblyNames) { AssemblyNames = assemblyNames; }
    }
}