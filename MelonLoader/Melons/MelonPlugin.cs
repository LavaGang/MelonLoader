namespace MelonLoader
{
    public abstract class MelonPlugin : MelonTypeBase<MelonPlugin>
    {
        static MelonPlugin()
        {
            TypeName = "Plugin";
        }

        protected private override bool RegisterInternal()
        {
            if (!base.RegisterInternal())
                return false;

            if (MelonEvents.MelonHarmonyEarlyInit.Disposed)
                HarmonyInit();
            else
                MelonEvents.MelonHarmonyEarlyInit.Subscribe(HarmonyInit, Priority, true);

            return true;
        }

        private void HarmonyInit()
        {
            if (!MelonAssembly.HarmonyDontPatchAll)
                HarmonyInstance.PatchAll(MelonAssembly.Assembly);
        }

        public override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            MelonEvents.OnPreInitialization.Subscribe(OnPreInitialization, Priority);
            MelonEvents.OnApplicationEarlyStart.Subscribe(OnApplicationEarlyStart, Priority);
            MelonEvents.OnPreModsLoaded.Subscribe(OnPreModsLoaded, Priority);

            MelonEvents.OnApplicationStart.Subscribe(OnApplicationStarted, Priority);
            MelonEvents.OnPreSupportModule.Subscribe(OnPreSupportModule, Priority);
        }

        #region Callbacks

        /// <summary>
        /// Runs before Game Initialization.
        /// </summary>
        public virtual void OnPreInitialization() { }

        /// <summary>
        /// Runs after Game Initialization, before OnApplicationStart
        /// </summary>
        public virtual void OnApplicationEarlyStart() { }

        /// <summary>
        /// Runs before MelonMods from the Mods folder are loaded.
        /// </summary>
        public virtual void OnPreModsLoaded() { }

        /// <summary>
        /// Runs after all MelonLoader components are fully initialized (including all MelonMods).
        /// </summary>
        public virtual void OnApplicationStarted() { }

        #endregion
    }
}