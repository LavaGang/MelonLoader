using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader
{
    internal static class BootstrapInterop
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string Internal_GetHashCode();

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[return: MarshalAs(UnmanagedType.LPStr)]
        internal /*extern */static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameName, [MarshalAs(UnmanagedType.LPStr)] string GameVersion = null)
        {
            MelonLogger.Warning($"TODO: SetDefaultConsoleTitleWithGameName({GameName}, {GameVersion})");
        }
    }
}
