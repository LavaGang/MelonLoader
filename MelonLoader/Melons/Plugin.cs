using System;
using System.Collections;
using System.Collections.Generic;

namespace MelonLoader
{
    public abstract class MelonPlugin : MelonBase
    {
        /// <summary>
        /// Runs before Game Initialization.
        /// </summary>
        public virtual void OnPreInitialization() { }

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

    internal class MelonPluginEnumerator : IEnumerator<MelonPlugin>
    {
        private MelonPlugin currentPlugin = null;
        private int currentIndex = -1;
        public bool MoveNext()
        {
            if ((MelonHandler._Plugins.Count <= 0) || (++currentIndex >= MelonHandler._Plugins.Count))
                return false;
            currentPlugin = MelonHandler._Plugins[currentIndex];
            return true;
        }
        public void Reset() => currentIndex = -1;
        public MelonPlugin Current => currentPlugin;
        object IEnumerator.Current => currentPlugin;
        void IDisposable.Dispose() { }
    }
}