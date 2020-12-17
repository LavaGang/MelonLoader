using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonGameAttribute : Attribute
    {
        public MelonGameAttribute(string developer = null, string name = null) { Developer = developer; Name = Name; }

        /// <summary>
        /// Developer of the Game.
        /// </summary>
        public string Developer { get; internal set; }

        /// <summary>
        /// Name of the Game.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// </summary>
        public bool IsUniveral() => ((Developer == null) || Developer.Equals("UNKNOWN") || (Name == null) || Name.Equals("UNKNOWN"));

        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly.
        /// </summary>
        public bool IsCompatible(string developer, string gameName) => (IsUniveral() || ((developer != null) && (gameName != null) && Developer.Equals(developer) && Name.Equals(gameName)));
        
        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly.
        /// </summary>
        public bool IsCompatible(MelonGameAttribute att) => (IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.Name.Equals(Name)));
        
        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly specifically because of Universal Compatibility.
        /// </summary>
        public bool IsCompatibleBecauseUniversal(MelonGameAttribute att) => ((att == null) || IsUniveral() || att.IsUniveral());

        [Obsolete("IsCompatible(MelonModGameAttribute) is obsolete. Please use IsCompatible(MelonGameAttribute) instead.")]
        public bool IsCompatible(MelonModGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(Name)));
        [Obsolete("IsCompatible(MelonPluginGameAttribute) is obsolete. Please use IsCompatible(MelonGameAttribute) instead.")]
        public bool IsCompatible(MelonPluginGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(Name)));
        [Obsolete("IsCompatibleBecauseUniversal(MelonModGameAttribute) is obsolete. Please use IsCompatible(MelonGameAttribute) instead.")]
        public bool IsCompatibleBecauseUniversal(MelonModGameAttribute att) => ((att == null) || ((Developer == null) || (Name == null)) || ((att.Developer == null) || (att.GameName == null)));
        [Obsolete("IsCompatibleBecauseUniversal(MelonPluginGameAttribute) is obsolete. Please use IsCompatible(MelonGameAttribute) instead.")]
        public bool IsCompatibleBecauseUniversal(MelonPluginGameAttribute att) => ((att == null) || ((Developer == null) || (Name == null)) || ((att.Developer == null) || (att.GameName == null)));
    }
}