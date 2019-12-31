namespace MelonLoader
{
    public abstract class MelonMod
    {
        /// <summary>
        /// Gets the Name of the Mod.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the Version of the Mod.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// Gets the Author of the Mod.
        /// </summary>
        public string Author { get; internal set; }

        /// <summary>
        /// Gets the Download Link of the Mod.
        /// </summary>
        public string DownloadLink { get; internal set; }
    }
}