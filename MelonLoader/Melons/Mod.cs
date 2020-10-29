using System;

namespace MelonLoader
{
    public abstract class MelonMod : MelonBase
    {
        /// <summary>
        /// Runs when a Scene has Loaded and is passed the Scene's Build Index and Name.
        /// </summary>
        public virtual void OnSceneWasLoaded(int buildIndex, string sceneName) { }

        /// <summary>
        /// Runs when a Scene has Initialized and is passed the Scene's Build Index and Name.
        /// </summary>
        public virtual void OnSceneWasInitialized(int buildIndex, string sceneName) { }

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