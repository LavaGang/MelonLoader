using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonProcessAttribute : Attribute
    {
        public MelonProcessAttribute(string exe_name = null)
            => EXE_Name = RemoveExtension(exe_name);

        /// <summary>
        /// Name of the Game's Executable without the '.exe' extension.
        /// </summary>
        public string EXE_Name { get; internal set; }

        /// <summary>
        /// If the Attribute is set as Universal or not.
        /// </summary>
        public bool Universal
            => string.IsNullOrEmpty(EXE_Name);

        /// <summary>
        /// Checks if the Attribute is compatible with <paramref name="processName"/> or not.
        /// </summary>
        public bool IsCompatible(string processName)
            => Universal || string.IsNullOrEmpty(processName) || (RemoveExtension(processName) == EXE_Name);

        private string RemoveExtension(string name)
            => name == null ? null : (name.EndsWith(".exe") ? name.Remove(name.Length - 4) : name);

    }
}