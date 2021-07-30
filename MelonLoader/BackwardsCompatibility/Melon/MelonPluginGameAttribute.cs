using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonPluginGameAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonPluginGameAttribute : Attribute
    {
        [Obsolete("MelonLoader.MelonPluginGameAttribute.Developer is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame.Developer instead.")]
        public string Developer { get; }
        [Obsolete("MelonLoader.MelonPluginGameAttribute.GameName is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame.Name instead.")]
        public string GameName { get; }
        [Obsolete("MelonLoader.MelonPluginGameAttribute is Only Here for Compatibility Reasons. Please use MelonLoader.MelonGame instead.")]
        public MelonPluginGameAttribute(string developer = null, string gameName = null)
        {
            Developer = developer;
            GameName = gameName;
        }
        internal MelonGameAttribute Convert() => new MelonGameAttribute(Developer, GameName);
    }
}