using MelonLoader.InternalUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader.Lemons.Cryptography;
#pragma warning disable 0618

namespace MelonLoader
{
    public static class MelonHandler
    {
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

            MelonLogger.Msg($"Loading {Melon<T>.TypeName}s from '{path}'...");

            var files = Directory.GetFiles(path, "*.dll");
            var melons = new List<T>();

            foreach (var f in files)
            {
                var fileMelons = MelonBase.Load(f, out MelonLoadErrorCodes errorCode);
                if (errorCode != MelonLoadErrorCodes.None)
                {
                    PrintErrorCodeMessage(errorCode, f);
                    continue;
                }
                foreach (var m in fileMelons)
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

            MelonBase.RegisterInOrder(melons);
        }

        public static void PrintErrorCodeMessage(MelonLoadErrorCodes errorCode, string melonLocation)
        {
            if (errorCode == MelonLoadErrorCodes.None)
                return;

            var msg = errorCode switch
            {
                MelonLoadErrorCodes.InvalidPath => "The given Path does not exist!",
                MelonLoadErrorCodes.ModNotSupported => "The given Assembly is incompatible with MelonLoader; no Compatibility Layer found!",
                MelonLoadErrorCodes.WrongFileExtension => "The given File is not a valid .NET Assembly!",
                MelonLoadErrorCodes.InvalidMelonType => "The main Melon Type is incompatible with MelonLoader; make sure it derives from one of the Melon types (MelonLoader.MelonMod for example)!",
                MelonLoadErrorCodes.FailedToLoadAssembly => "Failed to load the Assembly!",
                MelonLoadErrorCodes.AssemblyIsNull => "Assembly is Null!",
                MelonLoadErrorCodes.FailedToReadFile => "Failed to read the File from the given Path!",
                MelonLoadErrorCodes.FailedToInitializeMelon => "Something went wrong while initializing the main Melon Instance!",
                _ => "Unknown Error"
            };

            MelonLogger.Error($"Failed to load Melon(s) from '{melonLocation}': {msg}");
        }

        internal static void OnPreInitialization()
            => MelonPlugin.ExecuteAll(x => x.OnPreInitialization(), true);

        internal static void OnApplicationStart_Plugins()
        {
            MelonStartScreen.OnApplicationStart_Plugins();
            MelonPlugin.ExecuteAll(x =>
            {
                MelonStartScreen.OnApplicationStart_Plugin(x.Info.Name);
                x.OnApplicationStart();
            }, true);
        }

        internal static void OnApplicationStart_Mods()
        {
            MelonStartScreen.OnApplicationStart_Mods();
            MelonMod.ExecuteAll(x =>
            {
                MelonStartScreen.OnApplicationStart_Mod(x.Info.Name);
                x.OnApplicationStart();
            }, true);
        }

        internal static void OnApplicationLateStart_Plugins()
        {
            MelonStartScreen.OnApplicationLateStart_Plugins();
            MelonPlugin.ExecuteAll(x =>
            {
                MelonStartScreen.OnApplicationLateStart_Plugin(x.Info.Name);
                x.OnApplicationLateStart();
            }, true);
        }

        internal static void OnApplicationLateStart_Mods()
        {
            MelonStartScreen.OnApplicationLateStart_Mods();
            MelonMod.ExecuteAll(x =>
            {
                MelonStartScreen.OnApplicationLateStart_Mod(x.Info.Name);
                x.OnApplicationLateStart();
            }, true);
        }

        internal static void OnPreSupportModule()
        {
            MelonBase.ExecuteAll(x => x.OnPreSupportModule(), true);
        }
        internal static void OnApplicationEarlyStart() => MelonPlugin.ExecuteAll(x => x.OnApplicationEarlyStart(), true);
        internal static void OnApplicationQuit() => MelonBase.ExecuteAll(x => x.OnApplicationQuit());
        internal static void OnFixedUpdate() => MelonMod.ExecuteAll(x => x.OnFixedUpdate());
        internal static void OnLateUpdate() => MelonBase.ExecuteAll(x => x.OnLateUpdate());
        internal static void OnGUI() => MelonBase.ExecuteAll(x => x.OnGUI());
        internal static void OnPreferencesSaved(string filepath) => MelonBase.ExecuteAll(x =>
        {
            x.OnModSettingsApplied();
            x.OnPreferencesSaved();
            x.OnPreferencesSaved(filepath);
        });
        internal static void OnPreferencesLoaded(string filepath) => MelonBase.ExecuteAll(x =>
        {
            x.OnModSettingsApplied();
            x.OnPreferencesLoaded();
            x.OnPreferencesLoaded(filepath);
        });
        internal static void BONEWORKS_OnLoadingScreen() => MelonMod.ExecuteAll(x => x.BONEWORKS_OnLoadingScreen());

