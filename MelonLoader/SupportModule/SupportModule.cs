using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MelonLoader.Utils;

namespace MelonLoader
{
    internal static class SupportModule
    {
        internal static ISupportModule_To Interface = null;

        private static string BaseDirectory = null;
        private static List<ModuleListing> Modules = new List<ModuleListing>()
        {
            new ModuleListing("Il2Cpp.dll", MelonUtils.IsGameIl2Cpp),
            new ModuleListing("Mono.dll", () => !MelonUtils.IsGameIl2Cpp())
        };

        internal static bool Setup()
        {
            BaseDirectory = MelonEnvironment.SupportModuleDirectory;
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

            MelonLogger.Msg($"Support Module Loaded: {ModulePath}");

            return true;
        }

        // Module Listing
        private class ModuleListing
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