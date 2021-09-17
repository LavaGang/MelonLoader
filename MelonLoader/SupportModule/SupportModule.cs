using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MelonLoader
{
    internal static class SupportModule
    {
        internal static ISupportModule_To Interface = null;

        private static string UnityEngine_CoreModule_Path = null;
        private static string BaseDirectory = null;
        private static List<ModuleListing> Modules = new List<ModuleListing>()
        {
            new ModuleListing("Il2Cpp.dll", MelonUtils.IsGameIl2Cpp),

            new ModuleListing("Mono.dll", () => !MelonUtils.IsGameIl2Cpp()
                && File.Exists(UnityEngine_CoreModule_Path)),

            new ModuleListing("Mono.Pre-5.dll", () => !MelonUtils.IsGameIl2Cpp()
                && !File.Exists(UnityEngine_CoreModule_Path)
                && IsOldUnity()),

            new ModuleListing("Mono.Pre-2017.dll", () => !MelonUtils.IsGameIl2Cpp()
                && !File.Exists(UnityEngine_CoreModule_Path)
                && !IsOldUnity()),
        };

        internal static bool Setup()
        {
            UnityEngine_CoreModule_Path = Path.Combine(MelonUtils.GetManagedDirectory(), "UnityEngine.CoreModule.dll");

            BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MelonUtils.BaseDirectory, "MelonLoader"), "Dependencies"), "SupportModules");
            if (!Directory.Exists(BaseDirectory))
            {
                MelonLogger.Error("Failed to Find SupportModules Directory!");
                return false;
            }

            LemonEnumerator<ModuleListing> enumerator = new LemonEnumerator<ModuleListing>(Modules);
            while (enumerator.MoveNext())
            {
                string ModulePath = Path.Combine(BaseDirectory, enumerator.Current.FileName);
                if (!File.Exists(ModulePath))
                    continue;

                try
                {
                    if (enumerator.Current.LoadSpecifier != null)
                    {
                        if (!enumerator.Current.LoadSpecifier())
                        {
                            File.Delete(ModulePath);
                            continue;
                        }
                    }

                    if (Interface != null)
                        continue;

                    if (!LoadInterface(ModulePath))
                        continue;

                    MelonDebug.Msg($"Support Module Loaded: {enumerator.Current.FileName}");
                }
                catch (Exception ex)
                {
                    MelonDebug.Error($"Support Module [{enumerator.Current.FileName}] threw an Exception: {ex}");
                    continue;
                }
            }

            if (Interface == null)
            {
                MelonLogger.Error("No Support Module Loaded!");
                return false;
            }
            return true;
        }

        private static bool LoadInterface(string ModulePath)
        {
            Assembly assembly = Assembly.LoadFrom(ModulePath);
            if (assembly == null)
                return false;

            Type type = assembly.GetType("MelonLoader.Support.Main");
            if (type == null)
            {
                MelonLogger.Error("Failed to Get Type MelonLoader.Support.Main!");
                return false;
            }

            MethodInfo method = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                MelonLogger.Error("Failed to Get Method Initialize!");
                return false;
            }

            Interface = (ISupportModule_To)method.Invoke(null, new object[] { new SupportModule_From() });
            if (Interface == null)
            {
                MelonLogger.Error("Failed to Initialize Interface!");
                return false;
            }

            MelonDebug.Msg($"Support Module Loaded: {ModulePath}");

            return true;
        }

        private static bool IsOldUnity()
        {
            try
            {
                Assembly unityengine = Assembly.Load("UnityEngine");
                if (unityengine == null)
                    return true;
                Type scenemanager = unityengine.GetType("UnityEngine.SceneManagement.SceneManager");
                if (scenemanager == null)
                    return true;
                EventInfo sceneLoaded = scenemanager.GetEvent("sceneLoaded");
                if (sceneLoaded == null)
                    return true;
                return false;
            }
            catch { return true; }
        }

        // Module Listing
        internal class ModuleListing
        {
            internal string FileName = null;
            internal delegate bool dLoadSpecifier();
            internal dLoadSpecifier LoadSpecifier = null;
            internal ModuleListing(string filename)
                => FileName = filename;
            internal ModuleListing(string filename, dLoadSpecifier loadSpecifier)
            {
                FileName = filename;
                LoadSpecifier = loadSpecifier;
            }
        }
    }
}