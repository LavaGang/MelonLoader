using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Delegate)]
    public class MelonNativeLibraryImportAttribute : Attribute
    {
        public MelonNativeLibraryImportAttribute(string name) { Name = name; }

        /// <summary>
        /// Name of the Export.
        /// </summary>
        public string Name { get; internal set; }
    }
}