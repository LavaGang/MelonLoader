using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonGameAttribute : Attribute
    {
        /// <summary>
        /// Developer of the Game.
        /// </summary>
        public string Developer { get; internal set; }

        /// <summary>
        /// Name of the Game.
        /// </summary>
        public string Name { get; internal set; }

        public MelonGameAttribute(string developer = null, string name = null) { Developer = developer; Name = Name; }
    }
}