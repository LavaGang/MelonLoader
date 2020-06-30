using System;

namespace MelonLoader
{
    public abstract class MelonPlugin : MelonBase
    {
        /// <summary>
        /// Gets the Info Attribute of the Mod or Plugin.
        /// </summary>
        public MelonPluginInfoAttribute InfoAttribute { get; internal set; }

        /// <summary>
        /// Gets the Game Attributes of the Mod or Plugin.
        /// </summary>
        public MelonPluginGameAttribute[] GameAttributes { get; internal set; }

        public virtual void OnPreInitialization() { }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MelonPluginGameAttribute : Attribute
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
        public MelonPluginGameAttribute(string developer = null, string gameName = null)
        {
            Developer = developer;
            GameName = gameName;
        }

        public bool IsGame(string developer, string gameName) => (Universal || ((developer != null) && (gameName != null) && Developer.Equals(developer) && GameName.Equals(gameName)));
        public bool IsCompatible(MelonPluginGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(GameName)));
        public bool IsCompatibleBecauseUniversal(MelonPluginGameAttribute att) => ((att == null) || Universal || att.Universal);
        public bool IsCompatible(MelonModGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(GameName)));
        public bool IsCompatibleBecauseUniversal(MelonModGameAttribute att) => ((att == null) || Universal || att.Universal);
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonPluginInfoAttribute : Attribute
    {
        /// <summary>
        /// Gets the System.Type of the Mod.
        /// </summary>
        public Type SystemType { get; }

        /// <summary>
        /// Gets the Name of the Mod.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Version of the Mod.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the Author of the Mod.
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Gets the Download Link of the Mod.
        /// </summary>
        public string DownloadLink { get; }

        public MelonPluginInfoAttribute(Type type, string name, string version, string author, string downloadLink = null)
        {
            SystemType = type;
            Name = name;
            Version = version;
            Author = author;
            DownloadLink = downloadLink;
        }
    }
}
