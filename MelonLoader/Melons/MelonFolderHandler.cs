using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MelonLoader.Melons
{
    internal class MelonFolderHandler
    {
        private static bool firstSpacer = false;

        internal enum eScanType
        {
            UserLibs,
            Plugins,
            Mods,
        }

        internal static void ScanUserLibs(string path)
        {
            // Get Full Directory Path
            path = Path.GetFullPath(path);

            // Log Loading Message
            var loadingMsg = $"Loading UserLibs from '{path}'...";
            MelonLogger.WriteSpacer();
            MelonLogger.Msg(loadingMsg);

            // Parse Folders
            bool hasWroteLine = false;
            List<MelonAssembly> melonAssemblies = new();
            ProcessFolder(eScanType.UserLibs, path, ref hasWroteLine, ref melonAssemblies);
        }

        internal static void ScanMelons<T>(string path) where T : MelonTypeBase<T>
        {
            // Get Full Directory Path
            path = Path.GetFullPath(path);

            // Log Loading Message
            var loadingMsg = $"Loading {MelonTypeBase<T>.TypeName}s from '{path}'...";
            MelonLogger.WriteSpacer();
            MelonLogger.Msg(loadingMsg);

            // Parse Folders
            Type melonType = typeof(T);
            bool isMod = melonType == typeof(MelonMod);
            bool hasWroteLine = false;
            List<MelonAssembly> melonAssemblies = new();
            ProcessFolder(isMod ? eScanType.Mods : eScanType.Plugins, path, ref hasWroteLine, ref melonAssemblies);

            // Parse Queue
            var melons = new List<T>();
            foreach (var asm in melonAssemblies)
            {
                // Load Melons from Assembly
                asm.LoadMelons();

                // Parse Loaded Melons
                foreach (var m in asm.LoadedMelons)
                {
                    // Validate Type
                    if (m is T t)
                    {
                        melons.Add(t);
                        continue;
                    }

                    // Log Failure
                    MelonLogger.Warning($"Failed to load Melon '{m.Info.Name}' from '{path}': The given Melon is a {m.MelonTypeName} and cannot be loaded as a {MelonTypeBase<T>.TypeName}. Make sure it's in the right folder.");
                }
            }

            // Log
            if (hasWroteLine)
                MelonLogger.WriteSpacer();

            // Register and Sort Melons
            MelonBase.RegisterSorted(melons);

            // Log
            if (hasWroteLine)
                MelonLogger.WriteLine(Color.Magenta);

            // Log Melon Count
            var count = MelonTypeBase<T>._registeredMelons.Count;
            MelonLogger.Msg($"{count} {MelonTypeBase<T>.TypeName.MakePlural(count)} loaded.");
            if (firstSpacer || (typeof(T) == typeof(MelonMod)))
                MelonLogger.WriteSpacer();
            firstSpacer = true;
        }

        private static void ProcessFolder(eScanType scanType,
            string path,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies)
        {
            // Validate Path
            if (!Directory.Exists(path))
                return;

            // Scan Directories
            List<string> melonDirectories = new();
            List<string> userLibDirectories = new();
            ScanFolder(scanType, path, ref melonDirectories, ref userLibDirectories);

            // Add Base Path to End of Directories List
            if (scanType == eScanType.UserLibs)
                userLibDirectories.Add(path);
            else
                melonDirectories.Add(path);

            // Add Directories to Resolver
            foreach (string directory in userLibDirectories)
            {
                MelonUtils.AddNativeDLLDirectory(directory);
                Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
            }
            if (scanType != eScanType.UserLibs)
                foreach (string directory in melonDirectories)
                    Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);

            // Load UserLibs
            MelonPreprocessor.LoadFolders(userLibDirectories, false, ref hasWroteLine, ref melonAssemblies);

            // Load Melons from Folders
            if (scanType != eScanType.UserLibs)
                MelonPreprocessor.LoadFolders(melonDirectories, true, ref hasWroteLine, ref melonAssemblies);
        }

        private static void ScanFolder(eScanType scanType,
            string path,
            ref List<string> melonDirectories,
            ref List<string> userLibDirectories)
        {
            // Get Directories
            string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            if ((directories == null)
                || (directories.Length <= 0))
                return;

            // Parse Directories
            foreach (var dir in directories)
            {
                // Validate Path
                if (!Directory.Exists(dir))
                    continue;

                // Validate Manifest
                string manifestPath = Path.Combine(dir, "manifest.json");
                if (!File.Exists(manifestPath))
                    continue;

                // Check for Deeper UserLibs
                string userLibsPath = Path.Combine(dir, "UserLibs");
                if (Directory.Exists(userLibsPath))
                {
                    userLibDirectories.Add(userLibsPath);
                    ScanFolder(eScanType.UserLibs, userLibsPath, ref melonDirectories, ref userLibDirectories);
                }

                // Is UserLibs Scan?
                if (scanType == eScanType.UserLibs)
                    userLibDirectories.Add(dir); // Add to Directories List
                else
                {
                    // Check for Deeper Melon Folder
                    string melonPath = Path.Combine(dir, (scanType == eScanType.Plugins) ? "Plugins" : "Mods");
                    if (Directory.Exists(melonPath))
                    {
                        melonDirectories.Add(melonPath);
                        ScanFolder(scanType, melonPath, ref melonDirectories, ref userLibDirectories);
                    }

                    // Add to Directories List
                    melonDirectories.Add(dir);
                }
            }
        }
    }
}