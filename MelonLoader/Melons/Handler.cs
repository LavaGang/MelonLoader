using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
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
            PluginsDirectory = Path.Combine(MelonUtils.GameDirectory, "Plugins");
            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
            ModsDirectory = Path.Combine(MelonUtils.GameDirectory, "Mods");
            if (!Directory.Exists(ModsDirectory))
                Directory.CreateDirectory(ModsDirectory);
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
            if (_Plugins.Count <= 0)
            {
                MelonLogger.Msg("------------------------------");
                MelonLogger.Msg("No Plugins Loaded!");
                MelonLogger.Msg("------------------------------");
                return;
            }
            MelonLogger.Msg("------------------------------");
            MelonLogger.Msg(_Plugins.Count.ToString() + " Plugin" + ((_Plugins.Count > 1) ? "s" : "") + " Loaded");
            MelonLogger.Msg("------------------------------");
            foreach (MelonPlugin plugin in _Plugins)
            {
                MelonLogger.Internal_PrintModName(plugin.ConsoleColor, plugin.Info.Name, plugin.Info.Version);
                if (!string.IsNullOrEmpty(plugin.Info.Author))
                    MelonLogger.Msg("by " + plugin.Info.Author);
                MelonLogger.Msg("SHA256 Hash: " + GetMelonHash(plugin));
                MelonLogger.Msg("------------------------------");
            }
            SetupAttributes_Plugins();
        }

        internal static void LoadMods()
        {
            MelonLogger.WriteSpacer();
            MelonLogger.Msg("Loading Mods...");
            LoadMelons();
            MelonLogger.WriteSpacer();
            if (_Mods.Count <= 0)
            {
                MelonLogger.Msg("------------------------------");
                MelonLogger.Msg("No Mods Loaded!");
                MelonLogger.Msg("------------------------------");
                return;
            }
            MelonLogger.Msg("------------------------------");
            MelonLogger.Msg(_Mods.Count.ToString() + " Mod" + ((_Mods.Count > 1) ? "s" : "") + " Loaded");
            MelonLogger.Msg("------------------------------");
            foreach (MelonMod mod in _Mods)
            {
                MelonLogger.Internal_PrintModName(mod.ConsoleColor, mod.Info.Name, mod.Info.Version);
                if (!string.IsNullOrEmpty(mod.Info.Author))
                    MelonLogger.Msg("by " + mod.Info.Author);
                MelonLogger.Msg("SHA256 Hash: " + GetMelonHash(mod));
                MelonLogger.Msg("------------------------------");
            }
            SetupAttributes_Mods();
        }
        
        private static void LoadMelons(bool plugins = false)
        {
            MelonLaunchOptions.Core.LoadModeEnum mode = (plugins ? MelonLaunchOptions.Core.LoadMode_Plugins : MelonLaunchOptions.Core.LoadMode_Mods);
            string basedirectory = (plugins ? PluginsDirectory : ModsDirectory);

            // DLLs
            string[] dlltbl = Directory.GetFiles(basedirectory, "*.dll");
            if (dlltbl.Length > 0)
                for (int i = 0; i < dlltbl.Length; i++)
                {
                    string filename = dlltbl[i];
                    if (string.IsNullOrEmpty(filename))
                        continue;

                    if (mode != MelonLaunchOptions.Core.LoadModeEnum.BOTH)
                    {
                        bool file_extension_check = filename.EndsWith(".dev.dll");
                        if (((mode == MelonLaunchOptions.Core.LoadModeEnum.NORMAL) && file_extension_check) || ((mode == MelonLaunchOptions.Core.LoadModeEnum.DEV) && !file_extension_check))
                            continue;
                    }

                    string melonname = MelonUtils.GetFileProductName(filename);
                    if (string.IsNullOrEmpty(melonname))
                        melonname = Path.GetFileNameWithoutExtension(filename);

                    bool isAlreadyLoaded = (plugins ? IsPluginAlreadyLoaded(melonname) : IsModAlreadyLoaded(melonname));
                    if (isAlreadyLoaded)
                    {
                        MelonLogger.Warning("Duplicate File: " + filename);
                        continue;
                    }

                    LoadFromFile(filename, plugins);
                }

            // ZIPs
            string[] ziptbl = Directory.GetFiles(basedirectory, "*.zip");
            if (ziptbl.Length > 0)
                for (int i = 0; i < ziptbl.Length; i++)
                {
                    string filename = ziptbl[i];
                    if (string.IsNullOrEmpty(filename))
                        continue;
                    try
                    {
                        using (var filestream = File.OpenRead(filename))
                        using (var zipstream = new ZipInputStream(filestream))
                        {
                            ZipEntry entry;
                            while ((entry = zipstream.GetNextEntry()) != null)
                            {
                                if (string.IsNullOrEmpty(entry.Name))
                                    continue;
                                string filename2 = Path.GetFileName(entry.Name);
                                if (string.IsNullOrEmpty(filename2))
                                    continue;
                                if (mode != MelonLaunchOptions.Core.LoadModeEnum.BOTH)
                                {
                                    bool file_extension_check = filename2.EndsWith(".dev.dll");
                                    if (((mode == MelonLaunchOptions.Core.LoadModeEnum.NORMAL) && file_extension_check) || ((mode == MelonLaunchOptions.Core.LoadModeEnum.DEV) && !file_extension_check))
                                        continue;
                                }
                                using (MemoryStream memorystream = new MemoryStream())
                                {
                                    int size = 0;
                                    byte[] buffer = new byte[4096];
                                    while (true)
                                    {
                                        size = zipstream.Read(buffer, 0, buffer.Length);
                                        if (size > 0)
                                            memorystream.Write(buffer, 0, size);
                                        else
                                            break;
                                    }
                                    LoadFromByteArray(memorystream.ToArray(), (filename + "/" + filename2), plugins);
                                }
                            }
                        }
                    }
                    catch(Exception ex) { MelonLogger.Error(ex.ToString()); }
                }
        }

        public static string GetMelonHash(MelonBase melonBase)
        {
            byte[] byteHash = sha256.ComputeHash(File.ReadAllBytes(melonBase.Location));
            string finalHash = string.Empty;
            foreach (byte b in byteHash)
                finalHash += b.ToString("x2");
            return finalHash;
        }

        public static bool IsMelonAlreadyLoaded(string name) => (IsPluginAlreadyLoaded(name) || IsModAlreadyLoaded(name));
        public static bool IsPluginAlreadyLoaded(string name) => (_Plugins.Find(x => x.Info.Name.Equals(name)) != null);
        public static bool IsModAlreadyLoaded(string name) => (_Mods.Find(x => x.Info.Name.Equals(name)) != null);

        public static void LoadFromFile(string filelocation, bool is_plugin = false)
        {
            if (string.IsNullOrEmpty(filelocation))
                return;
            if (!MelonDebug.IsEnabled())
            {
                LoadFromByteArray(File.ReadAllBytes(filelocation), filelocation, is_plugin);
                return;
            }
            try
            {
                Assembly asm = Assembly.LoadFrom(filelocation);
                if (asm == null)
                {
                    MelonLogger.Error("Failed to Load Assembly for " + filelocation + ": Assembly.LoadFrom returned null"); ;
                    return;
                }
                LoadFromAssembly(asm, filelocation, is_plugin);
            }
            catch (Exception ex) { MelonLogger.Error("Failed to Load Assembly for " + filelocation + ": " + ex.ToString()); }
        }

        [Obsolete("MelonLoader.MelonHandler.LoadFromAssembly(byte[], string) is obsolete. Please use MelonLoader.MelonHandler.LoadFromAssembly(byte[], string, bool) instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation = null) => LoadFromByteArray(filedata, filelocation, false);
        public static void LoadFromByteArray(byte[] filedata, string filelocation = null, bool is_plugin = false)
        {
            if ((filedata == null) || (filedata.Length <= 0))
                return;
            try
            {
                byte[] symbols = { 0 };
                if (!string.IsNullOrEmpty(filelocation)
                    && !filelocation.Contains(".zip/")
                    && File.Exists(filelocation + ".mdb"))
                    symbols = File.ReadAllBytes(filelocation + ".mdb");

                Assembly asm = Assembly.Load(filedata, symbols);
                if (asm == null)
                {
                    if (string.IsNullOrEmpty(filelocation))
                        MelonLogger.Error("Failed to Load Assembly: Assembly.Load returned null");
                    else
                        MelonLogger.Error("Failed to Load Assembly for " + filelocation + ": Assembly.Load returned null");
                    return;
                }
                LoadFromAssembly(asm, filelocation, is_plugin);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(filelocation))
                    MelonLogger.Error("Failed to Load Assembly:" + ex.ToString());
                else
                    MelonLogger.Error("Failed to Load Assembly for " + filelocation + ": " + ex.ToString());
            }
        }

        [Obsolete("MelonLoader.MelonHandler.LoadFromAssembly(Assembly, string) is obsolete. Please use MelonLoader.MelonHandler.LoadFromAssembly(Assembly, string, bool) instead.")]
        public static void LoadFromAssembly(Assembly asm, string filelocation = null) => LoadFromAssembly(asm, filelocation, false);
        public static void LoadFromAssembly(Assembly asm, string filelocation = null, bool is_plugin = false)
        {
            if (asm == null)
                return;
            if (string.IsNullOrEmpty(filelocation))
                filelocation = asm.GetName().Name;

            MelonCompatibilityLayer.Resolver resolver = MelonCompatibilityLayer.ResolveAssemblyToLayerResolver(asm);
            if (resolver == null)
            {
                // File Is Not Compatible
                // To-Do: Error Here
                return;
            }

            List<MelonBase> melonTbl = new List<MelonBase>();
            resolver.CheckAndCreate(filelocation, is_plugin, ref melonTbl);
            if (melonTbl.Count <= 0)
                return;

            foreach (MelonBase melon in melonTbl)
            {
                if (melon is MelonPlugin plugin)
                    _Plugins.Add(plugin);
                else
                    _Mods.Add((MelonMod)melon);
            }

            if (is_plugin)
                SortPlugins();
            else
                SortMods();
        }

        internal static T PullCustomAttributeFromAssembly<T>(Assembly asm) where T : Attribute
        {
            T[] attributetbl = PullCustomAttributesFromAssembly<T>(asm);
            if ((attributetbl == null) || (attributetbl.Count() <= 0))
                return null;
            return attributetbl.First();
        }

        internal static T[] PullCustomAttributesFromAssembly<T>(Assembly asm) where T : Attribute
        {
            Attribute[] att_tbl = Attribute.GetCustomAttributes(asm);
            if ((att_tbl == null) || (att_tbl.Length <= 0))
                return null;
            Type requestedType = typeof(T);
            List<T> output = new List<T>();
            foreach (Attribute att in att_tbl)
            {
                Type attType = att.GetType();
                string attAssemblyName = attType.Assembly.GetName().Name;
                string requestedAssemblyName = requestedType.Assembly.GetName().Name;
                if ((attType == requestedType)
                    || attType.FullName.Equals(requestedType.FullName)
                    || ((attAssemblyName.Equals("MelonLoader")
                        || attAssemblyName.Equals("MelonLoader.ModHandler"))
                        && (requestedAssemblyName.Equals("MelonLoader")
                        || requestedAssemblyName.Equals("MelonLoader.ModHandler"))
                        && attType.Name.Equals(requestedType.Name)))
                    output.Add(att as T);
            }
            return output.ToArray();
        }

        private static void SortPlugins()
        {
            _Plugins = _Plugins.OrderBy(x => x.Priority).ToList();
            DependencyGraph<MelonPlugin>.TopologicalSort(_Plugins);
            MelonCompatibilityLayer.RefreshPluginsTable();
        }

        private static void SortMods()
        {
            _Mods = _Mods.OrderBy(x => x.Priority).ToList();
            DependencyGraph<MelonMod>.TopologicalSort(_Mods);
            MelonCompatibilityLayer.RefreshModsTable();
        }

        private static void RegisterIl2CppInjectAttributes(Assembly asm)
        {
            if (!MelonUtils.IsGameIl2Cpp())
                return;
            Type[] typeTbl = asm.GetTypes();
            if ((typeTbl == null) || (typeTbl.Length <= 0))
                return;
            foreach (Type type in typeTbl)
            {
                object[] attTbl = type.GetCustomAttributes(typeof(RegisterTypeInIl2Cpp), false);
                if ((attTbl == null) || (attTbl.Length <= 0))
                    continue;
                UnhollowerSupport.RegisterTypeInIl2CppDomain(type);
            }
        }

        private static void SetupAttributes_Plugins()
        {
            List<Assembly> setupasm = new List<Assembly>();
            InvokeMelonPluginMethod(x =>
            {
                if (setupasm.Contains(x.Assembly))
                    return;
                //RegisterIl2CppInjectAttributes(x.Assembly);
                x.HarmonyInstance.PatchAll(x.Assembly);
                setupasm.Add(x.Assembly);
            }, true);
        }

        private static void SetupAttributes_Mods()
        {
            List<Assembly> setupasm = new List<Assembly>();
            InvokeMelonModMethod(x =>
            {
                if (setupasm.Contains(x.Assembly))
                    return;
                RegisterIl2CppInjectAttributes(x.Assembly);
                x.HarmonyInstance.PatchAll(x.Assembly);
                setupasm.Add(x.Assembly);
            }, true);
        }

        internal static void OnPreInitialization() => InvokeMelonPluginMethod(x => x.OnPreInitialization(), true);
        internal static void OnApplicationEarlyStart() => InvokeMelonPluginMethod(x => x.OnApplicationEarlyStart(), true);
        internal static void OnApplicationStart_Plugins() => InvokeMelonPluginMethod(x => x.OnApplicationStart(), true);
        internal static void OnApplicationStart_Mods() => InvokeMelonModMethod(x => x.OnApplicationStart(), true);
        internal static void OnApplicationLateStart_Plugins() => InvokeMelonPluginMethod(x => x.OnApplicationLateStart(), true);
        internal static void OnApplicationLateStart_Mods() => InvokeMelonModMethod(x => x.OnApplicationLateStart(), true);
        internal static void OnApplicationQuit() { InvokeMelonPluginMethod(x => x.OnApplicationQuit()); InvokeMelonModMethod(x => x.OnApplicationQuit()); }
        internal static void OnFixedUpdate() => InvokeMelonModMethod(x => x.OnFixedUpdate());
        internal static void OnLateUpdate() { InvokeMelonPluginMethod(x => x.OnLateUpdate()); InvokeMelonModMethod(x => x.OnLateUpdate()); }
        internal static void OnGUI() { InvokeMelonPluginMethod(x => x.OnGUI()); InvokeMelonModMethod(x => x.OnGUI()); }
        internal static void OnPreferencesSaved() { InvokeMelonPluginMethod(x => x.OnPreferencesSaved()); InvokeMelonModMethod(x => x.OnPreferencesSaved()); }
        internal static void OnPreferencesLoaded() { InvokeMelonPluginMethod(x => x.OnPreferencesLoaded()); InvokeMelonModMethod(x => x.OnPreferencesLoaded()); }
        internal static void BONEWORKS_OnLoadingScreen() => InvokeMelonModMethod(x => x.BONEWORKS_OnLoadingScreen());

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
            InvokeMelonModMethod(x => x.OnSceneWasLoaded(buildIndex, sceneName));
        }

        internal static void OnSceneWasInitialized(int buildIndex, string sceneName) => InvokeMelonModMethod(x => x.OnSceneWasInitialized(buildIndex, sceneName));

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
            InvokeMelonPluginMethod(x => x.OnUpdate());
            InvokeMelonModMethod(x => x.OnUpdate());
        }

        private delegate void InvokeMelonPluginMethodDelegate(MelonPlugin plugin);
        private static void InvokeMelonPluginMethod(InvokeMelonPluginMethodDelegate method, bool remove_failed = false)
        {
            if (_Plugins.Count <= 0)
                return;
            List<MelonPlugin> failedPlugins = (remove_failed ? new List<MelonPlugin>() : null);
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { method(PluginEnumerator.Current); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); if (remove_failed) failedPlugins.Add(PluginEnumerator.Current); }
            if (!remove_failed)
                return;
            _Plugins.RemoveAll(failedPlugins.Contains);
            SortPlugins();
        }

        private delegate void InvokeMelonModMethodDelegate(MelonMod mod);
        private static void InvokeMelonModMethod(InvokeMelonModMethodDelegate method, bool remove_failed = false)
        {
            if (_Mods.Count <= 0)
                return;
            List<MelonMod> failedMods = (remove_failed ? new List<MelonMod>() : null);
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { method(ModEnumerator.Current); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); if (remove_failed) failedMods.Add(ModEnumerator.Current); }
            if (!remove_failed)
                return;
            _Mods.RemoveAll(failedMods.Contains);
            SortMods();
        }
    }
}