        private static bool SceneWasJustLoaded = false;
        private static int CurrentSceneBuildIndex = -1;
        private static string CurrentSceneName = null;
        internal static void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!MelonUtils.IsBONEWORKS)
            {
                SceneWasJustLoaded = true;
                CurrentSceneBuildIndex = buildIndex;
                CurrentSceneName = sceneName;
            }
            MelonMod.ExecuteAll(x =>
            {
                x.OnLevelWasLoaded(buildIndex);
                x.OnSceneWasLoaded(buildIndex, sceneName);
            });
        }

        internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
            => MelonMod.ExecuteAll(x =>
            {
                x.OnLevelWasInitialized(buildIndex);
                x.OnSceneWasInitialized(buildIndex, sceneName);
            });

        internal static void OnSceneWasUnloaded(int buildIndex, string sceneName)
            => MelonMod.ExecuteAll(x => x.OnSceneWasUnloaded(buildIndex, sceneName));

        private static bool InitializeScene = false;
        internal static void OnUpdate()
        {
            if (InitializeScene)
            {
                InitializeScene = false;
                OnSceneWasInitialized(CurrentSceneBuildIndex, CurrentSceneName);
            }
            if (SceneWasJustLoaded)
            {
                SceneWasJustLoaded = false;
                InitializeScene = true;
            }
            MelonBase.ExecuteAll(x => x.OnUpdate());
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

        [Obsolete("MelonLoader.MelonHandler.LoadFromFile(string, bool) is obsolete. Please use MelonLoader.MelonHandler.LoadFromFile(string, string) instead.")]
        public static void LoadFromFile(string filelocation, bool is_plugin) => LoadFromFile(filelocation);

        [Obsolete("MelonLoader.MelonHandler.LoadFromByteArray(byte[], string) is obsolete. Please use MelonLoader.MelonHandler.LoadFromByteArray(byte[], byte[], string) instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation) => LoadFromByteArray(filedata, filepath: filelocation);

        [Obsolete("MelonLoader.MelonHandler.LoadFromByteArray(byte[], string, bool) is obsolete. Please use MelonLoader.MelonHandler.LoadFromByteArray(byte[], byte[], string) instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation, bool is_plugin) => LoadFromByteArray(filedata, filepath: filelocation);

        [Obsolete("MelonLoader.MelonHandler.LoadFromAssembly(Assembly, string, bool) is obsolete. Please use MelonLoader.MelonHandler.LoadFromAssembly(Assembly, string) instead.")]
        public static void LoadFromAssembly(Assembly asm, string filelocation, bool is_plugin) => LoadFromAssembly(asm, filelocation);

        [Obsolete("Use 'melonBase.Hash' instead.")]
        public static string GetMelonHash(MelonBase melonBase)
            => melonBase.Hash;

        [Obsolete("Use 'MelonBase.RegisteredMelons.Any(1)' instead.")]
        public static bool IsMelonAlreadyLoaded(string name)
            => MelonBase._registeredMelons.Any(x => x.Info.Name == name);

        [Obsolete("Use 'MelonPlugin.RegisteredMelons.Any(1)' instead.")]
        public static bool IsPluginAlreadyLoaded(string name)
            => Melon<MelonPlugin>._registeredMelons.Any(x => x.Info.Name == name);

        [Obsolete("Use 'MelonMod.RegisteredMelons.Any(1)' instead.")]
        public static bool IsModAlreadyLoaded(string name)
            => Melon<MelonMod>._registeredMelons.Any(x => x.Info.Name == name);

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromFile(string filepath, string symbolspath = null)
        {
            if (filepath == null)
                return;

            filepath = Path.GetFullPath(filepath);

            var melons = MelonBase.Load(filepath, out MelonLoadErrorCodes errorCode);
            if (errorCode != MelonLoadErrorCodes.None)
            {
                PrintErrorCodeMessage(errorCode, filepath);
                return;
            }

            foreach (var m in melons)
                m.Register();
        }

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromByteArray(byte[] filedata, byte[] symbolsdata = null, string filepath = null)
        {
            if (filedata == null)
                return;

            var melons = MelonBase.Load(filedata, out MelonLoadErrorCodes errorCode, symbolsdata);
            if (errorCode != MelonLoadErrorCodes.None)
            {
                PrintErrorCodeMessage(errorCode, "Raw Assembly Data");
                return;
            }

            foreach (var m in melons)
                m.Register();
        }

        [Obsolete("Use 'MelonBase.Load' and 'MelonBase.Register' instead.")]
        public static void LoadFromAssembly(Assembly asm, string filepath = null)
        {
            if (asm == null)
                return;

            var melons = MelonBase.Load(asm, out MelonLoadErrorCodes errorCode);
            if (errorCode != MelonLoadErrorCodes.None)
            {
                PrintErrorCodeMessage(errorCode, asm.Location);
                return;
            }

            foreach (var m in melons)
                m.Register();
        }
        #endregion
    }
}
