using System;
using System.Runtime.CompilerServices;

namespace MelonLoader.External
{
    class Logger
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_Msg(ConsoleColor color, string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_Warning(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_Error(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void ThrowInternalFailure(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void WriteSpacer();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_PrintModName(ConsoleColor color, string name, string version);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Flush();
    }
}
