using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    public static class MelonHandler
    {
        /// <summary>
        /// A couple common naming conventions for mods that may care about their assemblies definitely being loaded first,
        /// even before the dependency graph can be made. This will almost never matter, but in a few niche
        /// cases could prevent modders from needing to care about alphabetical ordering when naming their DLLs.
        /// </summary>
        private static readonly string[] PriorityLoadTerms = { "API", "Helper", "Lib"};
        
        /// <summary>
        /// Directory of Plugins.
        /// </summary>
        public static string PluginsDirectory { get; internal set; }

        /// <summary>
        /// Directory of Mods.
        /// </summary>
        public static string ModsDirectory { get; internal set; }

        static MelonHandler()
        {
            PluginsDirectory = Path.Combine(MelonUtils.BaseDirectory, "Plugins");
            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
            ModsDirectory = Path.Combine(MelonUtils.BaseDirectory, "Mods");
            if (!Directory.Exists(ModsDirectory))
                Directory.CreateDirectory(ModsDirectory);
        }

        public static void LoadMelonsFromDirectory<T>(string path) where T : Melon<T>
        {
            path = Path.GetFullPath(path);

            var loadingMsg = $"Loading {Melon<T>.TypeName}s from '{path}'...";
            var line = new string('-', loadingMsg.Length + 1);

            MelonLogger.Msg(ConsoleColor.Yellow, line);
            MelonLogger.Msg(loadingMsg);
            MelonLogger.Msg(ConsoleColor.Yellow, line);
            MelonLogger.WriteSpacer();

            var files = Directory.GetFiles(path, "*.dll")
                .OrderByDescending(s => PriorityLoadTerms.Any(term => Path.GetFileName(s).Contains(term)))
                .ThenBy(s => s);
            var melons = new List<T>();

            foreach (var f in files)
            {
                var asm = MelonAssembly.LoadMelonAssembly(f);
                if (asm == null)
                    continue;

                foreach (var m in asm.LoadedMelons)
                {
                    if (m is T t)
                    {
                        melons.Add(t);
                    }
                    else
                    {
                        MelonLogger.Warning($"Failed to load Melon '{m.Info.Name}' from '{path}': The given Melon is a {m.MelonTypeName} and cannot be loaded as a {Melon<T>.TypeName}. Make sure it's in the right Directory.");
                        continue;
                    }
                }
            }

            MelonBase.RegisterSorted(melons);

            MelonLogger.Msg(ConsoleColor.Yellow, line);
            MelonLogger.Msg($"{Melon<T>._registeredMelons.Count} {Melon<T>.TypeName}s loaded.");
            MelonLogger.Msg(ConsoleColor.Yellow, line);
        }

        #region Obsolete Members
        /// <summary>
        /// List of Plugins.
        /// </summary>
        [Obsolete("Use 'MelonPlugin.RegisteredMelons' instead.")]
        public static List<MelonPlugin> Plugins => Melon<MelonPlugin>.RegisteredMelons;

        /// <summary>
        /// List of Mods.
        /// </summary>
        [Obsolete("Use 'MelonMod.RegisteredMelons' instead.")]
        public static List<MelonMod> Mods => Melon<MelonMod>.RegisteredMelons;

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromFile(string filelocation, bool is_plugin) => LoadFromFile(filelocation);

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation) => LoadFromByteArray(filedata, filepath: filelocation);

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation, bool is_plugin) => LoadFromByteArray(filedata, filepath: filelocation);

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromAssembly(Assembly asm, string filelocation, bool is_plugin) => LoadFromAssembly(asm, filelocation);

        [Obsolete("Use 'MelonBase.Hash' instead.")]
        public static string GetMelonHash(MelonBase melonBase)
            => melonBase.Hash;

        [Obsolete("Use 'MelonBase.RegisteredMelons.Exists(1)' instead.")]
        public static bool IsMelonAlreadyLoaded(string name)
            => MelonBase._registeredMelons.Exists(x => x.Info.Name == name);

        [Obsolete("Use 'MelonPlugin.RegisteredMelons.Exists(1)' instead.")]
        public static bool IsPluginAlreadyLoaded(string name)
            => Melon<MelonPlugin>._registeredMelons.Exists(x => x.Info.Name == name);

        [Obsolete("Use 'MelonMod.RegisteredMelons.Exists(1)' instead.")]
        public static bool IsModAlreadyLoaded(string name)
            => Melon<MelonMod>._registeredMelons.Exists(x => x.Info.Name == name);

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromFile(string filepath, string symbolspath = null)
        {
            var asm = MelonAssembly.LoadMelonAssembly(filepath);
            if (asm == null)
                return;

            MelonBase.RegisterSorted(asm.LoadedMelons);
        }

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromByteArray(byte[] filedata, byte[] symbolsdata = null, string filepath = null)
        {
            var asm = MelonAssembly.LoadMelonAssembly(filedata, symbolsdata);
            if (asm == null)
                return;

            MelonBase.RegisterSorted(asm.LoadedMelons);
        }

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromAssembly(Assembly asm, string filepath = null)
        {
            var ma = MelonAssembly.LoadMelonAssembly(asm);
            if (ma == null)
                return;

            MelonBase.RegisterSorted(ma.LoadedMelons);
        }
        #endregion
    }
}
