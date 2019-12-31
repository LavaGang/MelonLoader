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
        internal const string Version = "0.0.1";
    }

    public static class Main
    {
        public static string CommandLine = Imports.melonloader_getcommandline();
        public static List<MelonMod> Mods = new List<MelonMod>();
        private static List<MelonModController> ModControllers = new List<MelonModController>();

        internal static void Initialize()
        {
#if DEBUG
            Logger.consoleEnabled = true;
#else
            if (CommandLine.Contains("--melonloader.debug"))
                Logger.consoleEnabled = true;
            else if (CommandLine.Contains("--melonloader.console"))
            {
                Logger.consoleEnabled = true;
                DebugConsole.Create();
            }
#endif

            Logger.Initialize();

            Logger.Log("-----------------------------");
            //Logger.Log("Using v" + BuildInfo.Version);
            Logger.Log("Using v" + BuildInfo.Version + " Closed-Beta");
            Logger.Log("-----------------------------");

            string modDirectory = Path.Combine(Environment.CurrentDirectory, "Mods");
            if (!Directory.Exists(modDirectory))
                Directory.CreateDirectory(modDirectory);
            else
            {
                List<Assembly> loadedAssemblies = new List<Assembly>();
                string[] files = Directory.GetFiles(modDirectory, "*.dll");
                foreach (string s in files)
                {
                    if (!File.Exists(s) || !s.EndsWith(".dll", true, null))
                        return;
                    Logger.Log("Loading " + Path.GetFileName(s));
                    try
                    {
                        byte[] data = File.ReadAllBytes(s);
                        Assembly a = Assembly.Load(data);
                        loadedAssemblies.Add(a);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Unable to load Assembly " + s + ":\n" + e);
                    }
                }
                if (loadedAssemblies.Count() > 0)
                {
                    foreach (Assembly a in loadedAssemblies)
                        LoadModsFromAssembly(a);
                }
                if (Mods.Count() > 0)
                {
                    Logger.Log("-----------------------------");
                    foreach (var mod in Mods)
                        Logger.Log(mod.Name + " (" + mod.Version + ") by " + mod.Author + (mod.DownloadLink != null ? " (" + mod.DownloadLink + ")" : ""));
                    Logger.Log("-----------------------------");
                    OnApplicationStart();
                }
            }
        }

        private static void LoadModsFromAssembly(Assembly assembly)
        {
            try
            {
                foreach (Type t in GetLoadableTypes(assembly))
                {
                    if (t.IsSubclassOf(typeof(MelonMod)))
                    {
                        try
                        {
                            MelonMod modInstance = Activator.CreateInstance(t) as MelonMod;
                            Mods.Add(modInstance);
                            ModControllers.Add(new MelonModController(modInstance, t));
                            MelonModInfoAttribute modInfoAttribute = modInstance.GetType().GetCustomAttributes(typeof(MelonModInfoAttribute), true).FirstOrDefault() as MelonModInfoAttribute;
                            if (modInfoAttribute != null)
                            {
                                modInstance.Name = modInfoAttribute.Name;
                                modInstance.Version = modInfoAttribute.Version;
                                modInstance.Author = modInfoAttribute.Author;
                                modInstance.DownloadLink = modInfoAttribute.DownloadLink;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("Could not load mod " + t.FullName + " in " + assembly.GetName() + "! " + e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Could not load " + assembly.GetName() + "! " + e);
            }
        }

        public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                Logger.LogError("An error occured while getting types from assembly " + assembly.GetName().Name + ". Returning types from error.\n" + e);
                return e.Types.Where(t => t != null);
            }
        }

        internal static void OnApplicationStart()
        {
            if (ModControllers.Count() > 0)
                foreach (MelonModController mod in ModControllers)
                    mod.OnApplicationStart();
        }

        internal static void OnApplicationQuit()
        {
            Logger.Log("OnApplicationQuit");
            if (ModControllers.Count() > 0)
                foreach (MelonModController mod in ModControllers)
                    mod.OnApplicationQuit();
        }

        public static void OnModSettingsApplied()
        {
            if (ModControllers.Count() > 0)
                foreach (MelonModController mod in ModControllers)
                    mod.OnModSettingsApplied();
        }
    }
}