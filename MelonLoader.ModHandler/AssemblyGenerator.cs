using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json;
#pragma warning disable 0649

namespace MelonLoader.AssemblyGenerator
{
    internal static class Main
    {
        private static Package_BaseLibs BaseLibs = new Package_BaseLibs();
        private static Package_Il2CppDumper Il2CppDumper = new Package_Il2CppDumper();
        private static Package_Il2CppAssemblyUnhollower Il2CppAssemblyUnhollower = new Package_Il2CppAssemblyUnhollower();

        internal static bool Initialize()
        {
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            SetupDirectories();

            if (DownloadCheck() &&
                (
                    (Il2CppDumper.ShouldDownload && !Il2CppDumper.Download()) 
                    || (Il2CppAssemblyUnhollower.ShouldDownload && !Il2CppAssemblyUnhollower.Download()) 
                    || (BaseLibs.ShouldDownload && !BaseLibs.Download())
                    )
                )
                return false;

            return true;
        }

        private static void SetupDirectories()
        {
            string game_folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader");
            string base_folder = Path.Combine(game_folder, "AssemblyGenerator");
            if (!Directory.Exists(base_folder))
                Directory.CreateDirectory(base_folder);

            Il2CppDumper.SetupDirectory(base_folder);
            Il2CppAssemblyUnhollower.SetupDirectory(base_folder);
            BaseLibs.SetupDirectory(base_folder);
        }

        private static bool DownloadCheck() => (Il2CppDumper.DownloadCheck() && Il2CppAssemblyUnhollower.DownloadCheck() && BaseLibs.DownloadCheck());
    }

    internal class Package
    {
        internal string DirPath = null;
        internal bool ShouldDownload = false;
        internal virtual void SetupDirectory(string base_folder) { DirPath = base_folder; DirectoryCheck(); }
        internal void DirectoryCheck() { if (!Directory.Exists(DirPath)) Directory.CreateDirectory(DirPath); }
        internal virtual bool DownloadCheck() => false;
        internal virtual bool Download() => true;
    }

    internal class Package_BaseLibs : Package
    {
        internal override void SetupDirectory(string base_folder)
        {
            DirPath = Path.Combine(Path.Combine(base_folder, "Il2CppAssemblyUnhollower"), "BaseLibs");
            DirectoryCheck();
        }
    }

    internal class Package_Il2CppDumper : Package
    {
        internal override void SetupDirectory(string base_folder)
        {
            DirPath = Path.Combine(base_folder, "Il2CppDumper");
            DirectoryCheck();
        }
    }

    internal class Package_Il2CppAssemblyUnhollower : Package
    {
        internal override void SetupDirectory(string base_folder)
        {
            DirPath = Path.Combine(base_folder, "Il2CppAssemblyUnhollower");
            DirectoryCheck();
        }
    }
}
