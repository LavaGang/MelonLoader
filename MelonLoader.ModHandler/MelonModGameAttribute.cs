using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonModGameAttribute : Attribute
    {
        /// <summary>
        /// Gets the target Developer
        /// </summary>
        public string Developer { get; }

        /// <summary>
        /// Gets target Game Name
        /// </summary>
        public string GameName { get; }

        /// <summary>
        /// Gets whether this Mod can target any Game.
        /// </summary>
        public bool Universal { get => string.IsNullOrEmpty(Developer) || string.IsNullOrEmpty(GameName); }

        /// <summary>
        /// Mark this Mod as Universal or Compatible with specific Games.
        /// </summary>
        public MelonModGameAttribute(string developer = null, string gameName = null)
        {
            Developer = developer;
            GameName = gameName;
        }

        public bool IsGame(string developer, string gameName) => (Universal || ((developer != null) && (gameName != null) && Developer.Equals(developer) && GameName.Equals(gameName)));
        public bool IsCompatible(MelonModGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(GameName)));
        public bool IsCompatibleBecauseUniversal(MelonModGameAttribute att) => ((att == null) || Universal || att.Universal);
    }
}
