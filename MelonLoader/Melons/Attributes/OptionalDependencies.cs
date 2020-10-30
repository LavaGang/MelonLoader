﻿using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonOptionalDependenciesAttribute : Attribute
    {
        /// <summary>
        /// The (simple) assembly names of the dependencies that should be regarded as optional.
        /// </summary>
        public string[] AssemblyNames { get; internal set; }

        public MelonOptionalDependenciesAttribute(params string[] assemblyNames) { AssemblyNames = assemblyNames; }
    }

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