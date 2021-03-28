using System;
using System.Runtime.CompilerServices;

namespace MelonLoader.External
{
    class Debug
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsEnabled();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_Msg(ConsoleColor color, string namesection, string txt);
    }
}
