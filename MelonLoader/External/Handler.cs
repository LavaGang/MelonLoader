using System.Runtime.CompilerServices;

namespace MelonLoader.External
{
    class Handler
    {
        internal enum LoadMode
        {
            NORMAL,
            DEV,
            BOTH
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static LoadMode GetLoadModeForPlugins();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static LoadMode GetLoadModeForMods();
    }
}
