using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public static class MelonDebug
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsEnabled();
    }
}