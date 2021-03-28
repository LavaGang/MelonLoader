using System.Runtime.CompilerServices;

namespace MelonLoader.External
{
    class Core
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static bool QuitFix();
    }
}
