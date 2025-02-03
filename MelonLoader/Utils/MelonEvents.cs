namespace MelonLoader
{
    public class MelonEvents
    {
        /// <summary>
        /// Called after all MelonPlugins are initialized.
        /// </summary>
        public readonly static MelonEvent OnPreInitialization = new(true);

        /// <summary>
        /// Called after Game Initialization, before OnApplicationStart.
        /// </summary>
        public readonly static MelonEvent OnApplicationEarlyStart = new(true);

        /// <summary>
        /// Called after all MelonMods are initialized and right before the Engine Support Module is Initialized.
        /// </summary>
        public readonly static MelonEvent OnPreSupportModule = new(true);

        /// <summary>
        /// Called after all MelonLoader components are fully initialized (including all MelonMods).
        /// <para>Don't use this event to initialize your Melons anymore! Instead, override <see cref="MelonBase.OnInitializeMelon"/>.</para>
        /// </summary>
        public readonly static MelonEvent OnApplicationStart = new(true);

        /// <summary>
        /// Called when the first 'Start' Unity Messages are invoked.
        /// </summary>
        public readonly static MelonEvent OnApplicationLateStart = new(true);

        /// <summary>
        /// Called before the Application is closed. It is not possible to prevent the game from closing at this point.
        /// </summary>
        public readonly static MelonEvent OnApplicationDefiniteQuit = new(true);

        /// <summary>
        /// Called on a quit request. It is possible to abort the request in this callback.
        /// </summary>
        public readonly static MelonEvent OnApplicationQuit = new();

        /// <summary>
        /// Called before MelonMods are loaded from the Mods folder.
        /// </summary>
        public readonly static MelonEvent OnPreModsLoaded = new(true);

        internal readonly static MelonEvent MelonHarmonyEarlyInit = new(true);
        internal readonly static MelonEvent MelonHarmonyInit = new(true);
    }
}
