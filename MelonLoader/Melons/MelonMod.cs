using System;
using System.Collections.Generic;
using MelonLoader.Attributes;
using MelonLoader.BackwardsCompatibility.Melon;

namespace MelonLoader.Melons
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
        /// Runs when a Scene has Unloaded and is passed the Scene's Build Index and Name.
        /// </summary>
        public virtual void OnSceneWasUnloaded(int buildIndex, string sceneName) { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Physics.
        /// </summary>
        public virtual void OnFixedUpdate() { }

        /// <summary>
        /// Runs when BONEWORKS shows the Loading Screen. Only runs if the Melon is used in BONEWORKS.
        /// </summary>
        public virtual void BONEWORKS_OnLoadingScreen() { }

        [Obsolete("OnLevelWasLoaded is obsolete. Please use OnSceneWasLoaded instead.")]
        public virtual void OnLevelWasLoaded(int level) { }
        [Obsolete("OnLevelWasInitialized is obsolete. Please use OnSceneWasInitialized instead.")]
        public virtual void OnLevelWasInitialized(int level) { }

        [Obsolete]
        private MelonModInfoAttribute _LegacyInfoAttribute;
        [Obsolete("MelonMod.InfoAttribute is obsolete. Please use MelonBase.Info instead.")]
        public MelonModInfoAttribute InfoAttribute { get { if (_LegacyInfoAttribute == null) _LegacyInfoAttribute = new MelonModInfoAttribute(Info.SystemType, Info.Name, Info.Version, Info.Author, Info.DownloadLink); return _LegacyInfoAttribute; } }
        [Obsolete]
        private MelonModGameAttribute[] _LegacyGameAttributes;
        [Obsolete("MelonMod.GameAttributes is obsolete. Please use MelonBase.Games instead.")]
        public MelonModGameAttribute[] GameAttributes { get {
                if (_LegacyGameAttributes != null)
                    return _LegacyGameAttributes;
                List<MelonModGameAttribute> newatts = new List<MelonModGameAttribute>();
                foreach (MelonGameAttribute att in Games)
                    newatts.Add(new MelonModGameAttribute(att.Developer, att.Name));
                _LegacyGameAttributes = newatts.ToArray();
                return _LegacyGameAttributes;
            } }
    }
}