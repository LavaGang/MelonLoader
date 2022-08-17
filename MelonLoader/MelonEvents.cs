namespace MelonLoader
{
    public static class MelonEvents
    {
        /// <summary>
        /// Called after all MelonPlugins are initialized.
        /// </summary>
        public readonly static MelonEvent OnPreInitialization = new MelonEvent(true);

        /// <summary>
        /// Called after Game Initialization, before OnApplicationStart and before Assembly Generation on Il2Cpp games.
        /// </summary>
        public readonly static MelonEvent OnApplicationEarlyStart = new MelonEvent(true);

        /// <summary>
        /// Called after all MelonMods are initialized and before the right Support Module is loaded.
        /// </summary>
        public readonly static MelonEvent OnPreSupportModule = new MelonEvent(true);

        /// <summary>
        /// Called after all MelonLoader components are fully initialized (including all MelonMods).
        /// <para>Don't use this event to initialize your Melons anymore! Instead, override <see cref="MelonBase.OnInitializeMelon"/> or <see cref="MelonBase.OnLoaderInitialized"/>.</para>
        /// </summary>
        public readonly static MelonEvent OnApplicationStart = new MelonEvent(true);

        /// <summary>
        /// Called when the first 'Start' Unity Messages are invoked.
        /// </summary>
        public readonly static MelonEvent OnApplicationLateStart = new MelonEvent(true);

        /// <summary>
        /// Called before the Application is closed. It is not possible to prevent the game from closing at this point.
        /// </summary>
        public readonly static MelonEvent OnApplicationDefiniteQuit = new MelonEvent(true);

        /// <summary>
        /// Called on a quit request. It is possible to abort the request in this callback.
        /// </summary>
        public readonly static MelonEvent OnApplicationQuit = new MelonEvent();

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public readonly static VeryStupidEvent OnUpdate = new VeryStupidEvent();

        /// <summary>
        /// Called every 0.02 seconds, unless Time.fixedDeltaTime has a different Value. It is recommended to do all important Physics calculations inside this Callback.
        /// </summary>
        public readonly static VeryStupidEvent OnFixedUpdate = new VeryStupidEvent();

        /// <summary>
        /// Called once per frame, after <see cref="OnUpdate"/>.
        /// </summary>
        public readonly static VeryStupidEvent OnLateUpdate = new VeryStupidEvent();

        /// <summary>
        /// Called at every IMGUI event. Only use this for drawing IMGUI Elements.
        /// </summary>
        public readonly static VeryStupidEvent OnGUI = new VeryStupidEvent();

        /// <summary>
        /// Called when a new Scene is loaded.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MelonEvent<int, string> OnSceneWasLoaded = new MelonEvent<int, string>();

        /// <summary>
        /// Called once a Scene is initialized.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MelonEvent<int, string> OnSceneWasInitialized = new MelonEvent<int, string>();

        /// <summary>
        /// Called once a Scene unloads.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MelonEvent<int, string> OnSceneWasUnloaded = new MelonEvent<int, string>();

        /// <summary>
        /// Called before MelonMods are loaded from the Mods folder.
        /// </summary>
        public readonly static MelonEvent OnPreModsLoaded = new MelonEvent(true);

        /// <summary>
        /// Called when BONEWORKS shows the Loading Screen. Only runs if the Melon is used in BONEWORKS.
        /// </summary>
        public readonly static MelonEvent BONEWORKS_OnLoadingScreen = new MelonEvent();
    }
}
