using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
#pragma warning disable 0612

namespace MelonLoader
{
    public static class Main
    {
        public static List<MelonMod> Mods = new List<MelonMod>();
        public static List<MelonPlugin> Plugins = new List<MelonPlugin>();
        private static List<MelonPlugin> TempPlugins = null;
        internal static MelonGameAttribute CurrentGameAttribute = null;
        public static bool IsVRChat = false;
        public static bool IsBoneworks = false;
        private static Assembly Assembly_CSharp = null;
        private static bool HasGeneratedAssembly = false;

        private static void Initialize()
        {
            if (string.IsNullOrEmpty(AppDomain.CurrentDomain.BaseDirectory))
                ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AppDomain.CurrentDomain, new object[0])).ApplicationBase = Imports.GetGameDirectory();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;

            CurrentGameAttribute = new MelonGameAttribute(Imports.GetCompanyName(), Imports.GetProductName());

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
                LoadMelons(true);
                if (Plugins.Count > 0)
                {
                    HashSet<MelonPlugin> failedPlugins = new HashSet<MelonPlugin>();
                    TempPlugins = Plugins.Where(plugin => (plugin.Compatibility < MelonBase.MelonCompatibility.INCOMPATIBLE)).ToList();
                    DependencyGraph<MelonPlugin>.TopologicalSort(TempPlugins, plugin => plugin.Info.Name);
                    for (int i = 0; i < TempPlugins.Count; i++)
                    {
                        MelonPlugin plugin = TempPlugins[i];
                        if (plugin != null)
                            try { plugin.OnPreInitialization(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), plugin.Info.Name); failedPlugins.Add(plugin); }
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

            MelonLogger.Log("------------------------------");
            MelonLogger.Log("Unity " + UnityVersion);
            MelonLogger.Log("OS: " + Environment.OSVersion.ToString());
            MelonLogger.Log("------------------------------");
            MelonLogger.Log("Name: " + CurrentGameAttribute.GameName);
            MelonLogger.Log("Developer: " + CurrentGameAttribute.Developer);
            MelonLogger.Log("Type: " + (Imports.IsIl2CppGame() ? "Il2Cpp" : (Imports.IsOldMono() ? "Mono" : "MonoBleedingEdge")));
            MelonLogger.Log("------------------------------");
            MelonLogger.Log("Using v" + BuildInfo.Version + " Open-Beta");
            MelonLogger.Log("------------------------------");

            LoadMelons();
            if (Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MelonPlugin plugin = Plugins[i];
                    if (plugin != null)
                    {
                        MelonLogger.Log(plugin.Info.Name
                            + (!string.IsNullOrEmpty(plugin.Info.Version)
                            ? (" v" + plugin.Info.Version) : "")
                            + (!string.IsNullOrEmpty(plugin.Info.Author)
                            ? (" by " + plugin.Info.Author) : "")
                            + (!string.IsNullOrEmpty(plugin.Info.DownloadLink)
                            ? (" (" + plugin.Info.DownloadLink + ")")
                            : "")
                            );
                        MelonLogger.LogMelonCompatibility(plugin.Compatibility);
                        MelonLogger.Log("------------------------------");
                    }
                }
                Plugins = TempPlugins;
            }
            if (Plugins.Count <= 0)
            {
                MelonLogger.Log("No Plugins Loaded!");
                MelonLogger.Log("------------------------------");
            }

