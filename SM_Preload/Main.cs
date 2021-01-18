using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader.Support
{
    internal static class Preload
    {
        private static void Initialize()
        {
            if (IsGameIl2Cpp() || !IsOldMono())
                return;

            string SystemCorePath = Path.Combine(GetManagedDirectory(), "System.Core.dll");
            if (!File.Exists(SystemCorePath))
                File.WriteAllBytes(SystemCorePath, Properties.Resources.System_Core);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsOldMono();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsGameIl2Cpp();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string GetManagedDirectory();
    }
}