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
        /// Gets whether this mod can target any game
        /// </summary>
        public bool Universal
        {
            get => Developer == null && GameName == null;
        }

        /// <summary>
        /// Mark this game as compatible with any game
        /// </summary>
        public MelonModGameAttribute()
        {
            Developer = null;
            GameName = null;
        }

        public MelonModGameAttribute(string developer, string gameName)
        {
            Developer = developer ?? throw new ArgumentNullException(nameof(developer));
            GameName = gameName ?? throw new ArgumentNullException(nameof(gameName));
        }
    }
}