            if (Mods.Count > 0)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                    {
                        MelonLogger.Log(mod.Info.Name
                            + (!string.IsNullOrEmpty(mod.Info.Version)
                            ? (" v" + mod.Info.Version) : "")
                            + (!string.IsNullOrEmpty(mod.Info.Author)
                            ? (" by " + mod.Info.Author) : "")
                            + (!string.IsNullOrEmpty(mod.Info.DownloadLink)
                            ? (" (" + mod.Info.DownloadLink + ")")
                            : "")
                            );
                        MelonLogger.LogMelonCompatibility(mod.Compatibility);
                        MelonLogger.Log("------------------------------");
                    }
                }
                Mods.RemoveAll((MelonMod mod) => (mod.Compatibility >= MelonBase.MelonCompatibility.INCOMPATIBLE));
                DependencyGraph<MelonMod>.TopologicalSort(Mods, mod => mod.Info.Name);
            }
            if (Mods.Count <= 0)
            {
                MelonLogger.Log("No Mods Loaded!");
                MelonLogger.Log("------------------------------");
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
                        try { InitializeMelon(plugin); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), plugin.Info.Name); failedPlugins.Add(plugin); }
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
                        try { InitializeMelon(mod); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); failedMods.Add(mod); }
                }
                Mods.RemoveAll(mod => failedMods.Contains(mod));
            }

            if ((Plugins.Count <= 0) && (Mods.Count <= 0))
                SupportModule.Destroy();
        }

        private static void InitializeMelon(MelonBase modOrPlugin)
        {
            string harmonyId = modOrPlugin.Assembly.FullName;
            HarmonyInstance harmony = HarmonyInstance.Create(harmonyId);
            try
            {
                harmony.PatchAll(modOrPlugin.Assembly);
                modOrPlugin.OnApplicationStart();
            }
            catch (Exception)
            {
                harmony.UnpatchAll(modOrPlugin);
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
                        try { plugin.OnApplicationQuit(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), plugin.Info.Name); }
                }
            }
            if (Mods.Count > 0)
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnApplicationQuit(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
                }
            }
            if ((Plugins.Count > 0) || (Mods.Count > 0))
                MelonPrefs.SaveConfig();
            HarmonyInstance.UnpatchAllInstances();
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
                        try { plugin.OnModSettingsApplied(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), plugin.Info.Name); }
                }
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnModSettingsApplied(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
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
                        try { mod.OnUpdate(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
                }
        }

        public static void OnFixedUpdate()
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnFixedUpdate(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
                }
        }

        public static void OnLateUpdate()
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLateUpdate(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
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
                        try { mod.OnGUI(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
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
                        try { mod.OnLevelIsLoading(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
                }
        }

        internal static void OnLevelWasLoaded(int level)
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLevelWasLoaded(level); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
                }
        }

        internal static void OnLevelWasInitialized(int level)
        {
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.OnLevelWasInitialized(level); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
                }
        }

        private static bool ShouldCheckForUiManager = true;
        private static Type VRCUiManager = null;
        private static PropertyInfo VRCUiManager_Instance = null;
        private static void VRChat_CheckUiManager()
        {
            if (!ShouldCheckForUiManager)
                return;
            if (VRCUiManager == null)
                VRCUiManager = Assembly_CSharp.GetType("VRCUiManager");
            if (VRCUiManager == null)
                return;
            if (VRCUiManager_Instance == null)
                VRCUiManager_Instance = VRCUiManager.GetProperty("field_Protected_Static_VRCUiManager_0");
            if (VRCUiManager_Instance == null)
                return;
            object returnval = VRCUiManager_Instance.GetValue(null, new object[0]);
            if (returnval == null)
                return;
            ShouldCheckForUiManager = false;
            if (Mods.Count > 0)
                for (int i = 0; i < Mods.Count; i++)
                {
                    MelonMod mod = Mods[i];
                    if (mod != null)
                        try { mod.VRChat_OnUiManagerInit(); } catch (Exception ex) { MelonLogger.LogMelonError(ex.ToString(), mod.Info.Name); }
                }
        }

        private static void LoadMelons(bool plugins = false)
        {
            string searchdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (plugins ? "Plugins" : "Mods"));
            if (!Directory.Exists(searchdir))
            {
                Directory.CreateDirectory(searchdir);
                return;
            }
            Imports.LoadMode loadmode = (plugins ? Imports.GetLoadMode_Plugins() : Imports.GetLoadMode_Mods());

            // DLL
            string[] files = Directory.GetFiles(searchdir, "*.dll");
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    if (string.IsNullOrEmpty(file))
                        continue;

                    bool file_extension_check = Path.GetFileNameWithoutExtension(file).EndsWith("-dev");
                    if ((loadmode != Imports.LoadMode.BOTH) && ((loadmode == Imports.LoadMode.DEV) ? !file_extension_check : file_extension_check))
                        continue;

                    try
                    {
                        LoadMelonFromFile(file, plugins);
                    }
                    catch (Exception e)
                    {
                        MelonLogger.LogError("Unable to load " + file + ":\n" + e.ToString());
                        MelonLogger.Log("------------------------------");
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
                    if (string.IsNullOrEmpty(file))
                        continue;
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

                                    bool file_extension_check = Path.GetFileNameWithoutExtension(file).EndsWith("-dev");
                                    if ((loadmode != Imports.LoadMode.BOTH) && ((loadmode == Imports.LoadMode.DEV) ? !file_extension_check : file_extension_check))
                                        continue;

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
                                        LoadMelonFromAssembly(Assembly.Load(unzippedFileStream.ToArray()), plugins, (file + "/" + filename));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MelonLogger.LogError("Unable to load " + file + ":\n" + e.ToString());
                        MelonLogger.Log("------------------------------");
                    }
                }
            }
        }

        private static void LoadMelonFromFile(string filelocation, bool isPlugin = false) => LoadMelonFromAssembly((Imports.IsDebugMode() ? Assembly.LoadFrom(filelocation) : Assembly.Load(File.ReadAllBytes(filelocation))), isPlugin, filelocation);
        private static void LoadMelonFromAssembly(Assembly asm, bool isPlugin = false, string filelocation = null)
        {
            if (!asm.Equals(null))
            {
                MelonLegacyAttributeSupport.Response_Info response_Info = MelonLegacyAttributeSupport.GetMelonInfoAttribute(asm, isPlugin);
                MelonInfoAttribute InfoAttribute = response_Info.Default;
                if ((InfoAttribute != null) && (InfoAttribute.SystemType != null) && InfoAttribute.SystemType.IsSubclassOf((isPlugin ? typeof(MelonPlugin) : typeof(MelonMod))))
                {
                    bool isCompatible = false;
                    bool isUniversal = false;
                    bool hasAttribute = true;
                    MelonLegacyAttributeSupport.Response_Game response_Game = MelonLegacyAttributeSupport.GetMelonGameAttributes(asm, isPlugin);
                    MelonGameAttribute[] GameAttributes = response_Game.Default;
                    int GameAttributes_Count = GameAttributes.Length;
                    if (GameAttributes_Count > 0)
                    {
                        for (int i = 0; i < GameAttributes_Count; i++)
                        {
                            MelonGameAttribute GameAttribute = GameAttributes[i];
                            if (CurrentGameAttribute.IsCompatible(GameAttribute))
                            {
                                isCompatible = true;
                                isUniversal = CurrentGameAttribute.IsCompatibleBecauseUniversal(GameAttribute);
                                break;
                            }
                        }
                    }
                    else
                        hasAttribute = false;
                    MelonBase baseInstance = Activator.CreateInstance(InfoAttribute.SystemType) as MelonBase;
                    if (baseInstance != null)
                    {
                        response_Info.SetupMelon(baseInstance);
                        response_Game.SetupMelon(baseInstance);
                        baseInstance.OptionalDependenciesAttribute = asm.GetCustomAttributes(false).FirstOrDefault(x => (x.GetType() == typeof(MelonOptionalDependenciesAttribute))) as MelonOptionalDependenciesAttribute;
                        baseInstance.Location = filelocation;
                        baseInstance.Compatibility = (isUniversal ? MelonBase.MelonCompatibility.UNIVERSAL : 
                            (isCompatible ? MelonBase.MelonCompatibility.COMPATIBLE : 
                                (!hasAttribute ? MelonBase.MelonCompatibility.NOATTRIBUTE : MelonBase.MelonCompatibility.INCOMPATIBLE)
                            )
                        );
                        if (baseInstance.Compatibility < MelonBase.MelonCompatibility.INCOMPATIBLE)
                            baseInstance.Assembly = asm;
                        if (isPlugin)
                            Plugins.Add((MelonPlugin)baseInstance);
                        else
                            Mods.Add((MelonMod)baseInstance);
                    }
                    else
                        MelonLogger.LogError("Unable to load Plugin in " + asm.GetName() + "! Failed to Create Instance!");
                }
            }
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs e) => MelonLogger.LogError((e.ExceptionObject as Exception).ToString());

        private static string _unityVer = null;
        public static string UnityVersion
        {
            get
            {
                if (_unityVer != null)
                    return _unityVer;
                string exepath = Imports.GetExePath();
                string ggm_path = Path.Combine(Imports.GetGameDataDirectory(), "globalgamemanagers");
                if (!File.Exists(ggm_path))
                {
                    FileVersionInfo versioninfo = FileVersionInfo.GetVersionInfo(exepath);
                    if ((versioninfo == null) || string.IsNullOrEmpty(versioninfo.FileVersion))
                        return "UNKNOWN";
                    return versioninfo.FileVersion.Substring(0, versioninfo.FileVersion.LastIndexOf('.'));
                }
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
                return _unityVer = Encoding.UTF8.GetString(verstr_byte, 0, verstr_byte.Length);
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