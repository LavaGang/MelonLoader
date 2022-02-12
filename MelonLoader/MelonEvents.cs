namespace MelonLoader
{
    public static class MelonEvents
    {
        /// <summary>
        /// Occurs after all MelonPlugins are initialized.
        /// </summary>
        public readonly static MelonEvent OnPreInitialization = new MelonEvent(true);

        /// <summary>
        /// Occurs before the Start Screen is loaded.
        /// </summary>
        public readonly static MelonEvent OnApplicationEarlyStart = new MelonEvent(true);

        /// <summary>
        /// Occurs after all MelonMods are initialized and before the right Support Module is loaded.
        /// </summary>
        public readonly static MelonEvent OnPreSupportModule = new MelonEvent(true);

        /// <summary>
        /// Occurs after all MelonLoader components are fully initialized (including all MelonMods).
        /// <para>Don't use this event to initialize your mods anymore! Instead, override <see cref="MelonBase.OnInitializeMelon"/>.</para>
        /// </summary>
        public readonly static MelonEvent OnApplicationStart = new MelonEvent(true);

        /// <summary>
        /// Occurs when the Engine is fully initialized.
        /// </summary>
        public readonly static MelonEvent OnApplicationLateStart = new MelonEvent(true);

        /// <summary>
        /// Occurs when... well, the name pretty much says it.
        /// </summary>
        public readonly static MelonEvent OnApplicationQuit = new MelonEvent(true);

        /// <summary>
        /// Occurs once per frame.
        /// </summary>
        public readonly static MelonEvent OnUpdate = new MelonEvent();

        /// <summary>
        /// Occurs every 0.02 seconds, unless Time.fixedDeltaTime has a different Value. It is recommended to do all important Physics calculations inside this Callback.
        /// </summary>
        public readonly static MelonEvent OnFixedUpdate = new MelonEvent();

        /// <summary>
        /// Occurs once per frame, after <see cref="OnUpdate"/>.
        /// </summary>
        public readonly static MelonEvent OnLateUpdate = new MelonEvent();

        /// <summary>
        /// Occurs at every IMGUI event. Only use this for drawing IMGUI Elements.
        /// </summary>
        public readonly static MelonEvent OnGUI = new MelonEvent();

        /// <summary>
        /// Occurs when a new Scene is loaded.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MelonEvent<int, string> OnSceneWasLoaded = new MelonEvent<int, string>();

        /// <summary>
        /// Occurs once a Scene is initialized.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MelonEvent<int, string> OnSceneWasInitialized = new MelonEvent<int, string>();

        /// <summary>
        /// Occurs once a Scene unloads.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MelonEvent<int, string> OnSceneWasUnloaded = new MelonEvent<int, string>();

        /// <summary>
        /// Occurs when BONEWORKS shows the Loading Screen. Only runs if the Melon is used in BONEWORKS.
        /// </summary>
        public readonly static MelonEvent BONEWORKS_OnLoadingScreen = new MelonEvent();
    }
}
