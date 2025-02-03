using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MelonLoader.Melons
{
    internal class ModuleFolderHandler
    {
        private static bool firstSpacer = false;
        private static List<string> userLibDirs = new();
        private static List<string> pluginDirs = new();
        private static List<string> modDirs = new();

        internal enum eScanType
        {
            UserLibs,
            Plugins,
            Mods,
        }

        internal static void ScanForFolders()
        {
            // Add Base Directories to Start of List
            userLibDirs.Add(MelonEnvironment.UserLibsDirectory);
            pluginDirs.Add(MelonEnvironment.PluginsDirectory);
            modDirs.Add(MelonEnvironment.ModsDirectory);

            // Scan Base Folders
            ScanFolder(eScanType.UserLibs, MelonEnvironment.UserLibsDirectory, ref userLibDirs, ref pluginDirs, ref modDirs);
            ScanFolder(eScanType.Plugins, MelonEnvironment.PluginsDirectory, ref userLibDirs, ref pluginDirs, ref modDirs);
            ScanFolder(eScanType.Mods, MelonEnvironment.ModsDirectory, ref userLibDirs, ref pluginDirs, ref modDirs);

            // Add Directories to Resolver
            foreach (string directory in userLibDirs)
            {
                OsUtils.AddNativeDLLDirectory(directory);
                Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
            }
            foreach (string directory in pluginDirs)
                Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
            foreach (string directory in modDirs)
                Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
        }

        internal static void LoadMelons(eScanType type)
        {
            LogStart(type);

            bool hasWroteLine = false;
            List<MelonAssembly> melonAssemblies = new();

            if (type == eScanType.UserLibs)
                MelonPreprocessor.LoadFolders(userLibDirs, false, ref hasWroteLine, ref melonAssemblies);

            int count = melonAssemblies.Count;
            string typeName = "UserLib".MakePlural(count);

            if (type == eScanType.Plugins)
            {
                MelonPreprocessor.LoadFolders(pluginDirs, true, ref hasWroteLine, ref melonAssemblies);
                List<MelonPlugin> pluginsLoaded = LoadMelons<MelonPlugin>(melonAssemblies);
                if (hasWroteLine)
                    MelonLogger.WriteSpacer();
                MelonBase.RegisterSorted(pluginsLoaded);

                count = MelonPlugin.RegisteredMelons.Count;
                typeName = MelonPlugin.TypeName.MakePlural(count);
            }

            if (type == eScanType.Mods)
            {
                MelonPreprocessor.LoadFolders(modDirs, true, ref hasWroteLine, ref melonAssemblies);
                List<MelonMod> modsLoaded = LoadMelons<MelonMod>(melonAssemblies);
                if (hasWroteLine)
                    MelonLogger.WriteSpacer();
                MelonBase.RegisterSorted(modsLoaded);

                count = MelonMod.RegisteredMelons.Count;
                typeName = MelonMod.TypeName.MakePlural(count);
            }

            if (hasWroteLine)
                MelonLogger.WriteLine(Color.Magenta);
            MelonLogger.Msg($"{count} {typeName} loaded.");

            if (firstSpacer || (type == eScanType.Mods))
                MelonLogger.WriteSpacer();
            firstSpacer = true;
        }

        private static void LogStart(eScanType type)
        {
            string typeName = Enum.GetName(typeof(eScanType), type);
            var loadingMsg = $"Loading {typeName}...";
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

                // Check for Deeper UserLibs
                string userLibsPath = Path.Combine(dir, "UserLibs");
                if (Directory.Exists(userLibsPath))
                {
                    userLibDirectories.Add(userLibsPath);
                    ScanFolder(eScanType.UserLibs, userLibsPath, ref userLibDirectories, ref pluginDirectories, ref modDirectories);
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