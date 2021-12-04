using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonModGameAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonModGameAttribute : Attribute
    {
        [Obsolete("MelonLoader.MelonModGameAttribute.Developer is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame.Developer instead.")]
        public string Developer { get; }
        [Obsolete("MelonLoader.MelonModGameAttribute.GameName is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame.Name instead.")]
        public string GameName { get; }
        [Obsolete("MelonLoader.MelonModGameAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame instead.")]
        public MelonModGameAttribute(string developer = null, string gameName = null)
        {
            Developer = developer;
            GameName = gameName;
        }
        internal MelonGameAttribute Convert() => new MelonGameAttribute(Developer, GameName);
    }
}