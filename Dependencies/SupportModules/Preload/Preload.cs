using System.IO;
using System.Reflection;
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

            string systemPath = Path.Combine(managedFolder, "System.dll");
            if (!File.Exists(systemPath))
                WriteResource("System.dll", systemPath);

            string systemCorePath = Path.Combine(managedFolder, "System.Core.dll");
            if (!File.Exists(systemCorePath))
                WriteResource("System.Core.dll", systemCorePath);

            string systemDrawingPath = Path.Combine(managedFolder, "System.Drawing.dll");
            if (!File.Exists(systemDrawingPath))
                WriteResource("System.Drawing.dll", systemDrawingPath);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string GetManagedDirectory();

        private static void WriteResource(string name, string destination)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MelonLoader.Support.Resources." + name);
            var fileStream = new FileStream(destination, FileMode.CreateNew);
            for (int i = 0; i < stream!.Length; i++)
                fileStream.WriteByte((byte)stream.ReadByte());
            fileStream.Close();
        }
    }
}