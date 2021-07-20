using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
        /// List of Plugins.
        /// </summary>
        public static List<MelonPlugin> Plugins { get => _Plugins.AsReadOnly().ToList(); }
        internal static List<MelonPlugin> _Plugins = new List<MelonPlugin>();

        /// <summary>
        /// Directory of Mods.
        /// </summary>
        public static string ModsDirectory { get; internal set; }

        /// <summary>
        /// List of Mods.
        /// </summary>
        public static List<MelonMod> Mods { get => _Mods.AsReadOnly().ToList(); }
        internal static List<MelonMod> _Mods = new List<MelonMod>();

        private static SHA256 sha256 = SHA256.Create();
        static MelonHandler()
        {
            PluginsDirectory = Path.Combine(MelonUtils.BaseDirectory, "Plugins");
            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
            ModsDirectory = Path.Combine(MelonUtils.BaseDirectory, "Mods");
            if (!Directory.Exists(ModsDirectory))
                Directory.CreateDirectory(ModsDirectory);
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
        }

        internal static Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            string assembly_name = args.Name.Split(',')[0];
            string dll_name = (assembly_name + ".dll");
            string plugins_path = Path.Combine(PluginsDirectory, dll_name);
            if (File.Exists(plugins_path))
                return Assembly.LoadFile(plugins_path);
            string mods_path = Path.Combine(ModsDirectory, dll_name);
            if (File.Exists(mods_path))
                return Assembly.LoadFile(mods_path);
            return null;
        }

        internal static void LoadPlugins()
        {
            MelonLogger.Msg("Loading Plugins...");
            LoadMelons(true);
            MelonLogger.WriteSpacer();
            PrintMelonInfo(_Plugins.ToArray(), true);
        }

        internal static void LoadMods()
        {
            MelonLogger.WriteSpacer();
            MelonLogger.Msg("Loading Mods...");
            LoadMelons();
            MelonLogger.WriteSpacer();
            PrintMelonInfo(_Mods.ToArray());
        }

        private static void PrintMelonInfo(MelonBase[] melontbl, bool is_plugins = false)
        {
            if (melontbl.Length <= 0)
            {
                MelonLogger.Msg("------------------------------");
                MelonLogger.Msg($"No {(is_plugins ? "Plugins" : "Mods")} Loaded!");
                MelonLogger.Msg("------------------------------");
                return;
            }

            MelonLogger.Msg("------------------------------");
            MelonLogger.Msg($"{melontbl.Length} {(is_plugins ? "Plugin" : "Mod")}{((_Mods.Count > 1) ? "s" : "")} Loaded");
            MelonLogger.Msg("------------------------------");

            List<MelonBase> melontblList = new List<MelonBase>(melontbl);
            melontblList.Sort((MelonBase left, MelonBase right) => string.Compare(left.Info.Name, right.Info.Name));

            foreach (MelonBase melon in melontblList)
            {
                MelonLogger.Internal_PrintModName(melon.ConsoleColor, melon.Info.Name, melon.Info.Version);

                if (!string.IsNullOrEmpty(melon.Info.Author))
                    MelonLogger.Msg($"by {melon.Info.Author}");

                string melonhash = GetMelonHash(melon);
                if (!string.IsNullOrEmpty(melonhash))
                    MelonLogger.Msg($"SHA256 Hash: {melonhash}");

                MelonLogger.Msg("------------------------------");
            }
        }
        
        private static void LoadMelons(bool plugins = false)
        {
            string basedirectory = plugins ? PluginsDirectory : ModsDirectory;

            MelonBase[] original_array = plugins ? Array.ConvertAll(_Mods.ToArray(), x => (MelonBase)x) : Array.ConvertAll(_Plugins.ToArray(), x => (MelonBase)x);

            MelonFileTypes.DLL.LoadAll(basedirectory, plugins);
            MelonFileTypes.ZIP.LoadAll(basedirectory);

            MelonBase[] new_array = plugins ? Array.ConvertAll(_Mods.ToArray(), x => (MelonBase)x) : Array.ConvertAll(_Plugins.ToArray(), x => (MelonBase)x);

            List<MelonBase> failedMelons = new List<MelonBase>();
            MelonEnumerator<MelonBase> enumerator = new MelonEnumerator<MelonBase>(new_array.ToArray());
            while (enumerator.MoveNext())
            {
                if (!original_array.Contains(enumerator.Current))
                {
                    MelonLogger.Error($"{(plugins ? "Mod" : "Plugin")} {enumerator.Current.Info.Name} is in the {(plugins ? "Plugins" : "Mods")} Folder: {enumerator.Current.Location}");
                    failedMelons.Add(enumerator.Current);
                }
            }

            if (plugins)
            {
                _Mods.RemoveAll(failedMelons.Contains);
                MelonCompatibilityLayer.CreationCheck = false;
            }
            else
                _Plugins.RemoveAll(failedMelons.Contains);

            SortMelons(ref _Mods);
            SortMelons(ref _Plugins);
        }

        public static string GetMelonHash(MelonBase melonBase)
        {
            if ((melonBase == null)
                || string.IsNullOrEmpty(melonBase.Location))
                return null;

            string extension = Path.GetExtension(melonBase.Location).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension)
                || !extension.Equals(".dll"))
                return null;

            byte[] byteHash = sha256.ComputeHash(File.ReadAllBytes(melonBase.Location));
            string finalHash = string.Empty;
            foreach (byte b in byteHash)
                finalHash += b.ToString("x2");
            return finalHash;
        }

        public static bool IsMelonAlreadyLoaded(string name) => (IsPluginAlreadyLoaded(name) || IsModAlreadyLoaded(name));
        public static bool IsPluginAlreadyLoaded(string name) => (_Plugins.Find(x => x.Info.Name.Equals(name)) != null);
        public static bool IsModAlreadyLoaded(string name) => (_Mods.Find(x => x.Info.Name.Equals(name)) != null);

        public static void LoadFromFile(string filepath, string symbolspath = null)
        {
            if (string.IsNullOrEmpty(filepath))
                return;
            switch (Path.GetExtension(filepath).ToLowerInvariant())
            {
                case ".dll":
                    MelonFileTypes.DLL.LoadFromFile(filepath, symbolspath);
                    goto default;
                case ".zip":
                    MelonFileTypes.ZIP.LoadFromFile(filepath);
                    goto default;
                default:
                    break;
            }
        }

        public static void LoadFromByteArray(byte[] filedata, byte[] symbolsdata = null, string filepath = null)
        {
            if (filedata == null)
                return;

            // To-Do Check for ZIP Byte Arrays

            MelonFileTypes.DLL.LoadFromByteArray(filedata, symbolsdata, filepath);
        }

        public static void LoadFromAssembly(Assembly asm, string filepath = null)
        {
            if (asm == null)
                return;
            if (string.IsNullOrEmpty(filepath))
                filepath = asm.GetName().Name;

            MelonCompatibilityLayer.Resolver resolver = MelonCompatibilityLayer.GetResolverFromAssembly(asm, filepath);
            if (resolver == null)
            {
                MelonLogger.Error($"Failed to Load Assembly for {filepath}: No Compatibility Layer Found!");
                return;
            }

            try
            {
                List<MelonBase> melonTbl = new List<MelonBase>();
                resolver.CheckAndCreate(ref melonTbl);
                if (melonTbl.Count <= 0)
                    return;

                foreach (MelonBase melon in melonTbl)
                {
                    if (melon is MelonPlugin plugin)
                        _Plugins.Add(plugin);
                    else
                        _Mods.Add((MelonMod)melon);
                }

                // To-Do Check for Late Loads and Display Debug Warning
            }
            catch(Exception ex) { MelonLogger.Error($"Failed to Resolve Melons for {filepath}: {ex}"); }
        }

        internal static void OnPreInitialization()
            => InvokeMelonMethod(ref _Plugins, x =>
            {
                if (MelonUtils.PullAttributeFromAssembly<HarmonyDontPatchAllAttribute>(x.Assembly) == null)
                    x.HarmonyInstance.PatchAll(x.Assembly);
                x.OnPreInitialization();
            }, true);

        internal static void OnApplicationStart_Plugins()
            => InvokeMelonMethod(ref _Plugins, x =>
            {
                RegisterTypeInIl2Cpp.RegisterAssembly(x.Assembly);
                x.OnApplicationStart();
            }, true);

        internal static void OnApplicationStart_Mods()
            => InvokeMelonMethod(ref _Mods, x =>
            {
                if (MelonUtils.PullAttributeFromAssembly<HarmonyDontPatchAllAttribute>(x.Assembly) == null)
                    x.HarmonyInstance.PatchAll(x.Assembly);
                RegisterTypeInIl2Cpp.RegisterAssembly(x.Assembly);
                x.OnApplicationStart();
            }, true);

        internal static void OnApplicationEarlyStart() => InvokeMelonMethod(ref _Plugins, x => x.OnApplicationEarlyStart(), true);
        internal static void OnApplicationLateStart_Plugins() => InvokeMelonMethod(ref _Plugins, x => x.OnApplicationLateStart(), true);
        internal static void OnApplicationLateStart_Mods() => InvokeMelonMethod(ref _Mods, x => x.OnApplicationLateStart(), true);
        internal static void OnApplicationQuit() => InvokeMelonMethod(x => x.OnApplicationQuit());
        internal static void OnFixedUpdate() => InvokeMelonMethod(ref _Mods, x => x.OnFixedUpdate());
        internal static void OnLateUpdate() => InvokeMelonMethod(x => x.OnLateUpdate());
        internal static void OnGUI() => InvokeMelonMethod(x => x.OnGUI());
        internal static void OnPreferencesSaved() => InvokeMelonMethod(x =>
        {
            x.OnModSettingsApplied();
            x.OnPreferencesSaved();
        });
        internal static void OnPreferencesLoaded() => InvokeMelonMethod(x =>
        {
            x.OnModSettingsApplied();
            x.OnPreferencesLoaded();
        });
        internal static void BONEWORKS_OnLoadingScreen() => InvokeMelonMethod(ref _Mods, x => x.BONEWORKS_OnLoadingScreen());

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
            InvokeMelonMethod(ref _Mods, x =>
            {
                x.OnLevelWasLoaded(buildIndex);
                x.OnSceneWasLoaded(buildIndex, sceneName);
            });
        }

        internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
            => InvokeMelonMethod(ref _Mods, x =>
            {
                x.OnLevelWasInitialized(buildIndex);
                x.OnSceneWasInitialized(buildIndex, sceneName);
            });

        internal static void OnSceneWasUnloaded(int buildIndex, string sceneName)
            => InvokeMelonMethod(ref _Mods, x => x.OnSceneWasUnloaded(buildIndex, sceneName));

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
            InvokeMelonMethod(x => x.OnUpdate());
        }

        private static void SortMelons<T>(ref List<T> melons) where T : MelonBase
        {
            DependencyGraph<T>.TopologicalSort(melons);
            melons = melons.OrderBy(x => x.Priority).ToList();
            if (typeof(T) == typeof(MelonPlugin))
                MelonCompatibilityLayer.RefreshPlugins();
            else if (typeof(T) == typeof(MelonMod))
                MelonCompatibilityLayer.RefreshMods();
        }

        private delegate void InvokeMelonMethodDelegate(MelonBase melon);
        private delegate void InvokeMelonMethodDelegate<T>(T melon) where T : MelonBase;
        private static void InvokeMelonMethod(InvokeMelonMethodDelegate method, bool remove_failed = false)
        {
            InvokeMelonMethod(ref _Plugins, x => method(x), remove_failed);
            InvokeMelonMethod(ref _Mods, x => method(x), remove_failed);
        }
        private static void InvokeMelonMethod<T>(ref List<T> melons, InvokeMelonMethodDelegate<T> method, bool remove_failed = false) where T : MelonBase
        {
            if ((melons == null)
                || (melons.Count <= 0))
                return;

            List<T> failedMelons = (remove_failed ? new List<T>() : null);

            MelonEnumerator<T> enumerator = new MelonEnumerator<T>(melons.ToArray());
            while (enumerator.MoveNext())
            {
                try { method(enumerator.Current); }
                catch (Exception ex)
                {
                    MelonLogger.ManualMelonError(enumerator.Current, ex.ToString());
                    if (remove_failed)
                        failedMelons.Add(enumerator.Current);
                }
            }

            if (remove_failed)
            {
                melons.RemoveAll(failedMelons.Contains);
                SortMelons(ref melons);
            }
        }

        [Obsolete("MelonLoader.MelonHandler.LoadFromFile(string, bool) is obsolete. Please use MelonLoader.MelonHandler.LoadFromFile(string, string) instead.")]
        public static void LoadFromFile(string filelocation, bool is_plugin) => LoadFromFile(filelocation);
        [Obsolete("MelonLoader.MelonHandler.LoadFromByteArray(byte[], string) is obsolete. Please use MelonLoader.MelonHandler.LoadFromByteArray(byte[], byte[], string) instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation) => LoadFromByteArray(filedata, filepath: filelocation);
        [Obsolete("MelonLoader.MelonHandler.LoadFromByteArray(byte[], string, bool) is obsolete. Please use MelonLoader.MelonHandler.LoadFromByteArray(byte[], byte[], string) instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation, bool is_plugin) => LoadFromByteArray(filedata, filepath: filelocation);
        [Obsolete("MelonLoader.MelonHandler.LoadFromAssembly(Assembly, string, bool) is obsolete. Please use MelonLoader.MelonHandler.LoadFromAssembly(Assembly, string) instead.")]
        public static void LoadFromAssembly(Assembly asm, string filelocation, bool is_plugin) => LoadFromAssembly(asm, filelocation);
    }
}
