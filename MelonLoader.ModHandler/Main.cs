using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
#pragma warning disable 0618

namespace MelonLoader
{
    public static class Main
    {
        public static List<MelonMod> Mods = new List<MelonMod>();
        public static List<MelonPlugin> Plugins = new List<MelonPlugin>();
        public static List<MelonPlugin> TempPlugins = null;
        internal static MelonModGameAttribute CurrentGameAttribute = null;
        public static bool IsVRChat = false;
        public static bool IsBoneworks = false;
        public static string UnityVersion = null;
        private static Assembly Assembly_CSharp = null;
        private static bool HasGeneratedAssembly = false;

        private static void Initialize()
        {
            if (string.IsNullOrEmpty(AppDomain.CurrentDomain.BaseDirectory))
                ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AppDomain.CurrentDomain, new object[0])).ApplicationBase = Imports.GetGameDirectory();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;

            CurrentGameAttribute = new MelonModGameAttribute(Imports.GetCompanyName(), Imports.GetProductName());
            UnityVersion = GetUnityVersion();

            if (Imports.IsIl2CppGame())
            {
                IsVRChat = CurrentGameAttribute.IsGame("VRChat", "VRChat");
                IsBoneworks = CurrentGameAttribute.IsGame("Stress Level Zero", "BONEWORKS");
            }

            if (!Imports.IsDebugMode()
#if !DEBUG
                && Imports.IsConsoleEnabled()
#endif
                )
            {
                Console.Enabled = true;
                Console.Create();
            }

            if (!Imports.IsIl2CppGame() || AssemblyGenerator.Main.Initialize())
                HasGeneratedAssembly = true;
            else
                Imports.UNLOAD_MELONLOADER();

