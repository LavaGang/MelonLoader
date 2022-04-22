﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET6_0
using System.Runtime.Loader;
#endif

namespace MelonLoader.Melons
{
    internal static class FileHandler
    {
        private static string[] ExtensionBlacklist =
        {
            ".zip",
            ".rar",
            ".bz2",
            ".gz",
            ".lzw",
            ".tar",
            ".xz",
            ".7z"
        };

        private static bool IsExtensionBlacklisted(string filepath)
            => ExtensionBlacklist.Contains(Path.GetExtension(filepath).ToLowerInvariant());

        internal static void LoadAll(string folderpath, bool is_plugins = false)
        {
            string[] filearr = Directory.GetFiles(folderpath).ToArray();
            if (filearr.Length <= 0)
                return;

            MelonLaunchOptions.Core.LoadModeEnum loadMode = is_plugins
                  ? MelonLaunchOptions.Core.LoadMode_Plugins
                  : MelonLaunchOptions.Core.LoadMode_Mods;

            for (int i = 0; i < filearr.Length; i++)
            {
                string filepath = filearr[i];
                if (string.IsNullOrEmpty(filepath))
                    continue;
                
                if (IsExtensionBlacklisted(filepath))
                {
                    MelonLogger.Error($"Invalid File Extension for {filepath}");
                    continue;
                }

                string lowerFilePath = filepath.ToLowerInvariant();

                if ((loadMode == MelonLaunchOptions.Core.LoadModeEnum.NORMAL)
                    && !lowerFilePath.EndsWith(".dll"))
                    continue;

                if ((loadMode == MelonLaunchOptions.Core.LoadModeEnum.DEV)
                    && !lowerFilePath.EndsWith(".dev.dll"))
                    continue;

                // To-Do: File Type Check

                string melonname = GetMelonName(filepath);
                if (string.IsNullOrEmpty(melonname))
                    melonname = Path.GetFileNameWithoutExtension(filepath);

                if (is_plugins 
                    ? MelonPlugin.RegisteredMelons.Any(m => m.Info.Name == melonname)
                    : MelonMod.RegisteredMelons.Any(m => m.Info.Name == melonname))
                {
                    MelonLogger.Error($"Duplicate File: {filepath}");
                    continue;
                }

                LoadFromFile(filepath);
            }
        }

        private static string GetMelonName(string filePath)
        {
            return MelonUtils.GetFileProductName(filePath);
        }

        internal static void LoadFromFile(string filepath, string symbolspath = null)
        {
            if (string.IsNullOrEmpty(filepath))
                return;

            if (IsExtensionBlacklisted(filepath))
            {
                MelonLogger.Error($"Invalid File Extension for {filepath}");
                return;
            }

            string lowerFilePath = filepath.ToLowerInvariant();
            if (!lowerFilePath.EndsWith(".dll"))
                return;

            // To-Do: File Type Check

            //NET6: Always load from file because PDBs are automatically handled, and calling Assembly.Load from a byte array is no bueno, especially as we lose assembly location data and symbols.
#if NET6_0
            try
            {
                var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(filepath);

                MelonHandler.LoadFromAssembly(asm, filepath);
            } catch(Exception ex)
            {
                MelonLogger.Error($"Failed to Load Assembly for {filepath}: {ex}");
            }
#else
            if (MelonDebug.IsEnabled())
            {
                Assembly melonassembly = null;
                try
                {
                    melonassembly = Assembly.LoadFrom(filepath);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Failed to Load Assembly for {filepath}: {ex}");
                    return;
                }

                if (melonassembly == null)
                {
                    MelonLogger.Error($"Failed to Load Assembly for {filepath}: Assembly.LoadFrom returned null"); ;
                    return;
                }

                MelonHandler.LoadFromAssembly(melonassembly, filepath);
            }
            else
            {
                byte[] symbolsdata = null;
                if (string.IsNullOrEmpty(symbolspath))
                    symbolspath = Path.Combine(Path.GetDirectoryName(filepath), $"{Path.GetFileNameWithoutExtension(filepath)}.mdb");
                if (!File.Exists(symbolspath))
                    symbolspath = $"{filepath}.mdb";
                if (File.Exists(symbolspath))
                {
                    try
                    {
                        symbolsdata = File.ReadAllBytes(symbolspath);
                    }
                    catch (Exception ex)
                    {
                        if (MelonDebug.IsEnabled())
                            MelonLogger.Warning($"Failed to Load Symbols for {filepath}: {ex}");
                    }
                }

                byte[] filedata = null;
                try
                {
                    filedata = File.ReadAllBytes(filepath);
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Failed to Read File Data for {filepath}: {ex}");
                    return;
                }

                LoadFromByteArray(filedata, symbolsdata, filepath);
            }
#endif
        }

        internal static void LoadFromByteArray(byte[] filedata, byte[] symbolsdata = null, string filepath = null)
        {
            if (filedata == null)
                return;

            // To-Do: File Type Check

            Assembly melonassembly = null;
            try
            {
#if NET6_0
                var fileStream = new MemoryStream(filedata);
                var symStream = symbolsdata == null ? null : new MemoryStream(symbolsdata);

                melonassembly = AssemblyLoadContext.Default.LoadFromStream(fileStream, symStream);
#else
                melonassembly = Assembly.Load(filedata, symbolsdata);
#endif
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(filepath))
                    MelonLogger.Error($"Failed to Load Assembly from Byte Array: {ex}");
                else
                    MelonLogger.Error($"Failed to Load Assembly for {filepath}: {ex}");
                return;
            }

            if (melonassembly == null)
            {
                if (string.IsNullOrEmpty(filepath))
                    MelonLogger.Error("Failed to Load Assembly from Byte Array: Assembly.Load returned null");
                else
                    MelonLogger.Error($"Failed to Load Assembly for {filepath}: Assembly.Load returned null");
                return;
            }

            if (string.IsNullOrEmpty(filepath))
                filepath = melonassembly.GetName().Name;
            MelonHandler.LoadFromAssembly(melonassembly, filepath);
        }
    }
}
