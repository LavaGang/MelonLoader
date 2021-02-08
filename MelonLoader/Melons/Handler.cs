using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                MelonLogger.Msg("by " + plugin.Info.Author);
                MelonLogger.Msg("SHA256 Hash: " + GetMelonHash(plugin));
                MelonLogger.Msg("------------------------------");
            }
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
                MelonLogger.Msg("by " + mod.Info.Author);
                MelonLogger.Msg("SHA256 Hash: " + GetMelonHash(mod));
                MelonLogger.Msg("------------------------------");
            }
        }
        
        private static string GetMelonHash(MelonBase melonBase)
        {
            byte[] byteHash = sha256.ComputeHash(File.ReadAllBytes(melonBase.Location));

            string finalHash = string.Empty;
            foreach (byte b in byteHash)
                finalHash += b.ToString("x2");

            return finalHash;
        }
        
        private static void LoadMelons(bool plugins = false)
        {
            LoadMode mode = (plugins ? GetLoadModeForPlugins() : GetLoadModeForMods());
            string basedirectory = (plugins ? PluginsDirectory : ModsDirectory);

            // DLLs
            string[] dlltbl = Directory.GetFiles(basedirectory, "*.dll");
            if (dlltbl.Length > 0)
                for (int i = 0; i < dlltbl.Length; i++)
                {
                    string filename = dlltbl[i];
                    if (string.IsNullOrEmpty(filename))
                        continue;
                    if (mode != LoadMode.BOTH)
                    {
                        bool file_extension_check = filename.EndsWith(".dev.dll");
                        if (((mode == LoadMode.NORMAL) && file_extension_check) || ((mode == LoadMode.DEV) && !file_extension_check))
                            continue;
                    }
                    string melonname = MelonUtils.GetFileProductName(filename);
                    if (string.IsNullOrEmpty(melonname))
                        melonname = Path.GetFileNameWithoutExtension(filename);
                    if (IsMelonAlreadyLoaded(melonname, plugins))
                    {
                        MelonLogger.Warning("Duplicate File: " + filename);
                        continue;
                    }
                    LoadFromFile(filename);
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
                                if (mode != LoadMode.BOTH)
                                {
                                    bool file_extension_check = filename2.EndsWith(".dev.dll");
                                    if (((mode == LoadMode.NORMAL) && file_extension_check) || ((mode == LoadMode.DEV) && !file_extension_check))
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
                                    LoadFromByteArray(memorystream.ToArray(), (filename + "/" + filename2));
                                }
                            }
                        }
                    }
                    catch(Exception ex) { MelonLogger.Error(ex.ToString()); }
                }
            _Plugins = _Plugins.OrderBy(x => x.Priority).ToList();
            _Mods = _Mods.OrderBy(x => x.Priority).ToList();
            DependencyGraph<MelonPlugin>.TopologicalSort(_Plugins);
            DependencyGraph<MelonMod>.TopologicalSort(_Mods);
        }

        public static bool IsMelonAlreadyLoaded(string name, bool is_plugin = false)
        {
            if (is_plugin)
                return (_Plugins.Find(x => x.Info.Name.Equals(name)) != null);
            return (_Mods.Find(x => x.Info.Name.Equals(name)) != null);
        }

        public static void LoadFromFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                return;
            if (!MelonDebug.IsEnabled())
            {
                LoadFromByteArray(File.ReadAllBytes(filepath), filepath);
                return;
            }
            try
            {
                Assembly asm = Assembly.LoadFrom(filepath);
                if (asm == null)
                {
                    MelonLogger.Error("Failed to Load Assembly for " + filepath + ": Assembly.LoadFrom returned null"); ;
                    return;
                }
                LoadFromAssembly(asm, filepath);
            }
            catch (Exception ex) { MelonLogger.Error("Failed to Load Assembly for " + filepath + ": " + ex.ToString()); }
        }

        public static void LoadFromByteArray(byte[] filedata, string filelocation = null)
        {
            if ((filedata == null) || (filedata.Length <= 0))
                return;
            try
            {
                byte[] symbols = { 0 };
                if (File.Exists(filelocation + ".mdb"))
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
                LoadFromAssembly(asm, filelocation);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(filelocation))
                    MelonLogger.Error("Failed to Load Assembly:" + ex.ToString());
                else
                    MelonLogger.Error("Failed to Load Assembly for " + filelocation + ": " + ex.ToString());
            }
        }

        public static void LoadFromAssembly(Assembly asm, string filelocation = null)
        {
            if (asm == null)
                return;
            MelonInfoAttribute infoAttribute = PullCustomAttributeFromAssembly<MelonInfoAttribute>(asm);
            if (infoAttribute == null) // Legacy Support
                infoAttribute = PullCustomAttributeFromAssembly<MelonModInfoAttribute>(asm)?.Convert();
            if (infoAttribute == null) // Legacy Support
                infoAttribute = PullCustomAttributeFromAssembly<MelonPluginInfoAttribute>(asm)?.Convert();
            if (infoAttribute == null)
            {
                MelonLogger.Error("No MelonInfoAttribute Found in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                return;
            }
            if (infoAttribute.SystemType == null)
            {
                MelonLogger.Error("Invalid Type given to MelonInfoAttribute in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                return;
            }
            bool is_plugin = infoAttribute.SystemType.IsSubclassOf(typeof(MelonPlugin));
            bool is_mod = infoAttribute.SystemType.IsSubclassOf(typeof(MelonMod));
            if (!is_plugin && !is_mod)
            {
                MelonLogger.Error("Invalid Type given to MelonInfoAttribute in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                return;
            }
            if (string.IsNullOrEmpty(infoAttribute.Name))
            {
                MelonLogger.Error("Invalid Name given to MelonInfoAttribute in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                return;
            }
            if (string.IsNullOrEmpty(infoAttribute.Version))
            {
                MelonLogger.Error("Invalid Version given to MelonInfoAttribute in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                return;
            }
            if (string.IsNullOrEmpty(infoAttribute.Author))
            {
                MelonLogger.Error("Invalid Author given to MelonInfoAttribute in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                return;
            }

            List<MelonGameAttribute> gameAttributes = new List<MelonGameAttribute>();
            MelonGameAttribute[] gameatt = PullCustomAttributesFromAssembly<MelonGameAttribute>(asm);
            if ((gameatt != null) && (gameatt.Length > 0))
                gameAttributes.AddRange(gameatt);

            MelonModGameAttribute[] legacymodgameAttributes = PullCustomAttributesFromAssembly<MelonModGameAttribute>(asm);
            if ((legacymodgameAttributes != null) && (legacymodgameAttributes.Length > 0))
                foreach (MelonModGameAttribute legacyatt in legacymodgameAttributes)
                    gameAttributes.Add(legacyatt.Convert());

            MelonPluginGameAttribute[] legacyplugingameAttributes = PullCustomAttributesFromAssembly<MelonPluginGameAttribute>(asm);
            if ((legacyplugingameAttributes != null) && (legacyplugingameAttributes.Length > 0))
                foreach (MelonPluginGameAttribute legacyatt in legacyplugingameAttributes)
                    gameAttributes.Add(legacyatt.Convert());

            MelonBase.MelonCompatibility melonCompatibility = MelonBase.MelonCompatibility.INCOMPATIBLE;
            if (gameAttributes.Count <= 0)
            {
                MelonLogger.Warning("No MelonGameAttribute Found in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                melonCompatibility = MelonBase.MelonCompatibility.NOATTRIBUTE;
            }
            else
                for (int i = 0; i < gameAttributes.Count; i++)
                {
                    MelonGameAttribute melonGameAttribute = gameAttributes[i];
                    if (melonGameAttribute == null)
                        continue;
                    if (melonGameAttribute.Universal)
                    {
                        melonCompatibility = MelonBase.MelonCompatibility.UNIVERSAL;
                        break;
                    }
                    if (MelonUtils.CurrentGameAttribute.IsCompatible(melonGameAttribute))
                    {
                        melonCompatibility = MelonBase.MelonCompatibility.COMPATIBLE;
                        break;
                    }
                }
            if (melonCompatibility == MelonBase.MelonCompatibility.INCOMPATIBLE)
            {
                MelonLogger.Error("Incompatible " + (is_plugin ? "Plugin" : "Mod") + " in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                return;
            }
            MelonBase baseInstance = Activator.CreateInstance(infoAttribute.SystemType) as MelonBase;
            if (baseInstance == null)
            {
                MelonLogger.Error("Failed to Create Instance for" + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                return;
            }

            MelonBase.MelonPriority priority = MelonBase.MelonPriority.NORMAL;
            MelonPriorityAttribute priorityatt = PullCustomAttributeFromAssembly<MelonPriorityAttribute>(asm);
            if (priorityatt != null)
                priority = priorityatt.Priority;

            ConsoleColor color = MelonLogger.DefaultMelonColor;
            MelonColorAttribute coloratt = PullCustomAttributeFromAssembly<MelonColorAttribute>(asm);
            if (coloratt != null)
                color = coloratt.Color;

            baseInstance.Info = infoAttribute;
            baseInstance.Games = gameAttributes.ToArray();
            baseInstance.ConsoleColor = color;
            baseInstance.OptionalDependencies = PullCustomAttributeFromAssembly<MelonOptionalDependenciesAttribute>(asm);
            baseInstance.Location = filelocation;
            baseInstance.Compatibility = melonCompatibility;
            baseInstance.Priority = priority;
            baseInstance.Assembly = asm;
            baseInstance.Harmony = Harmony.HarmonyInstance.Create(asm.FullName);
            if (is_plugin)
                _Plugins.Add((MelonPlugin)baseInstance);
            else if (is_mod)
                _Mods.Add((MelonMod)baseInstance);
            try {
                baseInstance.Harmony.PatchAll(asm);
            } catch (Exception) {
                if (is_plugin)
                    _Plugins.Remove((MelonPlugin)baseInstance);
                else if (is_mod)
                    _Mods.Remove((MelonMod)baseInstance);
                throw;
            }
        }

        private static T PullCustomAttributeFromAssembly<T>(Assembly asm) where T : Attribute
        {
            T[] attributetbl = PullCustomAttributesFromAssembly<T>(asm);
            if ((attributetbl == null) || (attributetbl.Count() <= 0))
                return null;
            return attributetbl.First();
        }

        private static T[] PullCustomAttributesFromAssembly<T>(Assembly asm) where T : Attribute
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

        internal static void OnPreInitialization()
        {
            if (_Plugins.Count <= 0)
                return;
            List<MelonPlugin> failedPlugins = new List<MelonPlugin>();
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.OnPreInitialization(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); failedPlugins.Add(PluginEnumerator.Current); }
            _Plugins.RemoveAll(failedPlugins.Contains);
            _Plugins = _Plugins.OrderBy(x => x.Priority).ToList();
            DependencyGraph<MelonPlugin>.TopologicalSort(_Plugins);
        }

        internal static void OnApplicationStart_Plugins()
        {
            if (_Plugins.Count <= 0)
                return;
            List<MelonPlugin> failedPlugins = new List<MelonPlugin>();
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.OnApplicationStart(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); failedPlugins.Add(PluginEnumerator.Current); }
            _Plugins.RemoveAll(failedPlugins.Contains);
            _Plugins = _Plugins.OrderBy(x => x.Priority).ToList();
            DependencyGraph<MelonPlugin>.TopologicalSort(_Plugins);
        }

        internal static void OnApplicationStart_Mods()
        {
            if (_Mods.Count <= 0)
                return;
            List<MelonMod> failedMods = new List<MelonMod>();
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnApplicationStart(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); failedMods.Add(ModEnumerator.Current); }
            _Mods.RemoveAll(failedMods.Contains);
            _Mods = _Mods.OrderBy(x => x.Priority).ToList();
            DependencyGraph<MelonMod>.TopologicalSort(_Mods);
        }


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
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnLevelWasLoaded(buildIndex); ModEnumerator.Current.OnSceneWasLoaded(buildIndex, sceneName); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnLevelWasInitialized(buildIndex); ModEnumerator.Current.OnSceneWasInitialized(buildIndex, sceneName); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

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
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.OnUpdate(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); }
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnUpdate(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void OnFixedUpdate()
        {
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnFixedUpdate(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void OnLateUpdate()
        {
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.OnLateUpdate(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); }
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnLateUpdate(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void OnGUI()
        {
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.OnGUI(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); }
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnGUI(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void OnPreferencesSaved()
        {
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.OnPreferencesSaved(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); }
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnModSettingsApplied(); ModEnumerator.Current.OnPreferencesSaved(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void OnPreferencesLoaded()
        {
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.OnPreferencesLoaded(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); }
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnPreferencesLoaded(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void OnApplicationQuit()
        {
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.OnApplicationQuit(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); }
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.OnApplicationQuit(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void VRChat_OnUiManagerInit()
        {
            MelonPluginEnumerator PluginEnumerator = new MelonPluginEnumerator();
            while (PluginEnumerator.MoveNext())
                try { PluginEnumerator.Current.VRChat_OnUiManagerInit(); } catch (Exception ex) { MelonLogger.ManualMelonError(PluginEnumerator.Current, ex.ToString()); }
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.VRChat_OnUiManagerInit(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal static void BONEWORKS_OnLoadingScreen()
        {
            MelonModEnumerator ModEnumerator = new MelonModEnumerator();
            while (ModEnumerator.MoveNext())
                try { ModEnumerator.Current.BONEWORKS_OnLoadingScreen(); } catch (Exception ex) { MelonLogger.ManualMelonError(ModEnumerator.Current, ex.ToString()); }
        }

        internal enum LoadMode
        {
            NORMAL,
            DEV,
            BOTH
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static LoadMode GetLoadModeForPlugins();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static LoadMode GetLoadModeForMods();
    }
}
