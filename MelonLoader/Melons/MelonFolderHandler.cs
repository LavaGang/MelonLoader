using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MelonLoader.Melons
{
    internal class MelonFolderHandler
    {
        internal static void Scan<T>(string path) where T : MelonTypeBase<T>
        {
            path = Path.GetFullPath(path);

            var loadingMsg = $"Loading {MelonTypeBase<T>.TypeName}s from '{path}'...";
            MelonLogger.WriteSpacer();
            MelonLogger.Msg(loadingMsg);

            bool hasWroteLine = false;
            List<MelonAssembly> melonAssemblies = new();
            ProcessFolder<T>(path, ref hasWroteLine, ref melonAssemblies);

            var melons = new List<T>();
            foreach (var asm in melonAssemblies)
            {
                asm.LoadMelons();
                foreach (var m in asm.LoadedMelons)
                {
                    if (m is T t)
                    {
                        melons.Add(t);
                    }
                    else
                    {
                        MelonLogger.Warning($"Failed to load Melon '{m.Info.Name}' from '{path}': The given Melon is a {m.MelonTypeName} and cannot be loaded as a {MelonTypeBase<T>.TypeName}. Make sure it's in the right folder.");
                        continue;
                    }
                }
            }

            if (hasWroteLine)
                MelonLogger.WriteSpacer();

            MelonBase.RegisterSorted(melons);

            if (hasWroteLine)
                MelonLogger.WriteLine(Color.Magenta);

            var count = MelonTypeBase<T>._registeredMelons.Count;
            MelonLogger.Msg($"{count} {MelonTypeBase<T>.TypeName.MakePlural(count)} loaded.");
            if (MelonHandler.firstSpacer || (typeof(T) == typeof(MelonMod)))
                MelonLogger.WriteSpacer();
            MelonHandler.firstSpacer = true;
        }

        private static void LoadMelonsFromFolder<T>(string path,
            bool addToList,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies) where T : MelonTypeBase<T>
        {
            var files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var f in files)
            {
                if (!hasWroteLine)
                {
                    hasWroteLine = true;
                    MelonLogger.WriteLine(Color.Magenta);
                }

                var asm = MelonAssembly.LoadMelonAssembly(f, false);
                if (asm == null)
                    continue;

                if (addToList)
                    melonAssemblies.Add(asm);
            }
        }

        private static bool IsValidFolder(string path)
        {
            string dirName = Path.GetDirectoryName(path);
            string dirNameLower = dirName.ToLowerInvariant();
            if (dirNameLower.EndsWith("disabled")
                || dirNameLower.Equals("userlibs")
                || dirNameLower.Equals("plugins")
                || dirNameLower.Equals("mods"))
                return false;
            return true;
        }

        private static void ProcessFolder<T>(string path,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies) where T : MelonTypeBase<T>
        {
            if (!Directory.Exists(path)) 
                return;

            // Skip Disabled Folders
            if (!IsValidFolder(path))
                return;

            InternalUtils.MelonAssemblyResolver.AddSearchDirectory(path);
            LoadMelonsFromFolder<T>(path, true, ref hasWroteLine, ref melonAssemblies);

            // Scan Directories
            var directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in directories)
                ProcessPath<T>(dir, ref hasWroteLine, ref melonAssemblies);
        }

        private static void ProcessPath<T>(string path,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies) where T : MelonTypeBase<T>
        {
            // Skip Disabled Folders
            if (!IsValidFolder(path))
                return;

            // Check for UserLibs
            string melonUserLibs = Path.Combine(path, "UserLibs");
            if (Directory.Exists(melonUserLibs))
            {
                InternalUtils.MelonAssemblyResolver.AddSearchDirectory(melonUserLibs);
                LoadMelonsFromFolder<T>(melonUserLibs, false, ref hasWroteLine, ref melonAssemblies);
            }

            // Check for Plugins/Mods
            string melonMelonFolder = Path.Combine(path, (typeof(T) == typeof(MelonMod)) ? "Mods" : "Plugins");
            if (Directory.Exists(melonMelonFolder))
            {
                InternalUtils.MelonAssemblyResolver.AddSearchDirectory(melonMelonFolder);
                ProcessFolder<T>(melonUserLibs, ref hasWroteLine, ref melonAssemblies);
            }

            // Load Everything in Base of Melon Folder
            InternalUtils.MelonAssemblyResolver.AddSearchDirectory(path);
            ProcessFolder<T>(path, ref hasWroteLine, ref melonAssemblies);
        }
    }
}
