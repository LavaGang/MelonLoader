using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonProcessAttribute : Attribute
    {
        public MelonProcessAttribute(string exe_name = null)
            => EXE_Name = exe_name;

        /// <summary>
        /// Name of the Game's Executable.
        /// </summary>
        public string EXE_Name { get; internal set; }

        /// <summary>
        /// If the Attribute is set as Universal or not.
        /// </summary>
        public bool Universal { get => string.IsNullOrEmpty(EXE_Name); }
    }
}