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

            var ownDir = typeof(Preload).Assembly.Location;
            if (string.IsNullOrEmpty(ownDir))
                return;

            ownDir = Path.GetDirectoryName(ownDir);

            var managedFolder = GetManagedDirectory();

            var patchesDir = Path.Combine(ownDir, "NetStandardPatches");
            if (!Directory.Exists(patchesDir))
                return;

            foreach (var patch in Directory.GetFiles(patchesDir))
            {
                try
                {
                    File.Copy(patch, Path.Combine(managedFolder, Path.GetFileName(patch)), true);
                }
                catch { }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string GetManagedDirectory();
    }
}