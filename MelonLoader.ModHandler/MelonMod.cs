namespace MelonLoader
{
    public abstract class MelonMod : MelonBase
    {
        /// <summary>
        /// Gets the Info Attribute of the Mod or Plugin.
        /// </summary>
        public MelonModInfoAttribute InfoAttribute { get; internal set; }

        /// <summary>
        /// Gets the Game Attributes of the Mod or Plugin.
        /// </summary>
        public MelonModGameAttribute[] GameAttributes { get; internal set; }

        public virtual void OnLevelIsLoading() {}
        public virtual void OnLevelWasLoaded(int level) {}
        public virtual void OnLevelWasInitialized(int level) {}
        public virtual void OnUpdate() {}
        public virtual void OnFixedUpdate() {}
        public virtual void OnLateUpdate() {}
        public virtual void OnGUI() {}
        public virtual void VRChat_OnUiManagerInit() {}
    }
}