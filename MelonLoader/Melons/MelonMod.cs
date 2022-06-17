using System;
using System.Collections.Generic;
#pragma warning disable 0618 // Disabling the obsolete references warning to prevent the IDE going crazy when subscribing deprecated methods to some events in RegisterCallbacks

namespace MelonLoader
{
    public abstract class MelonMod : Melon<MelonMod>
    {
        static MelonMod()
        {
            TypeName = "Mod";
        }

        protected internal override bool RegisterInternal()
        {
            try
            {
                OnPreSupportModule();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to register {MelonTypeName} '{Location}': Melon failed to initialize in the deprecated OnPreSupportModule callback!");
                MelonLogger.Error(ex.ToString());
                return false;
            }

            base.RegisterInternal();
            return true;
        }

        protected internal override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            MelonEvents.OnSceneWasLoaded.Subscribe(OnSceneWasLoaded, Priority);
            MelonEvents.OnSceneWasInitialized.Subscribe(OnSceneWasInitialized, Priority);
            MelonEvents.OnSceneWasUnloaded.Subscribe(OnSceneWasUnloaded, Priority);

            MelonEvents.OnSceneWasLoaded.Subscribe((idx, name) => OnLevelWasLoaded(idx), Priority);
            MelonEvents.OnSceneWasInitialized.Subscribe((idx, name) => OnLevelWasInitialized(idx), Priority);
            MelonEvents.BONEWORKS_OnLoadingScreen.Subscribe(BONEWORKS_OnLoadingScreen, Priority);
            MelonEvents.OnApplicationStart.Subscribe(OnApplicationStart, Priority);
            MelonEvents.OnApplicationLateStart.Subscribe(OnApplicationLateStart, Priority);
        }

        #region Callbacks

        /// <summary>
        /// Runs when a new Scene is loaded.
        /// </summary>
        public virtual void OnSceneWasLoaded(int buildIndex, string sceneName) { }

        /// <summary>
        /// Runs once a Scene is initialized.
        /// </summary>
        public virtual void OnSceneWasInitialized(int buildIndex, string sceneName) { }

        /// <summary>
        /// Runs once a Scene unloads.
        /// </summary>
        public virtual void OnSceneWasUnloaded(int buildIndex, string sceneName) { }

        [Obsolete("This callback became a plugin-only callback. To initialize your Mod, override either OnInitializeMelon or OnLoaderInitialized instead.")]
        public virtual void OnPreSupportModule() { }

        [Obsolete("This callback became a plugin-only callback. To initialize your Mod, override either OnInitializeMelon or OnLoaderInitialized instead.")]
        public virtual void OnApplicationStart() { }

        [Obsolete("This callback became a plugin-only callback. To initialize your Mod, override either OnInitializeMelon or OnLoaderInitialized instead.")]
        public virtual void OnApplicationLateStart() { }

        #endregion

        #region Obsolete Members

        [Obsolete("Subscribe to the 'MelonEvents.BONEWORKS_OnLoadingScreen' event instead.")]
        public virtual void BONEWORKS_OnLoadingScreen() { }
        [Obsolete("Override OnSceneWasLoaded instead.")]
        public virtual void OnLevelWasLoaded(int level) { }
        [Obsolete("Override OnSceneWasInitialized instead.")]
        public virtual void OnLevelWasInitialized(int level) { }

        [Obsolete()]
        private MelonModInfoAttribute _LegacyInfoAttribute = null;
        [Obsolete("Use MelonBase.Info instead.")]
        public MelonModInfoAttribute InfoAttribute { get { if (_LegacyInfoAttribute == null) _LegacyInfoAttribute = new MelonModInfoAttribute(Info.SystemType, Info.Name, Info.Version, Info.Author, Info.DownloadLink); return _LegacyInfoAttribute; } }
        [Obsolete()]
        private MelonModGameAttribute[] _LegacyGameAttributes = null;
        [Obsolete("Use MelonBase.Games instead.")]
        public MelonModGameAttribute[] GameAttributes { get {
                if (_LegacyGameAttributes != null)
                    return _LegacyGameAttributes;
                List<MelonModGameAttribute> newatts = new List<MelonModGameAttribute>();
                foreach (MelonGameAttribute att in Games)
                    newatts.Add(new MelonModGameAttribute(att.Developer, att.Name));
                _LegacyGameAttributes = newatts.ToArray();
                return _LegacyGameAttributes;
            } }

        #endregion
    }
}