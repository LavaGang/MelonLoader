using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Mono.Cecil;

namespace MelonLoader.Melons
{
    internal static class MelonPreprocessor
    {
        internal static void LoadFolders(List<string> directoryPaths,
            bool isMelon,
            ref bool hasWroteLine,
            ref List<MelonAssembly> melonAssemblies)
        {
            // Find All Assemblies
            Dictionary<string, (Version, string)> foundAssemblies = new();
            foreach (string path in directoryPaths)
                PreprocessFolder(path, isMelon, ref foundAssemblies);

            // Load from File Paths
            foreach (var foundFile in foundAssemblies)
            {
                // Log
                if (!hasWroteLine)
                {
                    hasWroteLine = true;
                    MelonLogger.WriteLine(Color.Magenta);
                }

                // Load Assembly
                var asm = MelonAssembly.LoadMelonAssembly(foundFile.Value.Item2, false);
                if (asm == null)
                    continue;

                // Queue Assembly for Melon Parsing
                if (isMelon)
                    melonAssemblies.Add(asm);
            }
        }

        private static void PreprocessFolder(string path,
            bool isMelon,
            ref Dictionary<string, (Version, string)> foundAssemblies)
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

                // Load Definition using Cecil
                AssemblyDefinition asmDef = LoadDefinition(f);
                if (asmDef == null)
                    continue;

                // Pull Name and Version from AssemblyDefinitionName
                string name = $"{asmDef.Name.Name}";
                Version version = new(asmDef.Name.Version.ToString());

                // Dispose of Definition
                asmDef.Dispose();

                // Check for Existing Version
                if (foundAssemblies.TryGetValue(name, out (Version, string) existingVersion)
                    && (existingVersion.Item1 >= version))
                    continue;

                // Add File to List
                foundAssemblies[name] = (version, f);
            }
        }

        private static AssemblyDefinition LoadDefinition(string path)
        {
            path = Path.GetFullPath(path);

            try
            {
                return AssemblyDefinition.ReadAssembly(path);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to load AssemblyDefinition from '{path}':\n{ex}");
                return null;
            }
        }
    }
}
