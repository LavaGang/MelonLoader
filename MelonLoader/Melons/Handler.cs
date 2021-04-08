#if PORT_DISABLE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                ConsoleColor color = MelonLogger.DefaultMelonColor;
                if (plugin.Color != null)
                    color = plugin.Color.Color;
                MelonLogger.Internal_PrintModName(color, plugin.Info.Name, plugin.Info.Version);
                MelonLogger.Msg("by " + plugin.Info.Author);
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
                ConsoleColor color = MelonLogger.DefaultMelonColor;
                if (mod.Color != null)
                    color = mod.Color.Color;
                MelonLogger.Internal_PrintModName(color, mod.Info.Name, mod.Info.Version);
                MelonLogger.Msg("by " + mod.Info.Author);
                MelonLogger.Msg("------------------------------");
            }
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
                        return;
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
                    MelonLogger.Error("Failed to Load Assembly for " + filepath + "!"); ;
                    return;
                }
                LoadFromAssembly(asm, filepath);
            }
            catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        public static void LoadFromByteArray(byte[] filedata, string filelocation = null)
        {
            if ((filedata == null) || (filedata.Length <= 0))
                return;
            try
            {
                Assembly asm = Assembly.Load(filedata);
                if (asm == null)
                {
                    if (string.IsNullOrEmpty(filelocation))
                        MelonLogger.Error("Failed to Load Assembly!");
                    else
                        MelonLogger.Error("Failed to Load Assembly for " + filelocation + "!");
                    return;
                }
                LoadFromAssembly(asm, filelocation);
            }
            catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        public static void LoadFromAssembly(Assembly asm, string filelocation = null)
        {
            if (asm == null)
                return;
            MelonInfoAttribute infoAttribute = asm.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonInfoAttribute))) as MelonInfoAttribute;
            if (infoAttribute == null) // Legacy Support
            {
                MelonModInfoAttribute legacyinfoAttribute = asm.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonModInfoAttribute))) as MelonModInfoAttribute;
                if (legacyinfoAttribute != null)
                    infoAttribute = legacyinfoAttribute.Convert();
            }
            if (infoAttribute == null) // Legacy Support
            {
                MelonPluginInfoAttribute legacyinfoAttribute = asm.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonPluginInfoAttribute))) as MelonPluginInfoAttribute;
                if (legacyinfoAttribute != null)
                    infoAttribute = legacyinfoAttribute.Convert();
            }
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

            MelonGameAttribute[] gameAttributes = asm.GetCustomAttributes(typeof(MelonGameAttribute), true) as MelonGameAttribute[];
            
            // Legacy Support
            MelonModGameAttribute[] legacymodgameAttributes = asm.GetCustomAttributes(typeof(MelonModGameAttribute), true) as MelonModGameAttribute[];
            if (legacymodgameAttributes.Length > 0)
            {
                List<MelonGameAttribute> attributes = new List<MelonGameAttribute>();
                attributes.AddRange(gameAttributes);
                foreach (MelonModGameAttribute legacyatt in legacymodgameAttributes)
                    attributes.Add(legacyatt.Convert());
                gameAttributes = attributes.ToArray();
            }
            MelonPluginGameAttribute[] legacyplugingameAttributes = asm.GetCustomAttributes(typeof(MelonPluginGameAttribute), true) as MelonPluginGameAttribute[];
            if (legacyplugingameAttributes.Length > 0)
            {
                List<MelonGameAttribute> attributes = new List<MelonGameAttribute>();
                attributes.AddRange(gameAttributes);
                foreach (MelonPluginGameAttribute legacyatt in legacyplugingameAttributes)
                    attributes.Add(legacyatt.Convert());
                gameAttributes = attributes.ToArray();
            }

            MelonBase.MelonCompatibility melonCompatibility = MelonBase.MelonCompatibility.INCOMPATIBLE;
            if (gameAttributes.Length <= 0)
            {
                MelonLogger.Warning("No MelonGameAttribute Found in " + ((filelocation != null) ? filelocation : asm.GetName().Name) + "!");
                melonCompatibility = MelonBase.MelonCompatibility.NOATTRIBUTE;
            }
            else
                for (int i = 0; i < gameAttributes.Length; i++)
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
            baseInstance.Info = infoAttribute;
            baseInstance.Games = gameAttributes;
            baseInstance.Color = asm.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonColorAttribute))) as MelonColorAttribute;
            baseInstance.OptionalDependencies = asm.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonOptionalDependenciesAttribute))) as MelonOptionalDependenciesAttribute;
            baseInstance.Location = filelocation;
            baseInstance.Compatibility = melonCompatibility;
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

        internal static void OnPreInitialization()
        {
            if (_Plugins.Count <= 0)
                return;
            List<MelonPlugin> failedPlugins = new List<MelonPlugin>();
            foreach (MelonPlugin plugin in _Plugins)
                try { plugin.OnPreInitialization(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); failedPlugins.Add(plugin); }
            _Plugins.RemoveAll(failedPlugins.Contains);
            DependencyGraph<MelonPlugin>.TopologicalSort(_Plugins);
        }

        internal static void OnApplicationStart_Plugins()
        {
            if (_Plugins.Count <= 0)
                return;
            List<MelonPlugin> failedPlugins = new List<MelonPlugin>();
            foreach (MelonPlugin plugin in _Plugins)
                try { plugin.OnApplicationStart(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); failedPlugins.Add(plugin); }
            _Plugins.RemoveAll(failedPlugins.Contains);
            DependencyGraph<MelonPlugin>.TopologicalSort(_Plugins);
        }

        internal static void OnApplicationStart_Mods()
        {
            if (_Mods.Count <= 0)
                return;
            List<MelonMod> failedMods = new List<MelonMod>();
            foreach (MelonMod mod in _Mods)
                try { mod.OnApplicationStart(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); failedMods.Add(mod); }
            _Mods.RemoveAll(failedMods.Contains);
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
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnLevelWasLoaded(buildIndex); mod.OnSceneWasLoaded(buildIndex, sceneName); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnLevelWasInitialized(buildIndex); mod.OnSceneWasInitialized(buildIndex, sceneName); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
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
            if (_Plugins.Count > 0)
                foreach (MelonPlugin plugin in _Plugins)
                    try { plugin.OnUpdate(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnUpdate(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void OnFixedUpdate()
        {
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnFixedUpdate(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void OnLateUpdate()
        {
            if (_Plugins.Count > 0)
                foreach (MelonPlugin plugin in _Plugins)
                    try { plugin.OnLateUpdate(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnLateUpdate(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void OnGUI()
        {
            if (_Plugins.Count > 0)
                foreach (MelonPlugin plugin in _Plugins)
                    try { plugin.OnGUI(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnGUI(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void OnPreferencesSaved()
        {
            if (_Plugins.Count > 0)
                foreach (MelonPlugin plugin in _Plugins)
                    try { plugin.OnPreferencesSaved(); plugin.OnModSettingsApplied(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnPreferencesSaved(); mod.OnModSettingsApplied(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void OnPreferencesLoaded()
        {
            if (_Plugins.Count > 0)
                foreach (MelonPlugin plugin in _Plugins)
                    try { plugin.OnPreferencesLoaded(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnPreferencesLoaded(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void OnApplicationQuit()
        {
            if (_Plugins.Count > 0)
                foreach (MelonPlugin plugin in _Plugins)
                    try { plugin.OnApplicationQuit(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.OnApplicationQuit(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void VRChat_OnUiManagerInit()
        {
            if (_Plugins.Count > 0)
                foreach (MelonPlugin plugin in _Plugins)
                    try { plugin.VRChat_OnUiManagerInit(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.VRChat_OnUiManagerInit(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal static void BONEWORKS_OnLoadingScreen()
        {
            if (_Mods.Count > 0)
                foreach (MelonMod mod in _Mods)
                    try { mod.BONEWORKS_OnLoadingScreen(); } catch (Exception ex) { MelonLogger.Error(ex.ToString()); }
        }

        internal enum LoadMode
        {
            NORMAL,
            DEV,
            BOTH
        }
    }
}
#endif