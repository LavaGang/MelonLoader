using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader.Utils;

namespace MelonLoader
{
    public static class MelonHandler
    {
        /// <summary>
        /// Directory of Plugins.
        /// </summary>
        [Obsolete("Use MelonEnvironment.PluginsDirectory instead")]
        public static string PluginsDirectory => MelonEnvironment.PluginsDirectory;

        /// <summary>
        /// Directory of Mods.
        /// </summary>
        [Obsolete("Use MelonEnvironment.ModsDirectory instead")]
        public static string ModsDirectory => MelonEnvironment.ModsDirectory;

        internal static void Setup()
        {
            if (!Directory.Exists(MelonEnvironment.PluginsDirectory))
                Directory.CreateDirectory(MelonEnvironment.PluginsDirectory);
            
            if (!Directory.Exists(MelonEnvironment.ModsDirectory))
                Directory.CreateDirectory(MelonEnvironment.ModsDirectory);
        }

        private static bool firstSpacer = false;
        public static void LoadMelonsFromDirectory<T>(string path) where T : MelonTypeBase<T>
        {
            path = Path.GetFullPath(path);

            var loadingMsg = $"Loading {MelonTypeBase<T>.TypeName}s from '{path}'...";
            MelonLogger.WriteSpacer();
            MelonLogger.Msg(loadingMsg);

            bool hasWroteLine = false;

            var files = Directory.GetFiles(path, "*.dll");
            var melonAssemblies = new List<MelonAssembly>();
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

                melonAssemblies.Add(asm);
            }

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
            if (firstSpacer || (typeof(T) ==  typeof(MelonMod)))
                MelonLogger.WriteSpacer();
            firstSpacer = true;
        }

        #region Obsolete Members
        /// <summary>
        /// List of Plugins.
        /// </summary>
        [Obsolete("Use 'MelonPlugin.RegisteredMelons' instead.")]
        public static List<MelonPlugin> Plugins => MelonTypeBase<MelonPlugin>.RegisteredMelons.ToList();

        /// <summary>
        /// List of Mods.
        /// </summary>
        [Obsolete("Use 'MelonMod.RegisteredMelons' instead.")]
        public static List<MelonMod> Mods => MelonTypeBase<MelonMod>.RegisteredMelons.ToList();

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
            => MelonTypeBase<MelonPlugin>._registeredMelons.Exists(x => x.Info.Name == name);

        [Obsolete("Use 'MelonMod.RegisteredMelons.Exists(1)' instead.")]
        public static bool IsModAlreadyLoaded(string name)
            => MelonTypeBase<MelonMod>._registeredMelons.Exists(x => x.Info.Name == name);

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
            var asm = MelonAssembly.LoadRawMelonAssembly(filepath, filedata, symbolsdata);
            if (asm == null)
                return;

            MelonBase.RegisterSorted(asm.LoadedMelons);
        }

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromAssembly(Assembly asm, string filepath = null)
        {
            var ma = MelonAssembly.LoadMelonAssembly(filepath, asm);
            if (ma == null)
                return;

            MelonBase.RegisterSorted(ma.LoadedMelons);
        }
        #endregion
    }
}
