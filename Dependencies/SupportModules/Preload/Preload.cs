using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System;

namespace MelonLoader.Support
{
    internal static class Preload
    {
        private static void Initialize()
        {
            if (Environment.Version >= new Version("3.0.0.0"))
                return;

            string managedFolder = string.Copy(GetManagedDirectory());

            WriteResource(Properties.Resources.System, Path.Combine(managedFolder, "System.dll"));
            WriteResource(Properties.Resources.System_Core, Path.Combine(managedFolder, "System.Core.dll"));
            WriteResource(Properties.Resources.System_Drawing, Path.Combine(managedFolder, "System.Drawing.dll"));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string GetManagedDirectory();

        private static void WriteResource(byte[] data, string destination)
        {
            try
            {
                if (File.Exists(destination))
                    File.Delete(destination);
                File.WriteAllBytes(destination, data);
            }
            catch { }
        }
    }
}