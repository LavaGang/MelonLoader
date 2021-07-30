using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.Support.Properties;

namespace MelonLoader.Support
{
    internal static class Preload
    {
        private static void Initialize()
        {
            string ManagedFolder = string.Copy(GetManagedDirectory());

            string SystemPath = Path.Combine(ManagedFolder, "System.dll");
            if (!File.Exists(SystemPath))
                File.WriteAllBytes(SystemPath, Resources.System);

            string SystemCorePath = Path.Combine(ManagedFolder, "System.Core.dll");
            if (!File.Exists(SystemCorePath))
                File.WriteAllBytes(SystemCorePath, Resources.System_Core);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string GetManagedDirectory();
    }
}