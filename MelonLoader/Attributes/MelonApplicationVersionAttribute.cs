using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonApplicationVersionAttribute : Attribute
    {
        public MelonApplicationVersionAttribute(string version = null)
            => Version = version;

        /// <summary>
        /// Version of the Game.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// If the Attribute is set as Universal or not.
        /// </summary>
        public bool Universal { get => string.IsNullOrEmpty(Version); }
    }
}