using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    public static class Main
    {
        internal static List<MelonMod> Mods = new List<MelonMod>();
        internal static MelonModGameAttribute CurrentGameAttribute = null;
        internal static bool IsVRChat = false;
        internal static bool IsBoneworks = false;
        private static int LastSceneIndex = -9;
        private static NET_SDK.Reflection.IL2CPP_Field BoneworksSceneManager_currentSceneIndex = null;
        private static NET_SDK.Reflection.IL2CPP_Method SceneManager_GetActiveScene = null;
        private static NET_SDK.Reflection.IL2CPP_Method Scene_buildIndex_get = null;
        private static NET_SDK.Reflection.IL2CPP_Class VRCUiManager = null;
        private static NET_SDK.Reflection.IL2CPP_Method VRCUiManager_GetInstance = null;
        private static bool ShouldCheckForUiManager = true;
        internal static bool IsInitialized = false;

        private static void Initialize()
        {
            CurrentGameAttribute = new MelonModGameAttribute(Imports.GetCompanyName(), Imports.GetProductName());

            if (Imports.IsIl2CppGame())
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                IsVRChat = CurrentGameAttribute.IsGame("VRChat", "VRChat");
                IsBoneworks = CurrentGameAttribute.IsGame("Stress Level Zero", "BONEWORKS");
            }

            if (!Imports.IsDebugMode()
#if !DEBUG
            && Environment.CommandLine.Contains("--melonloader.console")
#endif
            )
            {
                MelonModLogger.consoleEnabled = true;
                Console.Create();
            }


            MelonModLogger.Log("------------------------------");
            MelonModLogger.Log("Unity " + Imports.GetUnityVersion());
            MelonModLogger.Log("Developer: " + CurrentGameAttribute.Developer);
            MelonModLogger.Log("GameName: " + CurrentGameAttribute.GameName);
            MelonModLogger.Log("Version: " + Imports.GetGameVersion());
            MelonModLogger.Log("------------------------------");
            MelonModLogger.Log("Using v" + BuildInfo.Version + " Open-Beta");
            MelonModLogger.Log("------------------------------");

            if (Imports.IsIl2CppGame())
            {
                MelonModLogger.Log("Initializing NET_SDK...");
                NET_SDK.SDK.Initialize();
                MelonModLogger.Log("------------------------------");
            }

            bool no_mods = false;
            string modDirectory = Path.Combine(Environment.CurrentDirectory, "Mods");
            if (!Directory.Exists(modDirectory))
            {
                Directory.CreateDirectory(modDirectory);
                no_mods = true;
            }
            else
            {
                string[] files = Directory.GetFiles(modDirectory, "*.dll");
                if (files.Length > 0)
                {
                    foreach (string s in files)
                    {
                        if (!File.Exists(s) || !s.EndsWith(".dll", true, null))
                            return;
                        try
                        {
                            byte[] data = File.ReadAllBytes(s);
                            if (data.Length > 0)
                            {
                                Assembly a = Assembly.Load(data);
                                if (a != null)
                                    LoadModsFromAssembly(a);
                                else
                                    MelonModLogger.LogError("Unable to load " + s);
                            }
                            else
                                MelonModLogger.LogError("Unable to load " + s);
                        }
                        catch (Exception e) { MelonModLogger.LogError("Unable to load " + s + ":\n" + e.ToString()); }
                        MelonModLogger.Log("------------------------------");
                    }
                    if (Mods.Count() <= 0)
                        no_mods = true;
                }
                else
                    no_mods = true;
            }
            if (no_mods)
            {
                MelonModLogger.Log("No Mods Loaded!");
                MelonModLogger.Log("------------------------------");
            }
            else
                MelonModComponent.Create();
        }

        private static void LoadModsFromAssembly(Assembly assembly)
        {
            MelonModInfoAttribute modInfoAttribute = assembly.GetCustomAttribute(typeof(MelonModInfoAttribute)) as MelonModInfoAttribute;
            if ((modInfoAttribute != null) && (modInfoAttribute.ModType != null) && modInfoAttribute.ModType.IsSubclassOf(typeof(MelonMod)))
            {
                MelonModLogger.Log(modInfoAttribute.Name + (!string.IsNullOrEmpty(modInfoAttribute.Version) ? (" v" + modInfoAttribute.Version) : "") + (!string.IsNullOrEmpty(modInfoAttribute.Author) ? (" by " + modInfoAttribute.Author) : "") + (!string.IsNullOrEmpty(modInfoAttribute.DownloadLink) ? (" (" + modInfoAttribute.DownloadLink + ")") : ""));

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
                        MelonMod modInstance = Activator.CreateInstance(modInfoAttribute.ModType) as MelonMod;
                        if (modInstance != null)
                        {
                            modInstance.IsUniversal = isUniversal;
                            modInstance.InfoAttribute = modInfoAttribute;
                            if (modGameAttributes_Count > 0)
                                modInstance.GameAttributes = modGameAttributes;
                            else
                                modInstance.GameAttributes = null;
                            Mods.Add(modInstance);
                            MelonModLogger.LogModStatus((modGameAttributes_Count > 0) ? (isUniversal ? 0 : 1) : 2);
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

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try { return assembly.GetTypes(); } catch (ReflectionTypeLoadException e) { MelonModLogger.LogError("An Error occured while getting Types from Assembly " + assembly.GetName().Name + ". Returning Types from Error.\n" + e); return e.Types.Where(t => t != null); }
        }

        internal static void OnApplicationStart()
        {
            if (Mods.Count() > 0)
                foreach (MelonMod mod in Mods)
                    try { mod.OnApplicationStart(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
            IsInitialized = true;
        }

        static int level_loaded_index = -1;
        static bool was_level_loaded = false;
        internal static void OnLevelWasLoaded(int level)
        {
            if (IsInitialized)
            {
                if (Mods.Count() > 0)
                    foreach (MelonMod mod in Mods)
                        try { mod.OnLevelWasLoaded(level); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                was_level_loaded = true;
                level_loaded_index = level;
            }
        }

        internal static void OnLevelWasInitialized(int level)
        {
            if (IsInitialized && (Mods.Count() > 0))
                foreach (MelonMod mod in Mods)
                        try { mod.OnLevelWasInitialized(level); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
        }

        internal static void OnUpdate()
        {
            if (IsInitialized)
            {
                if (was_level_loaded)
                {
                    OnLevelWasInitialized(level_loaded_index);
                    was_level_loaded = false;
                    level_loaded_index = -1;
                }
                if (Imports.IsIl2CppGame())
                {
                    if (IsVRChat)
                        VRChat_CheckUiManager();
                    if (!Imports.IsMUPOTMode())
                        CheckForSceneChange();
                }
                if (Mods.Count() > 0)
                    foreach (MelonMod mod in Mods)
                        try { mod.OnUpdate(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                if (Imports.IsIl2CppGame() && !Imports.IsMUPOTMode())
                    MelonCoroutines.Process();
            }
        }

        internal static void OnFixedUpdate()
        {
            if (IsInitialized)
            {
                if (Mods.Count() > 0)
                    foreach (MelonMod mod in Mods)
                        try { mod.OnFixedUpdate(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                if (Imports.IsIl2CppGame() && !Imports.IsMUPOTMode())
                    MelonCoroutines.ProcessWaitForFixedUpdate();
            }
        }

        internal static void OnLateUpdate()
        {
            if (IsInitialized && (Mods.Count() > 0))
                foreach (MelonMod mod in Mods)
                    try { mod.OnLateUpdate(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
        }

        internal static void OnGUI()
        {
            if (IsInitialized)
            {
                if (Mods.Count() > 0)
                    foreach (MelonMod mod in Mods)
                        try { mod.OnGUI(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
            }
        }

        internal static void OnApplicationQuit()
        {
            if (IsInitialized && (Mods.Count() > 0))
                foreach (MelonMod mod in Mods)
                    try { mod.OnApplicationQuit(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
            ModPrefs.SaveConfig();
            if (Imports.IsIl2CppGame() && !Imports.IsMUPOTMode())
                NET_SDK.Harmony.Manager.UnpatchAll();
            Harmony.HarmonyInstance.UnpatchAllInstances();
        }

        internal static void OnModSettingsApplied()
        {
            if (IsInitialized && (Mods.Count() > 0))
                foreach (MelonMod mod in Mods)
                    try { mod.OnModSettingsApplied(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
        }

        public static int GetActiveSceneIndex()
        {
            if (IsBoneworks)
            {
                if (BoneworksSceneManager_currentSceneIndex == null)
                    BoneworksSceneManager_currentSceneIndex = NET_SDK.SDK.GetClass("StressLevelZero.Utilities.BoneworksSceneManager")?.GetField("currentSceneIndex");
                if (BoneworksSceneManager_currentSceneIndex == null)
                    return -9;
                return BoneworksSceneManager_currentSceneIndex.GetValue().UnboxValue<int>();
            }
            else
            {
                if (SceneManager_GetActiveScene == null)
                    SceneManager_GetActiveScene = NET_SDK.SDK.GetClass("UnityEngine.SceneManagement.SceneManager")?.GetMethod("GetActiveScene");
                if (SceneManager_GetActiveScene == null)
                    return -9;
                if (Scene_buildIndex_get == null)
                    Scene_buildIndex_get = NET_SDK.SDK.GetClass("UnityEngine.SceneManagement.Scene")?.GetProperty("buildIndex")?.GetGetMethod();
                if (Scene_buildIndex_get == null)
                    return -9;
                NET_SDK.Reflection.IL2CPP_Object scene = SceneManager_GetActiveScene.Invoke();
                if (scene == null)
                    return -9;
                NET_SDK.Reflection.IL2CPP_Object buildIndex = Scene_buildIndex_get.Invoke(scene);
                if (buildIndex == null)
                    return -9;
                return buildIndex.UnboxValue<int>();
            }
        }

        private static void CheckForSceneChange()
        {
            int currentscene = GetActiveSceneIndex();
            if (LastSceneIndex != currentscene)
            {
                LastSceneIndex = currentscene;
                OnLevelWasLoaded(currentscene);
            }
        }

        private static void VRChat_CheckUiManager()
        {
            if (IsInitialized && ShouldCheckForUiManager)
            {
                if (VRCUiManager == null)
                    VRCUiManager = NET_SDK.SDK.GetClass("VRCUiManager");
                if (VRCUiManager != null)
                {
                    if (VRCUiManager_GetInstance == null)
                    {
                        NET_SDK.Reflection.IL2CPP_Method[] methods = VRCUiManager.GetMethods();
                        foreach (NET_SDK.Reflection.IL2CPP_Method method in methods)
                        {
                            NET_SDK.Reflection.IL2CPP_Type returntype = method.GetReturnType();
                            if ((returntype != null) && !string.IsNullOrEmpty(returntype.Name) && returntype.Name.Equals("VRCUiManager"))
                            {
                                if (method.HasFlag(NET_SDK.Reflection.IL2CPP_BindingFlags.METHOD_STATIC))
                                {
                                    VRCUiManager_GetInstance = method;
                                    break;
                                }
                            }
                        }
                    }
                    if (VRCUiManager_GetInstance != null)
                    {
                        NET_SDK.Reflection.IL2CPP_Object returnval = VRCUiManager_GetInstance.Invoke();
                        if (returnval != null)
                        {
                            ShouldCheckForUiManager = false;
                            if (Mods.Count() > 0)
                                foreach (MelonMod mod in Mods)
                                    try { mod.VRChat_OnUiManagerInit(); } catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.InfoAttribute.Name); }
                        }
                    }
                }
            }
        }
    }
}