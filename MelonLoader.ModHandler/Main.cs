using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
#pragma warning disable 0618

namespace MelonLoader
{
    public static class Main
    {
        public static List<MelonMod> Mods = new List<MelonMod>();
        public static List<MelonPlugin> Plugins = new List<MelonPlugin>();
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
            UnityVersion = GetUnityFileVersion();

            if (Imports.IsIl2CppGame())
            {
                IsVRChat = CurrentGameAttribute.IsGame("VRChat", "VRChat");
                IsBoneworks = CurrentGameAttribute.IsGame("Stress Level Zero", "BONEWORKS");
            }

            if (!Imports.IsDebugMode() && Imports.IsConsoleEnabled())
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
                    for (int i = 0; i < Plugins.Count; i++)
                    {
                        MelonPlugin plugin = Plugins[i];
                        if (plugin != null)
                            try { plugin.OnPreInitialization(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), plugin.InfoAttribute.Name); }
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
            ModSettingsMenu.Main.Setup();

            MelonModLogger.Log("------------------------------");
            MelonModLogger.Log("Unity " + UnityVersion);
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
                        MelonModLogger.LogModStatus((plugin.GameAttributes.Any()) ? (plugin.IsUniversal ? 0 : 1) : 2);
                        MelonModLogger.Log("------------------------------");
                    }
                }
            }
            else
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
                        MelonModLogger.LogModStatus((mod.GameAttributes.Any()) ? (mod.IsUniversal ? 0 : 1) : 2);
                        MelonModLogger.Log("------------------------------");
                    }
                }
            }
            else
            {
                MelonModLogger.Log("No Mods Loaded!");
                MelonModLogger.Log("------------------------------");
            }

            if ((Plugins.Count > 0) && (Mods.Count > 0))
                AddUnityDebugLog();

            if (Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MelonPlugin plugin = Plugins[i];
                    if (plugin != null)
                        try { plugin.OnApplicationStart(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), plugin.InfoAttribute.Name); }
                }
            }

            if (Mods.Count > 0)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnApplicationStart(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
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
                        try { plugin.OnApplicationQuit(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), plugin.InfoAttribute.Name); }
                }
            }
            if (Mods.Count() > 0)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnApplicationQuit(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
            }
            if ((Plugins.Count > 0) || (Mods.Count() > 0))
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
                        try { plugin.OnModSettingsApplied(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), plugin.InfoAttribute.Name); }
                }
            if (Mods.Count() > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnModSettingsApplied(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        public static void OnUpdate()
        {
            SceneHandler.CheckForSceneChange();
            if (Imports.IsIl2CppGame() && IsVRChat)
                VRChat_CheckUiManager();
            if (Mods.Count() > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnUpdate(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        public static void OnFixedUpdate()
        {
            if (Mods.Count() > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnFixedUpdate(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        public static void OnLateUpdate()
        {
            if (Mods.Count() > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLateUpdate(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        public static void OnGUI()
        {
            if (Mods.Count() > 0)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnGUI(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
            }
            ModSettingsMenu.Main.Render();
        }

        internal static void OnLevelIsLoading()
        {
            if (Mods.Count() > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLevelIsLoading(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        internal static void OnLevelWasLoaded(int level)
        {
            if (Mods.Count() > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLevelWasLoaded(level); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                }
        }

        internal static void OnLevelWasInitialized(int level)
        {
            if (Mods.Count() > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLevelWasInitialized(level); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
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
                            if (Mods.Count() > 0)
                                for (int i = 0; i < Mods.Count; i++)
                                {
                                    MelonMod mod = Mods[i];
                                    if (mod != null)
                                        try { mod.VRChat_OnUiManagerInit(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
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
                    for (int i = 0; i < files.Count(); i++)
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
                    for (int i = 0; i < zippedFiles.Count(); i++)
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
                bool should_continue = false;
                bool isUniversal = false;
                MelonPluginGameAttribute[] pluginGameAttributes = assembly.GetCustomAttributes(typeof(MelonPluginGameAttribute), true) as MelonPluginGameAttribute[];
                int pluginGameAttributes_Count = pluginGameAttributes.Length;
                if (pluginGameAttributes_Count > 0)
                {
                    for (int i = 0; i < pluginGameAttributes_Count; i++)
                    {
                        MelonPluginGameAttribute pluginGameAttribute = pluginGameAttributes[i];
                        if (CurrentGameAttribute.IsCompatible(pluginGameAttribute))
                        {
                            isUniversal = CurrentGameAttribute.IsCompatibleBecauseUniversal(pluginGameAttribute);
                            should_continue = true;
                            break;
                        }
                    }
                }
                else
                {
                    isUniversal = true;
                    should_continue = true;
                }
                if (should_continue)
                {
                    try
                    {
                        MelonPlugin pluginInstance = Activator.CreateInstance(pluginInfoAttribute.SystemType) as MelonPlugin;
                        if (pluginInstance != null)
                        {
                            pluginInstance.IsUniversal = isUniversal;
                            pluginInstance.InfoAttribute = pluginInfoAttribute;
                            if (pluginGameAttributes_Count > 0)
                                pluginInstance.GameAttributes = pluginGameAttributes;
                            else
                                pluginInstance.GameAttributes = null;
                            pluginInstance.ModAssembly = assembly;
                            pluginInstance.Location = filelocation;
                            Harmony.HarmonyInstance.Create(assembly.FullName).PatchAll(assembly);
                            Plugins.Add(pluginInstance);
                        }
                        else
                            MelonModLogger.LogError("Unable to load Plugin in " + assembly.GetName() + "! Failed to Create Instance!");
                    }
                    catch (Exception e) { MelonModLogger.LogError("Unable to load Plugin in " + assembly.GetName() + "! " + e.ToString()); }
                }
                else
                    MelonModLogger.LogModStatus(3);
            }
        }

        private static void LoadModFromAssembly(Assembly assembly, string filelocation = null)
        {
            MelonModInfoAttribute modInfoAttribute = assembly.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonModInfoAttribute))) as MelonModInfoAttribute;
            if ((modInfoAttribute != null) && (modInfoAttribute.SystemType != null) && modInfoAttribute.SystemType.IsSubclassOf(typeof(MelonMod)))
            {
                bool should_continue = false;
                bool isUniversal = false;
                MelonModGameAttribute[] modGameAttributes = assembly.GetCustomAttributes(typeof(MelonModGameAttribute), true) as MelonModGameAttribute[];
                int modGameAttributes_Count = modGameAttributes.Length;
                if (modGameAttributes_Count > 0)
                {
                    for (int i = 0; i < modGameAttributes_Count; i++)
                    {
                        MelonModGameAttribute modGameAttribute = modGameAttributes[i];
                        if (CurrentGameAttribute.IsCompatible(modGameAttribute))
                        {
                            isUniversal = CurrentGameAttribute.IsCompatibleBecauseUniversal(modGameAttribute);
                            should_continue = true;
                            break;
                        }
                    }
                }
                else
                {
                    isUniversal = true;
                    should_continue = true;
                }
                if (should_continue)
                {
                    try
                    {
                        MelonMod modInstance = Activator.CreateInstance(modInfoAttribute.SystemType) as MelonMod;
                        if (modInstance != null)
                        {
                            modInstance.IsUniversal = isUniversal;
                            modInstance.InfoAttribute = modInfoAttribute;
                            if (modGameAttributes_Count > 0)
                                modInstance.GameAttributes = modGameAttributes;
                            else
                                modInstance.GameAttributes = null;
                            modInstance.ModAssembly = assembly;
                            modInstance.Location = filelocation;
                            Harmony.HarmonyInstance.Create(assembly.FullName).PatchAll(assembly);
                            Mods.Add(modInstance);
                        }
                        else
                            MelonModLogger.LogError("Unable to load Mod in " + assembly.GetName() + "! Failed to Create Instance!");
                    }
                    catch (Exception e) { MelonModLogger.LogError("Unable to load Mod in " + assembly.GetName() + "! " + e.ToString()); }
                }
                else
                    MelonModLogger.LogModStatus(3);
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
        private static string GetUnityFileVersion()
        {
            string file_version = FileVersionInfo.GetVersionInfo(Imports.GetExePath()).FileVersion;
            return file_version.Substring(0, file_version.LastIndexOf('.'));
        }

        private static void AddUnityDebugLog()
        {
            SupportModule.UnityDebugLog("--------------------------------------------------------------------------------------------------");
            SupportModule.UnityDebugLog("~   This Game has been MODIFIED using MelonLoader. DO NOT report any issues to the Developers!   ~");
            SupportModule.UnityDebugLog("--------------------------------------------------------------------------------------------------");
        }

        public static string GetUserDataPath()
        {
            string userDataDir = Path.Combine(Environment.CurrentDirectory, "UserData");
            if (!Directory.Exists(userDataDir))
                Directory.CreateDirectory(userDataDir);
            return userDataDir;
        }
    }
}