using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MelonLoader
{
    public static class MelonEvents
    {
        /// <summary>
        /// This Event is invoked after all MelonPlugins are initialized.
        /// </summary>
        public readonly static MelonEvent OnPreInitialization = new MelonEvent(true);

        /// <summary>
        /// This Event is invoked before the Start Screen is loaded.
        /// </summary>
        public readonly static MelonEvent OnApplicationEarlyStart = new MelonEvent(true);

        /// <summary>
        /// This Event is invoked after all MelonMods are initialized and before the right Support Module is loaded.
        /// </summary>
        public readonly static MelonEvent OnPreSupportModule = new MelonEvent(true);

        /// <summary>
        /// This Event is invoked after all MelonLoader components are fully initialized (including all MelonMods).
        /// <para>Don't use this event to initialize your mods anymore! Instead, override <see cref="MelonBase.OnInitializeMelon"/>.</para>
        /// </summary>
        public readonly static MelonEvent OnApplicationStart = new MelonEvent(true);

        /// <summary>
        /// This Event is invoked once the Engine is fully initialized.
        /// </summary>
        public readonly static MelonEvent OnApplicationLateStart = new MelonEvent(true);

        /// <summary>
        /// This Event is invoked before... well, the name pretty much says it.
        /// </summary>
        public readonly static MelonEvent OnApplicationQuit = new MelonEvent(true);
    }
}
