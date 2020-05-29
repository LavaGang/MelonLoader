using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Main
    {
        private static Package BaseLibs = new Package();
        private static Package Il2CppDumper = new Package();
        private static Package Il2CppAssemblyUnhollower = new Package();

        internal static bool Initialize()
        {
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            SetupDirectories();

            if (DownloadCheck() && !Download())
                return false;

            return true;
        }

        private static void SetupDirectories()
        {
            string game_folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader");
            string base_folder = Path.Combine(game_folder, "AssemblyGenerator");
            if (!Directory.Exists(base_folder))
                Directory.CreateDirectory(base_folder);

            Il2CppDumper.SetupDirectory(Path.Combine(base_folder, "Il2CppDumper"));
            Il2CppAssemblyUnhollower.SetupDirectory(Path.Combine(base_folder, "Il2CppAssemblyUnhollower"));
            BaseLibs.SetupDirectory(Path.Combine(Path.Combine(base_folder, "Il2CppAssemblyUnhollower"), "BaseLibs"));
        }

        private static bool Download()
        {
            if (Il2CppDumper.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Il2CppDumper ");
                if (!Il2CppDumper.Download())
                {
                    MelonModLogger.LogError("Failed to Download Il2CppDumper!");
                    return false;
                }
            }

            if (Il2CppAssemblyUnhollower.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Il2CppAssemblyUnhollower ");
                if (!Il2CppAssemblyUnhollower.Download())
                {
                    MelonModLogger.LogError("Failed to Download Il2CppAssemblyUnhollower!");
                    return false;
                }
            }

            if (BaseLibs.ShouldDownload)
            {
                MelonModLogger.Log("Downloading Unity BaseLibs for " + Imports.GetUnityFileVersion());
                if (!BaseLibs.Download())
                {
                    MelonModLogger.LogError("Failed to Download Unity BaseLibs!");
                    return false;
                }
            }

            return true;
        }

        private static bool DownloadCheck() => (Il2CppDumper.DownloadCheck() || Il2CppAssemblyUnhollower.DownloadCheck() || BaseLibs.DownloadCheck());
    }

    internal class Package
    {
        internal string DirPath = null;
        internal bool ShouldDownload = false;

        internal void SetupDirectory(string base_folder) { DirPath = base_folder; if (!Directory.Exists(DirPath)) Directory.CreateDirectory(DirPath); }

        internal bool DownloadCheck()
        {

            return false;
        }

        internal bool Download()
        {

            return true;
        }
    }
}
