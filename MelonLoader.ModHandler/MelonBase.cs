using System;

namespace MelonLoader
{
    public abstract class MelonBase
    {
        /// <summary>
        /// Gets the Assembly of the Mod or Plugin.
        /// </summary>
        public System.Reflection.Assembly Assembly { get; internal set; }

        /// <summary>
        /// Gets the File Location of the Mod or Plugin.
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// Enum for DLL Compatibility
        /// </summary>
        public enum MelonCompatibility
        {
            UNIVERSAL = 0,
            COMPATIBLE = 1,
            NOATTRIBUTE = 2,
            INCOMPATIBLE = 3,
        }

        /// <summary>
        /// Gets the Compatibility of the Mod or Plugin.
        /// </summary>
        public MelonCompatibility Compatibility { get; internal set; }

        public virtual void OnApplicationStart() { }
        public virtual void OnApplicationQuit() { }
        public virtual void OnModSettingsApplied() { }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonOptionalDependenciesAttribute : Attribute
    {
        /// <summary>
        /// The (simple) assembly names of the dependencies that should be regarded as optional.
        /// </summary>
        public string[] AssemblyNames { get; }

        public MelonOptionalDependenciesAttribute(params string[] assemblyNames)
        {
            AssemblyNames = assemblyNames;
        }
    }
}
