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

        internal static void Scan(eScanType type, string path)
        {
            // Get Full Directory Path
            path = Path.GetFullPath(path);

            // Log Loading Message
            LogStart(path, type);

            // Find Melon Folders
            List<string> userLibDirs = new();
            List<string> melonDirs = new();
            FindMelonFolders(type, path, ref melonDirs, ref userLibDirs);

            // Load UserLib Assemblies
            bool hasWroteLine = false;
            List<MelonAssembly> melonAssemblies = new();
            MelonPreprocessor.LoadFolders(userLibDirs, (type == eScanType.UserLibs), ref hasWroteLine, ref melonAssemblies);

            if (type != eScanType.UserLibs)
            {
                // Load Melon Assemblies
                MelonPreprocessor.LoadFolders(melonDirs, true, ref hasWroteLine, ref melonAssemblies);
                PreloadMelonAssemblies(melonAssemblies);

                // Load Plugins
                List<MelonPlugin> pluginsLoaded =
                        LoadMelons<MelonPlugin>(path, (type == eScanType.Plugins), melonAssemblies);

                // Load Mods
                List<MelonMod> modsLoaded = (type == eScanType.Mods)
                    ? LoadMelons<MelonMod>(path, false, melonAssemblies)
                    : null;

                // Register and Sort Plugins
                if (hasWroteLine)
                    MelonLogger.WriteSpacer();
                MelonBase.RegisterSorted(pluginsLoaded);

                // Register and Sort Mods
                if (type == eScanType.Mods)
                    MelonBase.RegisterSorted(modsLoaded);
            }

            // Log Count
            int count = (type == eScanType.UserLibs)
                ? melonAssemblies.Count
                : ((type == eScanType.Mods) ? MelonMod.RegisteredMelons.Count : MelonPlugin.RegisteredMelons.Count);
            string typeName = (type == eScanType.UserLibs)
                ? "UserLib".MakePlural(count)
                : ((type == eScanType.Mods) ? MelonMod.TypeName.MakePlural(count) : MelonPlugin.TypeName.MakePlural(count));

            if (hasWroteLine)
                MelonLogger.WriteLine(Color.Magenta);
            MelonLogger.Msg($"{count} {typeName} loaded.");

            // Final Spacer
            if (firstSpacer || (type == eScanType.Mods))
                MelonLogger.WriteSpacer();
            firstSpacer = true;
        }

        private static void LogStart(string path, eScanType type)
        {
            string typeName = Enum.GetName(typeof(eScanType), type);
            var loadingMsg = $"Loading {typeName} from '{path}'...";
            MelonLogger.WriteSpacer();
            MelonLogger.Msg(loadingMsg);
        }

        private static void PreloadMelonAssemblies(List<MelonAssembly> melonAssemblies)
        {
            // Load Melons from Assembly
            foreach (var asm in melonAssemblies)
                asm.LoadMelons();
        }

        private static List<T> LoadMelons<T>(string path,
            bool logFailure,
            List<MelonAssembly> melonAssemblies)
            where T : MelonTypeBase<T>
        {
            List<T> loadedMelons = new();
            foreach (var asm in melonAssemblies)
                foreach (var m in asm.LoadedMelons)
                {
                    // Validate Type
                    if (m is T t)
                    {
                        loadedMelons.Add(t);
                        continue;
                    }

                    // Log Failure
                    if (logFailure)
                        MelonLogger.Warning($"Failed to load Melon '{m.Info.Name}' from '{path}': The given Melon is a {m.MelonTypeName} and cannot be loaded as a {MelonTypeBase<T>.TypeName}. Make sure it's in the right folder.");
                }
            return loadedMelons;
        }

        private static void FindMelonFolders(
            eScanType scanType,
            string path,
            ref List<string> melonDirectories,
            ref List<string> userLibDirectories)
        {
            // Validate Path
            if (!Directory.Exists(path))
                return;

            // Scan Directories
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
                    string melonPath = Path.Combine(dir, "Plugins");
                    if (Directory.Exists(melonPath))
                    {
                        melonDirectories.Add(melonPath);
                        ScanFolder(scanType, melonPath, ref melonDirectories, ref userLibDirectories);
                    }

                    if (scanType == eScanType.Mods)
                    {
                        // Check for Deeper Melon Folder
                        melonPath = Path.Combine(dir, "Mods");
                        if (Directory.Exists(melonPath))
                        {
                            melonDirectories.Add(melonPath);
                            ScanFolder(scanType, melonPath, ref melonDirectories, ref userLibDirectories);
                        }
                    }

                    // Add to Directories List
                    melonDirectories.Add(dir);
                }
            }
        }
    }
}