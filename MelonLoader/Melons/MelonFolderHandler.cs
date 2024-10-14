﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MelonLoader.Melons
{
    internal class MelonFolderHandler
    {
        internal static void Scan<T>(string path) where T : MelonTypeBase<T>
        {
            // Get Full Directory Path
            path = Path.GetFullPath(path);

            // Log Loading Message
            var loadingMsg = $"Loading {MelonTypeBase<T>.TypeName}s from '{path}'...";
            MelonLogger.WriteSpacer();
            MelonLogger.Msg(loadingMsg);

            // Parse Folders
            bool hasWroteLine = false;
            List<MelonAssembly> melonAssemblies = new();
            ProcessFolder<T>(path, ref hasWroteLine, ref melonAssemblies);

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
            if (MelonHandler.firstSpacer || (typeof(T) == typeof(MelonMod)))
                MelonLogger.WriteSpacer();
            MelonHandler.firstSpacer = true;
        }

        private static void LoadFolder<T>(string path,
            bool addToList,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies) where T : MelonTypeBase<T>
        {
            // Get DLLs in Directory
            var files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var f in files)
            {
                // Log
                if (!hasWroteLine)
                {
                    hasWroteLine = true;
                    MelonLogger.WriteLine(Color.Magenta);
                }

                // Load Assembly
                var asm = MelonAssembly.LoadMelonAssembly(f, addToList);
                if (asm == null)
                    continue;

                // Queue Assembly for Melon Parsing
                if (addToList)
                    melonAssemblies.Add(asm);
            }
        }

        private static void ProcessFolder<T>(string path,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies) where T : MelonTypeBase<T>
        {
            // Validate Path
            if (!Directory.Exists(path))
                return;

            // Get Directories
            var directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            if ((directories != null) && (directories.Length > 0))
            {
                // Load UserLibs
                foreach (var dir in directories)
                {
                    // Validate Path
                    if (!Directory.Exists(dir))
                        continue;

                    // Skip any folders that doesn't end with or isn't equal to UserLibs
                    string dirName = new DirectoryInfo(dir).Name;
                    string dirNameLower = dirName.ToLowerInvariant();
                    if (!dirNameLower.EndsWith("userlibs"))
                        continue;

                    // Load Assemblies
                    LoadFolder<T>(dir, false, ref hasWroteLine, ref melonAssemblies);
                }

                // Load Melons from Extended Folders
                foreach (var dir in directories)
                {
                    // Validate Path
                    if (!Directory.Exists(dir))
                        continue;

                    // Skip any folders that ends with or is equal to Disabled
                    string dirName = new DirectoryInfo(dir).Name;
                    string dirNameLower = dirName.ToLowerInvariant();
                    if (dirNameLower.EndsWith("disabled"))
                        continue;

                    // Load Melons from Extended Folder
                    LoadFolder<T>(dir, true, ref hasWroteLine, ref melonAssemblies);
                }
            }

            // Load Melons from Base Path
            LoadFolder<T>(path, true, ref hasWroteLine, ref melonAssemblies);
        }
    }
}