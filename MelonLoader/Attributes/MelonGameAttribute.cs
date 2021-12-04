using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonGameAttribute : Attribute
    {
        public MelonGameAttribute(string developer = null, string name = null) { Developer = developer; Name = name; }

        /// <summary>
        /// Developer of the Game.
        /// </summary>
        public string Developer { get; internal set; }

        /// <summary>
        /// Name of the Game.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// If the Attribute is set as Universal or not.
        /// </summary>
        public bool Universal { get => (string.IsNullOrEmpty(Developer) || Developer.Equals("UNKNOWN") || string.IsNullOrEmpty(Name) || Name.Equals("UNKNOWN")); }

        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly.
        /// </summary>
        public bool IsCompatible(string developer, string gameName) => (Universal || (!string.IsNullOrEmpty(developer) && Developer.Equals(developer) && !string.IsNullOrEmpty(gameName) && Name.Equals(gameName)));
        
        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly.
        /// </summary>
        public bool IsCompatible(MelonGameAttribute att) => (IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.Name.Equals(Name)));
        
        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly specifically because of Universal Compatibility.
        /// </summary>
        public bool IsCompatibleBecauseUniversal(MelonGameAttribute att) => ((att == null) || Universal || att.Universal);

        [Obsolete("IsCompatible(MelonModGameAttribute) is obsolete. Please use IsCompatible(MelonGameAttribute) instead.")]
        public bool IsCompatible(MelonModGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(Name)));
        [Obsolete("IsCompatible(MelonPluginGameAttribute) is obsolete. Please use IsCompatible(MelonGameAttribute) instead.")]
        public bool IsCompatible(MelonPluginGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(Name)));
        [Obsolete("IsCompatibleBecauseUniversal(MelonModGameAttribute) is obsolete. Please use IsCompatible(MelonGameAttribute) instead.")]
        public bool IsCompatibleBecauseUniversal(MelonModGameAttribute att) => ((att == null) || Universal || (string.IsNullOrEmpty(att.Developer) || string.IsNullOrEmpty(att.GameName)));
        [Obsolete("IsCompatibleBecauseUniversal(MelonPluginGameAttribute) is obsolete. Please use IsCompatible(MelonGameAttribute) instead.")]
        public bool IsCompatibleBecauseUniversal(MelonPluginGameAttribute att) => ((att == null) || Universal || (string.IsNullOrEmpty(att.Developer) || string.IsNullOrEmpty(att.GameName)));
    }
}