using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MelonLoader
{
    public static class MelonCompatibilityLayer
    {
        internal static bool CreationCheck = true;
        private static string BaseDirectory = null;
        private static List<ModuleListing> Modules = new List<ModuleListing>()
        {
            // Il2Cpp Unity Tls
            new ModuleListing("Il2CppUnityTls.dll", x =>
            {
                x.SetupType = SetupType.OnPreInitialization;
                x.ShouldDelete = !MelonUtils.IsGameIl2Cpp();
            }),

            // Illusion Plugin Architecture
            new ModuleListing("IPA.dll", x =>
            {
                x.SetupType = SetupType.OnPreInitialization;
                x.ShouldDelete = MelonUtils.IsGameIl2Cpp();
            }),

            // MuseDashModLoader
            new ModuleListing("MDML.dll", x =>
            {
                x.SetupType = SetupType.OnPreInitialization;
                x.ShouldDelete = !MelonUtils.IsMuseDash || MelonUtils.IsGameIl2Cpp();
            }),

            // Demeo Integration
            new ModuleListing("Demeo.dll", x =>
            {
                x.SetupType = SetupType.OnApplicationStart;
                x.ShouldDelete = !MelonUtils.IsDemeo;
            }),
        };

        internal static void Setup()
        {
            BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MelonUtils.BaseDirectory, "MelonLoader"), "Dependencies"), "CompatibilityLayers");
        }

        internal enum SetupType
        {
            OnPreInitialization,
            OnApplicationStart
        }
        internal static void SetupModules(SetupType setupType)
        {
            if (!Directory.Exists(BaseDirectory))
                return;

            LemonEnumerator<ModuleListing> enumerator = new LemonEnumerator<ModuleListing>(Modules);
            while (enumerator.MoveNext())
            {
                string ModulePath = Path.Combine(BaseDirectory, enumerator.Current.FileName);
                if (!File.Exists(ModulePath))
                    continue;

                try
                {
                    if (enumerator.Current.LoadSpecifier == null)
                        continue;

                    ModuleListing.LoadSpecifierArgs args = new ModuleListing.LoadSpecifierArgs();
                    enumerator.Current.LoadSpecifier(args);

                    if (args.ShouldDelete)
                    {
                        File.Delete(ModulePath);
                        continue;
                    }

                    if (args.SetupType != setupType)
                        continue;

                    Assembly assembly = Assembly.LoadFrom(ModulePath);
                    if (assembly == null)
                        continue;
                    Type[] ModuleTypes = assembly.GetValidTypes(x => x.IsSubclassOf(typeof(Module))).ToArray();
                    if ((ModuleTypes.Length <= 0) || (ModuleTypes[0] == null))
                        continue;

                    Module moduleInstance = Activator.CreateInstance(ModuleTypes[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null) as Module;
                    if (moduleInstance == null)
                        continue;

                    moduleInstance.Setup();
                    enumerator.Current.Interface = moduleInstance;
                    MelonDebug.Msg($"Loaded Compatibility Layer: {enumerator.Current.FileName}");
                }
                catch (Exception ex) { MelonDebug.Error($"Compatibility Layer [{enumerator.Current.FileName}] threw an Exception: {ex}"); continue; }
            }
        }

        // Module Base
        public class Module { public virtual void Setup() { } }

        // Module Listing
        private class ModuleListing
        {
            internal string FileName = null;
            internal Module Interface = null;
            internal class LoadSpecifierArgs
            {
                internal SetupType SetupType = SetupType.OnPreInitialization;
                internal bool ShouldDelete = false;
            }
            internal Action<LoadSpecifierArgs> LoadSpecifier = null;
            internal ModuleListing(string filename, Action<LoadSpecifierArgs> loadSpecifier)
            {
                FileName = filename;
                LoadSpecifier = loadSpecifier;
            }
        }
    }
}