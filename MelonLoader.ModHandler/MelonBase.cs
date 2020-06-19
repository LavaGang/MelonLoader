using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MelonLoader
{
    public abstract class MelonBase
    {
        /// <summary>
        /// Gets if the Mod or Plugin is Universal or not.
        /// </summary>
        public bool IsUniversal { get; internal set; }

        /// <summary>
        /// Gets the Info Attribute of the Mod or Plugin.
        /// </summary>
        public MelonModInfoAttribute InfoAttribute { get; internal set; }

        /// <summary>
        /// Gets the Game Attributes of the Mod or Plugin.
        /// </summary>
        public MelonModGameAttribute[] GameAttributes { get; internal set; }

        /// <summary>
        /// Gets the Assembly of the Mod or Plugin.
        /// </summary>
        public System.Reflection.Assembly ModAssembly { get; internal set; }

        /// <summary>
        /// Gets the File Location of the Mod or Plugin.
        /// </summary>
        public string Location { get; internal set; }

        public virtual void OnApplicationStart() { }
        public virtual void OnApplicationQuit() { }
        public virtual void OnModSettingsApplied() { }
    }
}
