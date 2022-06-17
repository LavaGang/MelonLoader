using System;
using System.Collections.Generic;
#pragma warning disable 0618 // Disabling the obsolete references warning to prevent the IDE going crazy when subscribing deprecated methods to some events in RegisterCallbacks

namespace MelonLoader
{
    public abstract class MelonPlugin : Melon<MelonPlugin>
    {
        static MelonPlugin()
        {
            TypeName = "Plugin";
        }

        protected internal override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            MelonEvents.OnPreInitialization.Subscribe(OnPreInitialization, Priority);
            MelonEvents.OnApplicationEarlyStart.Subscribe(OnApplicationEarlyStart, Priority);
            MelonEvents.OnPreModsLoaded.Subscribe(OnPreModsLoaded, Priority);
            MelonEvents.OnPreModsLoaded.Subscribe(OnApplicationStart, Priority);
            MelonEvents.OnApplicationStart.Subscribe(OnApplicationStarted, Priority);
            MelonEvents.OnApplicationLateStart.Subscribe(OnApplicationLateStart, Priority);
            MelonEvents.OnPreSupportModule.Subscribe(OnPreSupportModule, Priority);
        }

        #region Callbacks

        /// <summary>
        /// Runs before Game Initialization.
        /// </summary>
        public virtual void OnPreInitialization() { }

        /// <summary>
        /// Runs after Game Initialization, before OnApplicationStart and before Assembly Generation on Il2Cpp games
        /// </summary>
        public virtual void OnApplicationEarlyStart() { }

        /// <summary>
        /// Runs before MelonMods are loaded from the Mods folder.
        /// </summary>
        public virtual void OnPreModsLoaded() { }

        /// <summary>
        /// Runs after all MelonLoader components are fully initialized (including all MelonMods).
        /// </summary>
        public virtual void OnApplicationStarted() { }

        #endregion

        #region Obsolete Members

        [Obsolete()]
        private MelonPluginInfoAttribute _LegacyInfoAttribute = null;
        [Obsolete("MelonPlugin.InfoAttribute is obsolete. Please use MelonBase.Info instead.")]
        public MelonPluginInfoAttribute InfoAttribute { get { if (_LegacyInfoAttribute == null) _LegacyInfoAttribute = new MelonPluginInfoAttribute(Info.SystemType, Info.Name, Info.Version, Info.Author, Info.DownloadLink); return _LegacyInfoAttribute; } }
        [Obsolete()]
        private MelonPluginGameAttribute[] _LegacyGameAttributes = null;
        [Obsolete("MelonPlugin.GameAttributes is obsolete. Please use MelonBase.Games instead.")]
        public MelonPluginGameAttribute[] GameAttributes
        {
            get
            {
                if (_LegacyGameAttributes != null)
                    return _LegacyGameAttributes;
                List<MelonPluginGameAttribute> newatts = new List<MelonPluginGameAttribute>();
                foreach (MelonGameAttribute att in Games)
                    newatts.Add(new MelonPluginGameAttribute(att.Developer, att.Name));
                _LegacyGameAttributes = newatts.ToArray();
                return _LegacyGameAttributes;
            }
        }

        #endregion
    }
}