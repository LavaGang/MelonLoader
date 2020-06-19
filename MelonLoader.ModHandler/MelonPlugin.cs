using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MelonLoader
{
    public abstract class MelonPlugin : MelonBase
    {
        /// <summary>
        /// Gets the Info Attribute of the Mod or Plugin.
        /// </summary>
        public MelonPluginInfoAttribute InfoAttribute { get; internal set; }

        /// <summary>
        /// Gets the Game Attributes of the Mod or Plugin.
        /// </summary>
        public MelonPluginGameAttribute[] GameAttributes { get; internal set; }

        public virtual void OnPreInitialization() { }
    }
}
