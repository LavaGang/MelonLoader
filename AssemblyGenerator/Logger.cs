using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Logger
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Msg([MarshalAs(UnmanagedType.LPStr)] string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Warning([MarshalAs(UnmanagedType.LPStr)] string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Error([MarshalAs(UnmanagedType.LPStr)] string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Debug_Msg([MarshalAs(UnmanagedType.LPStr)] string txt);
    }
}