            if (HasGeneratedAssembly)
            {
                LoadDLLs(true);
                if (Plugins.Count > 0)
                {
                    HashSet<MelonPlugin> failedPlugins = new HashSet<MelonPlugin>();
                    TempPlugins = Plugins.Where(plugin => (plugin.Compatibility < MelonBase.MelonCompatibility.INCOMPATIBLE)).ToList();
                    DependencyGraph<MelonPlugin>.TopologicalSort(TempPlugins, plugin => plugin.InfoAttribute.Name);
                    for (int i = 0; i < TempPlugins.Count; i++)
                    {
                        MelonPlugin plugin = TempPlugins[i];
                        if (plugin != null)
                            try { plugin.OnPreInitialization(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), plugin.InfoAttribute.Name); failedPlugins.Add(plugin); }
                    }
                    TempPlugins.RemoveAll(plugin => failedPlugins.Contains(plugin));
                }
            }
        }

        private static void OnApplicationStart()
        {
            if (!HasGeneratedAssembly)
                return;

            if (Imports.IsIl2CppGame())
            {
                if (IsVRChat)
                    Assembly_CSharp = Assembly.Load("Assembly-CSharp");
                UnhollowerSupport.Initialize();
            }
            SupportModule.Initialize();

            MelonModLogger.Log("------------------------------");
            MelonModLogger.Log("Unity " + UnityVersion);
            MelonModLogger.Log("OS: " + Environment.OSVersion.ToString());
            MelonModLogger.Log("------------------------------");
            MelonModLogger.Log("Name: " + CurrentGameAttribute.GameName);
            MelonModLogger.Log("Developer: " + CurrentGameAttribute.Developer);
            MelonModLogger.Log("Type: " + (Imports.IsIl2CppGame() ? "Il2Cpp" : (Imports.IsOldMono() ? "Mono" : "MonoBleedingEdge")));
            MelonModLogger.Log("------------------------------");
            MelonModLogger.Log("Using v" + BuildInfo.Version + " Open-Beta");
            MelonModLogger.Log("------------------------------");

            LoadDLLs();
            if (Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MelonPlugin plugin = Plugins[i];
                    if (plugin != null)
                    {
                        MelonModLogger.Log(plugin.InfoAttribute.Name
                            + (!string.IsNullOrEmpty(plugin.InfoAttribute.Version)
                            ? (" v" + plugin.InfoAttribute.Version) : "")
                            + (!string.IsNullOrEmpty(plugin.InfoAttribute.Author)
                            ? (" by " + plugin.InfoAttribute.Author) : "")
                            + (!string.IsNullOrEmpty(plugin.InfoAttribute.DownloadLink)
                            ? (" (" + plugin.InfoAttribute.DownloadLink + ")")
                            : "")
                            );
                        MelonModLogger.LogDLLStatus(plugin.Compatibility);
                        MelonModLogger.Log("------------------------------");
                    }
                }
                Plugins = TempPlugins;
            }
            if (Plugins.Count <= 0)
            {
                MelonModLogger.Log("No Plugins Loaded!");
                MelonModLogger.Log("------------------------------");
            }

            if (Mods.Count > 0)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                    {
                        MelonModLogger.Log(mod.InfoAttribute.Name
                            + (!string.IsNullOrEmpty(mod.InfoAttribute.Version)
                            ? (" v" + mod.InfoAttribute.Version) : "")
                            + (!string.IsNullOrEmpty(mod.InfoAttribute.Author)
                            ? (" by " + mod.InfoAttribute.Author) : "")
                            + (!string.IsNullOrEmpty(mod.InfoAttribute.DownloadLink)
                            ? (" (" + mod.InfoAttribute.DownloadLink + ")")
                            : "")
                            );
                        MelonModLogger.LogDLLStatus(mod.Compatibility);
                        MelonModLogger.Log("------------------------------");
                    }
                }
                Mods.RemoveAll((MelonMod mod) => (mod.Compatibility >= MelonBase.MelonCompatibility.INCOMPATIBLE));
                DependencyGraph<MelonMod>.TopologicalSort(Mods, mod => mod.InfoAttribute.Name);
            }
            if (Mods.Count <= 0)
            {
                MelonModLogger.Log("No Mods Loaded!");
                MelonModLogger.Log("------------------------------");
            }

            if ((Plugins.Count > 0) || (Mods.Count > 0))
                AddUnityDebugLog();

            if (Plugins.Count > 0)
            {
                HashSet<MelonPlugin> failedPlugins = new HashSet<MelonPlugin>();
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MelonPlugin plugin = Plugins[i];
                    if (plugin != null)
                        try { InitializeModOrPlugin(plugin); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), plugin.InfoAttribute.Name); failedPlugins.Add(plugin); }
                }
                Plugins.RemoveAll(plugin => failedPlugins.Contains(plugin));
            }

            if (Mods.Count > 0)
            {
                HashSet<MelonMod> failedMods = new HashSet<MelonMod>();
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { InitializeModOrPlugin(mod); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); failedMods.Add(mod); }
                }
                Mods.RemoveAll(mod => failedMods.Contains(mod));
            }

            if ((Plugins.Count <= 0) && (Mods.Count <= 0))
                SupportModule.Destroy();
        }

        private static void InitializeModOrPlugin(MelonBase modOrPlugin) {
            string harmonyId = modOrPlugin.Assembly.FullName;
            HarmonyInstance harmony = HarmonyInstance.Create(harmonyId);
            try
            {
                harmony.PatchAll(modOrPlugin.Assembly);
                modOrPlugin.OnApplicationStart();
            }
            catch (Exception)
            {
                harmony.UnpatchAll(harmonyId);
                throw;
            }
        }

        public static void OnApplicationQuit()
        {
            if (Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MelonPlugin plugin = Plugins[i];
                    if (plugin != null)
                        try { plugin.OnApplicationQuit(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), plugin.InfoAttribute.Name); }
                }
            }
            if (Mods.Count > 0)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnApplicationQuit(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
            }
            if ((Plugins.Count > 0) || (Mods.Count > 0))
                ModPrefs.SaveConfig();
            Harmony.HarmonyInstance.UnpatchAllInstances();
            Imports.UNLOAD_MELONLOADER();
            if (Imports.IsQuitFix()) Process.GetCurrentProcess().Kill();
        }

        public static void OnModSettingsApplied()
        {
            if (Plugins.Count > 0)
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MelonPlugin plugin = Plugins[i];
                    if (plugin != null)
                        try { plugin.OnModSettingsApplied(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), plugin.InfoAttribute.Name); }
                }
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnModSettingsApplied(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        public static void OnUpdate()
        {
            SceneHandler.CheckForSceneChange();
            if (Imports.IsIl2CppGame() && IsVRChat)
                VRChat_CheckUiManager();
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnUpdate(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        public static void OnFixedUpdate()
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnFixedUpdate(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        public static void OnLateUpdate()
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLateUpdate(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        public static void OnGUI()
        {
            if (Mods.Count > 0)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnGUI(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
            }
        }

        internal static void OnLevelIsLoading()
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLevelIsLoading(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        internal static void OnLevelWasLoaded(int level)
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLevelWasLoaded(level); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        internal static void OnLevelWasInitialized(int level)
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLevelWasInitialized(level); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        private static bool ShouldCheckForUiManager = true;
        private static Type VRCUiManager = null;
        private static PropertyInfo VRCUiManager_Instance = null;
        private static void VRChat_CheckUiManager()
        {
            if (ShouldCheckForUiManager)
            {
                if (VRCUiManager == null)
                    VRCUiManager = Assembly_CSharp.GetType("VRCUiManager");
                if (VRCUiManager != null)
                {
                    if (VRCUiManager_Instance == null)
                        VRCUiManager_Instance = VRCUiManager.GetProperty("field_Protected_Static_VRCUiManager_0");
                    if (VRCUiManager_Instance != null)
                    {
                        object returnval = VRCUiManager_Instance.GetValue(null, new object[0]);
                        if (returnval != null)
                        {
                            ShouldCheckForUiManager = false;
                            if (Mods.Count > 0)
                                for (int i = 0; i < Mods.Count; i++)
                                {
                                    MelonMod mod = Mods[i];
                                    if (mod != null)
                                        try { mod.VRChat_OnUiManagerInit(); } catch (Exception ex) { MelonModLogger.LogDLLError(ex.ToString(), mod.InfoAttribute.Name); }
                                }
                        }
                    }
                }
            }
        }

        private static void LoadDLLs(bool plugins = false)
        {
            string searchdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (plugins ? "Plugins" : "Mods"));
            if (!Directory.Exists(searchdir))
                Directory.CreateDirectory(searchdir);
            else
            {
                // DLL
                string[] files = Directory.GetFiles(searchdir, "*.dll");
                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        string file = files[i];
                        if (!string.IsNullOrEmpty(file))
                        {
                            if (plugins)
                            {
                                if ((Imports.IsDevPluginsOnly() && !file.EndsWith("-dev.dll")) || (!Imports.IsDevPluginsOnly() && file.EndsWith("-dev.dll")))
                                    continue;
                            }
                            else
                            {
                                if ((Imports.IsDevModsOnly() && !file.EndsWith("-dev.dll")) || (!Imports.IsDevModsOnly() && file.EndsWith("-dev.dll")))
                                    continue;
                            }
                            try
                            {
                                LoadAssembly(File.ReadAllBytes(file), plugins, file);
                            }
                            catch (Exception e)
                            {
                                MelonModLogger.LogError("Unable to load " + file + ":\n" + e.ToString());
                                MelonModLogger.Log("------------------------------");
                            }
                        }
                    }
                }

                // ZIP
                string[] zippedFiles = Directory.GetFiles(searchdir, "*.zip");
                if (zippedFiles.Length > 0)
                {
                    for (int i = 0; i < zippedFiles.Length; i++)
                    {
                        string file = zippedFiles[i];
                        if (!string.IsNullOrEmpty(file))
                        {
                            try
                            {
                                using (var fileStream = File.OpenRead(file))
                                {
                                    using (var zipInputStream = new ZipInputStream(fileStream))
                                    {
                                        ZipEntry entry;
                                        while ((entry = zipInputStream.GetNextEntry()) != null)
                                        {
                                            string filename = Path.GetFileName(entry.Name);
                                            if (string.IsNullOrEmpty(filename) || !filename.EndsWith(".dll"))
                                                continue;

                                            if (plugins)
                                            {
                                                if ((Imports.IsDevPluginsOnly() && !filename.EndsWith("-dev.dll")) || (!Imports.IsDevPluginsOnly() && filename.EndsWith("-dev.dll")))
                                                    continue;
                                            }
                                            else
                                            {
                                                if ((Imports.IsDevModsOnly() && !filename.EndsWith("-dev.dll")) || (!Imports.IsDevModsOnly() && filename.EndsWith("-dev.dll")))
                                                    continue;
                                            }

                                            using (var unzippedFileStream = new MemoryStream())
                                            {
                                                int size = 0;
                                                byte[] buffer = new byte[4096];
                                                while (true)
                                                {
                                                    size = zipInputStream.Read(buffer, 0, buffer.Length);
                                                    if (size > 0)
                                                        unzippedFileStream.Write(buffer, 0, size);
                                                    else
                                                        break;
                                                }
                                                LoadAssembly(unzippedFileStream.ToArray(), plugins, (file + "/" + filename));
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                MelonModLogger.LogError("Unable to load " + file + ":\n" + e.ToString());
                                MelonModLogger.Log("------------------------------");
                            }
                        }
                    }
                }
            }
        }

        private static void LoadDLLFromAssembly(Assembly assembly, bool isPlugin = false, string filelocation = null)
        {
            if (isPlugin)
                LoadPluginFromAssembly(assembly, filelocation);
            else
                LoadModFromAssembly(assembly, filelocation);
        }

        private static void LoadPluginFromAssembly(Assembly assembly, string filelocation = null)
        {
            MelonPluginInfoAttribute pluginInfoAttribute = assembly.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonPluginInfoAttribute))) as MelonPluginInfoAttribute;
            if ((pluginInfoAttribute != null) && (pluginInfoAttribute.SystemType != null) && pluginInfoAttribute.SystemType.IsSubclassOf(typeof(MelonPlugin)))
            {
                bool isCompatible = false;
                bool isUniversal = false;
                bool hasAttribute = true;
                MelonPluginGameAttribute[] pluginGameAttributes = assembly.GetCustomAttributes(typeof(MelonPluginGameAttribute), true) as MelonPluginGameAttribute[];
                int pluginGameAttributes_Count = pluginGameAttributes.Length;
                if (pluginGameAttributes_Count > 0)
                {
                    for (int i = 0; i < pluginGameAttributes_Count; i++)
                    {
                        MelonPluginGameAttribute pluginGameAttribute = pluginGameAttributes[i];
                        if (CurrentGameAttribute.IsCompatible(pluginGameAttribute))
                        {
                            isCompatible = true;
                            isUniversal = CurrentGameAttribute.IsCompatibleBecauseUniversal(pluginGameAttribute);
                            break;
                        }
                    }
                }
                else
                    hasAttribute = false;
                try
                {
                    MelonPlugin pluginInstance = Activator.CreateInstance(pluginInfoAttribute.SystemType) as MelonPlugin;
                    if (pluginInstance != null)
                    {
                        pluginInstance.InfoAttribute = pluginInfoAttribute;
                        if (pluginGameAttributes_Count > 0)
                            pluginInstance.GameAttributes = pluginGameAttributes;
                        else
                            pluginInstance.GameAttributes = null;
                        pluginInstance.Location = filelocation;
                        pluginInstance.Compatibility = (isUniversal ? MelonBase.MelonCompatibility.UNIVERSAL : (isCompatible ? MelonBase.MelonCompatibility.COMPATIBLE : (!hasAttribute ? MelonBase.MelonCompatibility.NOATTRIBUTE : MelonBase.MelonCompatibility.INCOMPATIBLE)));
                        if (pluginInstance.Compatibility < MelonBase.MelonCompatibility.INCOMPATIBLE)
                        {
                            pluginInstance.Assembly = assembly;
                        }
                        Plugins.Add(pluginInstance);
                    }
                    else
                        MelonModLogger.LogError("Unable to load Plugin in " + assembly.GetName() + "! Failed to Create Instance!");
                }
                catch (Exception e) { MelonModLogger.LogError("Unable to load Plugin in " + assembly.GetName() + "! " + e.ToString()); }
            }
        }

        private static void LoadModFromAssembly(Assembly assembly, string filelocation = null)
        {
            MelonModInfoAttribute modInfoAttribute = assembly.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonModInfoAttribute))) as MelonModInfoAttribute;
            if ((modInfoAttribute != null) && (modInfoAttribute.SystemType != null) && modInfoAttribute.SystemType.IsSubclassOf(typeof(MelonMod)))
            {
                bool isCompatible = false;
                bool isUniversal = false;
                bool hasAttribute = true;
                MelonModGameAttribute[] modGameAttributes = assembly.GetCustomAttributes(typeof(MelonModGameAttribute), true) as MelonModGameAttribute[];
                int modGameAttributes_Count = modGameAttributes.Length;
                if (modGameAttributes_Count > 0)
                {
                    for (int i = 0; i < modGameAttributes_Count; i++)
                    {
                        MelonModGameAttribute modGameAttribute = modGameAttributes[i];
                        if (CurrentGameAttribute.IsCompatible(modGameAttribute))
                        {
                            isCompatible = true;
                            isUniversal = CurrentGameAttribute.IsCompatibleBecauseUniversal(modGameAttribute);
                            break;
                        }
                    }
                }
                else
                    hasAttribute = false;
                try
                {
                    MelonMod modInstance = Activator.CreateInstance(modInfoAttribute.SystemType) as MelonMod;
                    if (modInstance != null)
                    {
                        modInstance.InfoAttribute = modInfoAttribute;
                        if (modGameAttributes_Count > 0)
                            modInstance.GameAttributes = modGameAttributes;
                        else
                            modInstance.GameAttributes = null;
                        modInstance.Location = filelocation;
                        modInstance.Compatibility = (isUniversal ? MelonBase.MelonCompatibility.UNIVERSAL : (isCompatible ? MelonBase.MelonCompatibility.COMPATIBLE : (!hasAttribute ? MelonBase.MelonCompatibility.NOATTRIBUTE : MelonBase.MelonCompatibility.INCOMPATIBLE)));
                        if (modInstance.Compatibility < MelonBase.MelonCompatibility.INCOMPATIBLE)
                        {
                            modInstance.Assembly = assembly;
                        }
                        Mods.Add(modInstance);
                    }
                    else
                        MelonModLogger.LogError("Unable to load Mod in " + assembly.GetName() + "! Failed to Create Instance!");
                }
                catch (Exception e) { MelonModLogger.LogError("Unable to load Mod in " + assembly.GetName() + "! " + e.ToString()); }
            }
        }

        private static void LoadAssembly(string file, bool isPlugin = false) => LoadDLLFromAssembly(Assembly.LoadFrom(file), isPlugin, file);
        private static void LoadAssembly(byte[] data, bool isPlugin = false, string filelocation = null) => LoadDLLFromAssembly(Assembly.Load(data), isPlugin, filelocation);
        private static void LoadAssembly(Assembly asm, bool isPlugin = false, string filelocation = null)
        {
            if (!asm.Equals(null))
                LoadDLLFromAssembly(asm, isPlugin, filelocation);
            else
                MelonModLogger.LogError("Unable to load " + asm);
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs e) => MelonModLogger.LogError((e.ExceptionObject as Exception).ToString());
        
        internal static string GetUnityVersion()
        {
            string exepath = Imports.GetExePath();
            string ggm_path = Path.Combine(Imports.GetGameDataDirectory(), "globalgamemanagers");
            if (!File.Exists(ggm_path))
            {
                FileVersionInfo versioninfo = FileVersionInfo.GetVersionInfo(exepath);
                if ((versioninfo == null) || string.IsNullOrEmpty(versioninfo.FileVersion))
                    return "UNKNOWN";
                return versioninfo.FileVersion.Substring(0, versioninfo.FileVersion.LastIndexOf('.'));
            }
            else
            {
                byte[] ggm_bytes = File.ReadAllBytes(ggm_path);
                if ((ggm_bytes == null) || (ggm_bytes.Length <= 0))
                    return "UNKNOWN";
                int start_position = 0;
                for (int i = 10; i < ggm_bytes.Length; i++)
                {
                    byte pos_byte = ggm_bytes[i];
                    if ((pos_byte <= 0x39) && (pos_byte >= 0x30))
                    {
                        start_position = i;
                        break;
                    }
                }
                if (start_position == 0)
                    return "UNKNOWN";
                int end_position = 0;
                for (int i = start_position; i < ggm_bytes.Length; i++)
                {
                    byte pos_byte = ggm_bytes[i];
                    if ((pos_byte != 0x2E) && ((pos_byte > 0x39) || (pos_byte < 0x30)))
                    {
                        end_position = (i - 1);
                        break;
                    }
                }
                if (end_position == 0)
                    return "UNKNOWN";
                int verstr_byte_pos = 0;
                byte[] verstr_byte = new byte[((end_position - start_position) + 1)];
                for (int i = start_position; i <= end_position; i++)
                {
                    verstr_byte[verstr_byte_pos] = ggm_bytes[i];
                    verstr_byte_pos++;
                }
                return Encoding.UTF8.GetString(verstr_byte, 0, verstr_byte.Length);
            }
        }

        private static void AddUnityDebugLog()
        {
            SupportModule.UnityDebugLog("--------------------------------------------------------------------------------------------------");
            SupportModule.UnityDebugLog("~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Developers!   ~");
            SupportModule.UnityDebugLog("--------------------------------------------------------------------------------------------------");
        }

        public static string GetUserDataPath()
        {
            string userDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserData");
            if (!Directory.Exists(userDataDir))
                Directory.CreateDirectory(userDataDir);
            return userDataDir;
        }
    }
}