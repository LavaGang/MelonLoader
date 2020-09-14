using System;

namespace MelonLoader
{
    public abstract class MelonMod : MelonBase
    {
        /// <summary>
        /// Runs when a Scene has Loaded.
        /// </summary>
        public virtual void OnSceneWasLoaded(int buildIndex) { }

        /// <summary>
        /// Runs when a Scene has Initialized.
        /// </summary>
        public virtual void OnSceneWasInitialized(int buildIndex) { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Physics.
        /// </summary>
        public virtual void OnFixedUpdate() { }

        [Obsolete("OnLevelWasLoaded is obsolete. Please use OnSceneWasLoaded instead.")]
        public virtual void OnLevelWasLoaded(int level) { }
        [Obsolete("OnLevelWasInitialized is obsolete. Please use OnSceneWasInitialized instead.")]
        public virtual void OnLevelWasInitialized(int level) { }
    }
}