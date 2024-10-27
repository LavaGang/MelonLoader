using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MelonLoader.Melons
{
    internal class MelonFolderHandler
    {
        private static bool firstSpacer = false;

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
            ProcessFolder(false, path, true, ref hasWroteLine, ref melonAssemblies);
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
            ProcessFolder(isMod, path, false, ref hasWroteLine, ref melonAssemblies);

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

        private static void LoadFolder(string path,
            bool addToList,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies)
        {
            // Validate Path
            if (!Directory.Exists(path))
                return;

            // Get DLLs in Directory
            var files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var f in files)
            {
                // Ignore Native DLLs
                if (!MelonUtils.IsManagedDLL(f)) 
                    continue;

                // Log
                if (!hasWroteLine)
                {
                    hasWroteLine = true;
                    MelonLogger.WriteLine(Color.Magenta);
                }

                // Load Assembly
                var asm = MelonAssembly.LoadMelonAssembly(f, false);
                if (asm == null)
                    continue;

                // Queue Assembly for Melon Parsing
                if (addToList)
                    melonAssemblies.Add(asm);
            }
        }

        private static void ProcessFolder(bool isMod,
            string path,
            bool userLibsOnly,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies)
        {
            // Validate Path
            if (!Directory.Exists(path))
                return;

            // Scan Directories
            List<string> melonDirectories = new();
            List<string> userLibDirectories = new();
            ScanFolder(isMod, path, userLibsOnly, ref melonDirectories, ref userLibDirectories);

            // Add Base Path to End of Directories List
            if (userLibsOnly)
                userLibDirectories.Add(path);
            else
                melonDirectories.Add(path);

            // Add Directories to Resolver
            foreach (string directory in userLibDirectories)
            {
                MelonUtils.AddNativeDLLDirectory(directory);
                Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);
            }
            if (!userLibsOnly)
                foreach (string directory in melonDirectories)
                    Resolver.MelonAssemblyResolver.AddSearchDirectory(directory);

            // Load UserLibs
            foreach (var dir in userLibDirectories)
                LoadFolder(dir, false, ref hasWroteLine, ref melonAssemblies);

            // Load Melons from Folders
            if (!userLibsOnly)
                foreach (var dir in melonDirectories)
                    LoadFolder(dir, true, ref hasWroteLine, ref melonAssemblies);
        }

        private static void ScanFolder(bool isMod,
            string path,
            bool userLibsOnly,
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
                if (!Directory.Exists(dir)
                    || userLibDirectories.Contains(dir)
                    || melonDirectories.Contains(dir))
                    continue;

                // Validate Folder
                if (IsDisabledFolder(dir, out string dirNameLower))
                    continue;

                // Check for UserLibs
                if (userLibsOnly || IsUserLibsFolder(dirNameLower))
                    userLibDirectories.Add(dir);
                else
                    melonDirectories.Add(dir);

                ScanFolder(isMod, dir, userLibsOnly, ref melonDirectories, ref userLibDirectories);
            }
        }

        private static bool StartsOrEndsWith(string dirNameLower, string target)
           => dirNameLower.StartsWith(target)
               || dirNameLower.EndsWith(target);

        private static bool IsUserLibsFolder(string dirNameLower)
            => StartsOrEndsWith(dirNameLower, "userlibs");

        private static bool IsDisabledFolder(string path,
            out string dirNameLower)
        {
            string dirName = new DirectoryInfo(path).Name;
            dirNameLower = dirName.ToLowerInvariant();
            return StartsOrEndsWith(dirNameLower, "disabled")
                || StartsOrEndsWith(dirNameLower, "old")
                || StartsOrEndsWith(dirNameLower, "~");
        }
    }
}