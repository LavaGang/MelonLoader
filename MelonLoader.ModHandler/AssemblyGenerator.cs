using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Main
    {
        private static Package BaseLibs = new Package();
        private static Executable_Package Il2CppDumper = new Executable_Package();
        private static Executable_Package Il2CppAssemblyUnhollower = new Executable_Package();
        internal static string BaseFolder = null;
        internal static string UniversalFolder = null;
        internal static string UniversalBaseLibsFolder = null;

        internal static bool Initialize()
        {
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            SetupDirectories();

            if (DownloadCheck() && !Download())
                return false;

            // Check if Extraction is Needed

            // Check if Assembly Generation is Needed

            // if so Cleanup Old Managed Files, Execute, and Cleanup

            return true;
        }

        private static void SetupDirectories()
        {
            // Setup Universal Directories
            UniversalFolder = Path.Combine(Path.GetTempPath(), "MelonLoader");
            UniversalBaseLibsFolder = Path.Combine(UniversalFolder, "BaseLibs");

            // Setup Local Game Directories
            string game_folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader");
            BaseFolder = Path.Combine(game_folder, "AssemblyGenerator");
            if (!Directory.Exists(BaseFolder))
                Directory.CreateDirectory(BaseFolder);
            
            Il2CppDumper.SetupDirectory(Path.Combine(BaseFolder, "Il2CppDumper"));
            Il2CppAssemblyUnhollower.SetupDirectory(Path.Combine(BaseFolder, "Il2CppAssemblyUnhollower"));
            BaseLibs.SetupDirectory(Path.Combine(Path.Combine(BaseFolder, "Il2CppAssemblyUnhollower"), "BaseLibs"));
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
        internal bool ShouldExtract = false;

        internal void SetupDirectory(string base_folder)
        {
            DirPath = base_folder;
            if (!Directory.Exists(DirPath))
                Directory.CreateDirectory(DirPath);
        }

        internal bool DownloadCheck()
        {

            return false;
        }

        internal bool Download()
        {

            return true;
        }
    }

    internal class Executable_Package : Package
    {
        internal bool Execute()
        {

            return true;
        }
    }
}
