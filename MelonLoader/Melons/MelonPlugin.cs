using System;
using System.Collections.Generic;
#pragma warning disable 0618 // Disabling the obsolete references warning to prevent the IDE going crazy when subscribing deprecated methods to some events in RegisterCallbacks

namespace MelonLoader
{
    public abstract class MelonPlugin : MelonTypeBase<MelonPlugin>
    {
        static MelonPlugin()
        {
            TypeName = "Plugin";
        }

        protected private override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            MelonEvents.OnPreInitialization.Subscribe(OnPreInitialization, Priority);
            MelonEvents.OnApplicationEarlyStart.Subscribe(OnApplicationEarlyStart, Priority);
            MelonEvents.OnPreModsLoaded.Subscribe(OnPreModsLoaded, Priority);
            MelonEvents.OnPreModsLoaded.Subscribe(OnApplicationStart, Priority);
            MelonEvents.OnApplicationStart.Subscribe(OnApplicationStarted, Priority);
            MelonEvents.OnPreSupportModule.Subscribe(OnPreSupportModule, Priority);
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
        /// Runs before MelonMods from the Mods folder are loaded.
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
                List<MelonPluginGameAttribute> newatts = new();
                foreach (MelonGameAttribute att in Games)
                    newatts.Add(new MelonPluginGameAttribute(att.Developer, att.Name));
                _LegacyGameAttributes = newatts.ToArray();
                return _LegacyGameAttributes;
            }
        }

        #endregion
    }
}