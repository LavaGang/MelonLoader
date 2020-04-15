namespace MelonLoader
{
    public abstract class MelonMod
    {
        /// <summary>
        /// Gets if the Mod is Universal or not.
        /// </summary>
        public bool IsUniversal { get; internal set; }

        /// <summary>
        /// Gets the Info Attribute of the Mod.
        /// </summary>
        public MelonModInfoAttribute InfoAttribute { get; internal set; }

        /// <summary>
        /// Gets the Game Attributes of the Mod.
        /// </summary>
        public MelonModGameAttribute[] GameAttributes { get; internal set; }
    }
}