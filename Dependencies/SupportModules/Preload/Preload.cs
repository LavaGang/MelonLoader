using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader.Support
{
    internal static class Preload
    {
        private static void Initialize()
        {
            string ManagedFolder = string.Copy(GetManagedDirectory());

            string SystemPath = Path.Combine(ManagedFolder, "System.dll");
            if (!File.Exists(SystemPath))
                File.WriteAllBytes(SystemPath, Properties.Resources.System);

            string SystemCorePath = Path.Combine(ManagedFolder, "System.Core.dll");
            if (!File.Exists(SystemCorePath))
                File.WriteAllBytes(SystemCorePath, Properties.Resources.System_Core);

            string SystemDrawingPath = Path.Combine(ManagedFolder, "System.Drawing.dll");
            if (!File.Exists(SystemDrawingPath))
                File.WriteAllBytes(SystemDrawingPath, Properties.Resources.System_Drawing);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string GetManagedDirectory();
    }
}