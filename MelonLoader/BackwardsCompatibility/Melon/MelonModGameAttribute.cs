using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonModGameAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonModGameAttribute : MelonGameAttribute
    {
        [Obsolete("MelonLoader.MelonModGameAttribute.Developer is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame.Developer instead.")]
        new public string Developer => base.Developer;
        [Obsolete("MelonLoader.MelonModGameAttribute.GameName is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame.Name instead.")]
        public string GameName => Name;
        [Obsolete("MelonLoader.MelonModGameAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame instead.")]
        public MelonModGameAttribute(string developer = null, string gameName = null) : base(developer, gameName) { }
    }
}