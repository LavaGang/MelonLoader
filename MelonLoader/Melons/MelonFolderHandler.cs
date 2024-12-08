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
            List<string> pluginDirs = new();
            List<string> modDirs = new();
            FindMelonFolders(type, path, ref userLibDirs, ref pluginDirs, ref modDirs);

            // Load UserLib Assemblies
            bool hasWroteLine = false;
            List<MelonAssembly> userLibAssemblies = new();
            MelonPreprocessor.LoadFolders(userLibDirs, (type == eScanType.UserLibs), ref hasWroteLine, ref userLibAssemblies);

            if (type != eScanType.UserLibs)
            {
                // Load Plugins
                List<MelonAssembly> pluginAssemblies = new();
                MelonPreprocessor.LoadFolders(pluginDirs, true, ref hasWroteLine, ref pluginAssemblies);
                List<MelonPlugin> pluginsLoaded =
                        LoadMelons<MelonPlugin>(pluginAssemblies);

                // Load Mods
                List<MelonMod> modsLoaded = null;
                if (type == eScanType.Mods)
                {
                    List<MelonAssembly> modAssemblies = new();
                    MelonPreprocessor.LoadFolders(modDirs, true, ref hasWroteLine, ref modAssemblies);
                    modsLoaded = LoadMelons<MelonMod>(modAssemblies);
                }

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
                ? userLibAssemblies.Count
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

        private static List<T> LoadMelons<T>(List<MelonAssembly> melonAssemblies)
            where T : MelonTypeBase<T>
        {
            // Load Melons from Assembly
            foreach (var asm in melonAssemblies)
                asm.LoadMelons();

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
                    MelonLogger.Warning($"Failed to load Melon '{m.Info.Name}' from '{m.MelonAssembly.Location}': The given Melon is a {m.MelonTypeName} and cannot be loaded as a {MelonTypeBase<T>.TypeName}. Make sure it's in the right folder.");
                }
            return loadedMelons;
        }

        private static void FindMelonFolders(
            eScanType scanType,
            string path,
            ref List<string> userLibDirectories,
            ref List<string> pluginDirectories,
            ref List<string> modDirectories)
        {
            // Validate Path
            if (!Directory.Exists(path))
                return;

            // Scan Directories
            ScanFolder(scanType, path, ref userLibDirectories, ref pluginDirectories, ref modDirectories);

            if (scanType == eScanType.UserLibs)
                userLibDirectories.Add(path); 
            else
            {
                if (scanType == eScanType.Mods)
                    modDirectories.Add(path);
                else
                    pluginDirectories.Add(path);
            }

            // Add Directories to Resolver
            foreach (string directory in userLibDirectories)
            {
                MelonUtils.AddNativeDLLDirectory(directory);
                Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
            }

            if (scanType != eScanType.UserLibs)
            {
                foreach (string directory in pluginDirectories)
                    Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
                foreach (string directory in modDirectories)
                    Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
            }
        }

        private static void ScanExtraFolders(eScanType scanType,
            string path,
            ref List<string> userLibDirectories,
            ref List<string> pluginDirectories,
            ref List<string> modDirectories)
        {
            string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            if ((directories == null)
                || (directories.Length <= 0))
                return;

            foreach (var dir in directories)
            {
                if (userLibDirectories.Contains(dir)
                    || pluginDirectories.Contains(dir)
                    || modDirectories.Contains(dir))
                    continue;

                ScanExtraFolders(scanType, dir, ref userLibDirectories, ref pluginDirectories, ref modDirectories);

                // Add to Directories List
                if (scanType == eScanType.UserLibs)
                    userLibDirectories.Add(dir); 
                else if (scanType == eScanType.Plugins)
                    pluginDirectories.Add(dir);
                else
                    modDirectories.Add(dir);
            }
        }

        private static void ScanFolder(eScanType scanType,
            string path,
            ref List<string> userLibDirectories,
            ref List<string> pluginDirectories,
            ref List<string> modDirectories)
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
                    ScanFolder(eScanType.UserLibs, userLibsPath, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
                }

                // Is UserLibs Scan?
                if (scanType == eScanType.UserLibs)
                {
                    ScanExtraFolders(scanType, dir, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
                    userLibDirectories.Add(dir); // Add to Directories List
                }
                else
                {
                    // Check for Deeper Melon Folder
                    string melonPath = Path.Combine(dir, "Plugins");
                    if (Directory.Exists(melonPath))
                    {
                        pluginDirectories.Add(melonPath);
                        ScanFolder(eScanType.Plugins, melonPath, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
                    }

                    if (scanType == eScanType.Mods)
                    {
                        melonPath = Path.Combine(dir, "Mods");
                        if (Directory.Exists(melonPath))
                        {
                            modDirectories.Add(melonPath);
                            ScanFolder(scanType, melonPath, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
                        }
                    }

                    ScanExtraFolders(scanType, dir, ref userLibDirectories, ref pluginDirectories, ref modDirectories);

                    // Add to Directories List
                    if (scanType == eScanType.Mods)
                        modDirectories.Add(dir);
                    else
                        pluginDirectories.Add(dir);
                }
            }
        }
    }
}