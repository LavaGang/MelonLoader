using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public abstract class MelonPlugin : MelonBase
    {
        /// <summary>
        /// Runs before Game Initialization.
        /// </summary>
        public virtual void OnPreInitialization() { }

        /// <summary>
        /// Runs after Game Initialization, before OnApplicationStart and (on Il2Cpp games) before Unhollower
        /// </summary>
        public virtual void OnApplicationEarlyStart() { }

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
    }
}