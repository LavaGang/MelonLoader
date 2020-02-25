using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    internal static class BuildInfo
    {
        internal const string Name = "MelonLoader";
        internal const string Author = "Herp Derpinstine";
        internal const string Company = "NanoNuke @ nanonuke.net";
        internal const string Version = "0.0.3";
    }

    public static class Main
    {
        internal static List<MelonModController> ModControllers = new List<MelonModController>();

        private static void Initialize()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

#if DEBUG
            if (Imports.melonloader_is_debug_mode())
                MelonModLogger.consoleEnabled = true;
            else
            {
                MelonModLogger.consoleEnabled = true;
                DebugConsole.Create();
            }
#else
            if (Imports.melonloader_is_debug_mode())
                MelonModLogger.consoleEnabled = true;
            else if (Environment.CommandLine.Contains("--melonloader.console"))
            {
                MelonModLogger.consoleEnabled = true;
                DebugConsole.Create();
            }
#endif

            MelonModLogger.Initialize();
            MelonModLogger.Log("Unity " + UnityEngine.Application.unityVersion);
            MelonModLogger.Log("-----------------------------");
            MelonModLogger.Log("Using v" + BuildInfo.Version + " Closed-Beta");
            MelonModLogger.Log("-----------------------------");

            if (Imports.melonloader_is_il2cpp_game())
                NET_SDK.SDK.Initialize();

            string modDirectory = Path.Combine(Environment.CurrentDirectory, "Mods");
            if (!Directory.Exists(modDirectory))
                Directory.CreateDirectory(modDirectory);
            else
            {
                ModLoaderBackwardsCompatibility.AddAssemblyResolveHandler();
                string[] files = Directory.GetFiles(modDirectory, "*.dll");
                foreach (string s in files)
                {
                    if (!File.Exists(s) || !s.EndsWith(".dll", true, null))
                        return;
                    MelonModLogger.Log("Loading " + Path.GetFileName(s));
                    try
                    {
                        byte[] data = File.ReadAllBytes(s);
                        Assembly a = Assembly.Load(data);
                        LoadModsFromAssembly(a);
                    }
                    catch (Exception e)
                    {
                        MelonModLogger.LogError("Unable to load Assembly " + s + ":\n" + e);
                    }
                }
                if (ModControllers.Count() > 0)
                {
                    MelonModLogger.Log("-----------------------------");
                    foreach (MelonModController modController in ModControllers)
                    {
                        if (modController.modInstance != null)
                        {
                            MelonMod mod = modController.modInstance;
                            if ((mod != null) && !string.IsNullOrEmpty(mod.Name))
                                MelonModLogger.Log(mod.Name + (!string.IsNullOrEmpty(mod.Version) ? (" v" + mod.Version) : "") + (!string.IsNullOrEmpty(mod.Author) ? (" by " + mod.Author) : "") + (!string.IsNullOrEmpty(mod.DownloadLink) ? (" (" + mod.DownloadLink + ")") : ""));
                        }
                    }
                    MelonModLogger.Log("-----------------------------");
                    MelonModComponent.Create();
                }
            }
        }

        private static void LoadModsFromAssembly(Assembly assembly)
        {
            try
            {
                foreach (Type t in GetLoadableTypes(assembly))
                {
                    if (t.IsSubclassOf(typeof(MelonMod))) // MelonLoader
                    {
                        try
                        {
                            MelonMod modInstance = Activator.CreateInstance(t) as MelonMod;
                            MelonModInfoAttribute modInfoAttribute = modInstance.GetType().Assembly.GetCustomAttributes(typeof(MelonModInfoAttribute), true).FirstOrDefault() as MelonModInfoAttribute;
                            if (modInfoAttribute != null)
                            {
                                modInstance.Name = modInfoAttribute.Name;
                                modInstance.Version = modInfoAttribute.Version;
                                modInstance.Author = modInfoAttribute.Author;
                                modInstance.DownloadLink = modInfoAttribute.DownloadLink;
                            }
                            ModControllers.Add(new MelonModController(modInstance, t, assembly));
                        }
                        catch (Exception e)
                        {
                            MelonModLogger.LogError("Could not load mod " + t.FullName + " in " + assembly.GetName() + "! " + e);
                        }
                    }
                    else if (t.IsSubclassOf(typeof(VRCModLoader.VRCMod))) // VRCModLoader
                        ModLoaderBackwardsCompatibility.VRCModLoader(t, assembly);
                }
            }
            catch (Exception e)
            {
                MelonModLogger.LogError("Could not load " + assembly.GetName() + "! " + e);
            }
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                MelonModLogger.LogError("An error occured while getting types from assembly " + assembly.GetName().Name + ". Returning types from error.\n" + e);
                return e.Types.Where(t => t != null);
            }
        }

        internal static void OnApplicationStart()
        {
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnApplicationStart(); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
        }

        static int level_loaded_index = -1;
        static bool was_level_loaded = false;
        internal static void OnLevelWasLoaded(int level)
        {
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnLevelWasLoaded(level); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
            was_level_loaded = true;
            level_loaded_index = level;
        }

        internal static void OnLevelWasInitialized(int level)
        {
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnLevelWasInitialized(level); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
        }

        internal static void OnUpdate()
        {
            if (was_level_loaded)
            {
                OnLevelWasInitialized(level_loaded_index);
                was_level_loaded = false;
                level_loaded_index = -1;
            }
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnUpdate(); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
        }

        internal static void OnFixedUpdate()
        {
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnFixedUpdate(); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
        }

        internal static void OnLateUpdate()
        {
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnLateUpdate(); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
        }

        internal static void OnGUI()
        {
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnGUI(); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
        }

        internal static void OnApplicationQuit()
        {
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnApplicationQuit(); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
            ModPrefs.SaveConfig();
            NET_SDK.Harmony.Manager.UnpatchAll();
            NET_SDK.Harmony.Manager.UnpatchMain();
            MelonModLogger.Stop();
        }

        internal static void OnModSettingsApplied()
        {
            if (ModControllers.Count() > 0)
            {
                foreach (MelonModController mod in ModControllers)
                {
                    try { mod.OnModSettingsApplied(); }
                    catch (Exception ex) { MelonModLogger.LogModError(ex.ToString(), mod.modInstance.Name); }
                }
            }
        }
    }
}