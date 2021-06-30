using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MelonLoader
{
    public static class MelonCompatibilityLayer
    {
        internal static bool CreationCheck = true;
        private static string BaseDirectory = null;
        private static List<ModuleListing> Modules = new List<ModuleListing>()
        {
            // Illusion Plugin Architecture
            new ModuleListing("IPA.dll", x =>
            {
                x.ShouldLoad = x.SetupType == SetupType.OnPreInitialization;
                x.ShouldDelete = MelonUtils.IsGameIl2Cpp();
            }),

            // MuseDashModLoader
            new ModuleListing("MDML.dll", x =>
            {
                x.ShouldLoad = x.SetupType == SetupType.OnPreInitialization;
                x.ShouldDelete = MelonUtils.IsGameIl2Cpp();
            }),
        };

        internal static void Setup(AppDomain domain)
        {
            BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MelonUtils.GameDirectory, "MelonLoader"), "Dependencies"), "CompatibilityLayers");

            string versionending = ", Version=";
            domain.AssemblyResolve += (sender, args) =>
                (args.Name.StartsWith($"Mono.Cecil{versionending}")
                || args.Name.StartsWith($"Mono.Cecil.Mdb{versionending}")
                || args.Name.StartsWith($"Mono.Cecil.Pdb{versionending}")
                || args.Name.StartsWith($"Mono.Cecil.Rocks{versionending}")
                || args.Name.StartsWith($"MonoMod.RuntimeDetour{versionending}")
                || args.Name.StartsWith($"MonoMod.Utils{versionending}")
                || args.Name.StartsWith($"0Harmony{versionending}")
                || args.Name.StartsWith($"Tomlet{versionending}"))
                ? typeof(MelonCompatibilityLayer).Assembly
                : null;

            CompatibilityLayers.Melon_CL.Setup(domain);
        }

        internal enum SetupType
        {
            OnPreInitialization,
            OnApplicationStart
        }
        internal static void SetupModules(SetupType setupType, AppDomain domain)
        {
            if (!Directory.Exists(BaseDirectory))
                return;

            ModuleEnumerator enumerator = new ModuleEnumerator();
            while (enumerator.MoveNext())
            {
                string ModulePath = Path.Combine(BaseDirectory, enumerator.Current.FileName);
                if (!File.Exists(ModulePath))
                    continue;

                try
                {
                    if (enumerator.Current.LoadSpecifier != null)
                    {
                        ModuleListing.LoadSpecifierArgs args = new ModuleListing.LoadSpecifierArgs();
                        enumerator.Current.LoadSpecifier(args);
                        if (!args.ShouldLoad)
                            continue;

                        if (args.ShouldDelete)
                        {
                            File.Delete(ModulePath);
                            continue;
                        }
                    }

                    Assembly assembly = Assembly.LoadFrom(ModulePath);
                    if (assembly == null)
                        continue;

                    Type[] ModuleTypes = assembly.GetValidTypes(x => x.GetInterfaces().Contains(typeof(Module))).ToArray();
                    if ((ModuleTypes.Length <= 0) || (ModuleTypes[0] == null))
                        continue;

                    Module Interface = (Module)Activator.CreateInstance(ModuleTypes[0]);
                    if (Interface == null)
                        continue;

                    Interface.Setup(domain);
                }
                catch (Exception ex) { MelonDebug.Error(ex.ToString()); continue; }
            }
        }

        public class WrapperData
        {
            public Assembly Assembly = null;
            public MelonInfoAttribute Info = null;
            public MelonGameAttribute[] Games = null;
            public MelonOptionalDependenciesAttribute OptionalDependencies = null;
            public ConsoleColor ConsoleColor = MelonLogger.DefaultMelonColor;
            public int Priority = 0;
            public string Location = null;
        }
        public static T CreateMelon<T>(this WrapperData creationData) where T : MelonBase
        {
            MelonBase baseMelon = CreateMelon(creationData);
            if (baseMelon == null)
                return default;
            return baseMelon as T;
        }
        public static MelonBase CreateMelon(this WrapperData creationData)
        {
            if (CreationCheck)
            {
                if (creationData.Info.SystemType.IsSubclassOf(typeof(MelonMod)))
                {
                    MelonLogger.Error($"Mod {creationData.Info.Name} is in the Plugins Folder: {creationData.Location}");
                    return null;
                }
            }

            MelonBase instance = FormatterServices.GetUninitializedObject(creationData.Info.SystemType) as MelonBase;
            if (instance == null)
            {
                MelonLogger.Error($"Failed to Create Instance for {creationData.Location}");
                return null;
            }
            instance.Assembly = creationData.Assembly;
            instance.Info = creationData.Info;
            instance.Games = creationData.Games;
            instance.OptionalDependencies = creationData.OptionalDependencies;
            instance.Location = creationData.Location;
            instance.Priority = creationData.Priority;
            instance.ConsoleColor = creationData.ConsoleColor;
            instance.HarmonyInstance = new HarmonyLib.Harmony(instance.Assembly.FullName);
            return instance;
        }

        // Assembly to Compatibility Layer Conversion
        private static event Action<LayerResolveEventArgs> ResolveAssemblyToLayerResolverEvents;
        public static void AddResolveAssemblyToLayerResolverEvent(Action<LayerResolveEventArgs> evt) => ResolveAssemblyToLayerResolverEvents += evt;
        internal static Resolver ResolveAssemblyToLayerResolver(Assembly asm, string filepath)
        {
            LayerResolveEventArgs args = new LayerResolveEventArgs();
            args.assembly = asm;
            args.filepath = filepath;
            ResolveAssemblyToLayerResolverEvents?.Invoke(args);
            return args.inter;
        }

        // Refresh Event - Plugins
        private static event Action RefreshPluginsTableEvents;
        public static void AddRefreshPluginsTableEvent(Action evt) => RefreshPluginsTableEvents += evt;
        public static void RefreshPluginsTable() => RefreshPluginsTableEvents?.Invoke();

        // Refresh Event - Mods
        private static event Action RefreshModsTableEvents;
        public static void AddRefreshModsTableEvent(Action evt) => RefreshModsTableEvents += evt;
        public static void RefreshModsTable() => RefreshModsTableEvents?.Invoke();

        // Resolver Event Args
        public class LayerResolveEventArgs : EventArgs
        {
            public Assembly assembly;
            public string filepath;
            public Resolver inter;
        }

        // Resolver Base
        public class Resolver
        {
            public readonly Assembly Assembly = null;
            public readonly string FilePath = null;
            public Resolver(Assembly assembly, string filepath)
            {
                Assembly = assembly;
                FilePath = filepath;
            }
            public virtual void CheckAndCreate(ref List<MelonBase> melonTbl) { }
        }

        // Module Base
        public interface Module
        {
            public abstract void Setup(AppDomain domain);
        }

        // Module Listing
        internal class ModuleListing
        {
            internal string FileName = null;
            internal class LoadSpecifierArgs
            {
                internal SetupType SetupType = SetupType.OnPreInitialization;
                internal bool ShouldLoad = false;
                internal bool ShouldDelete = false;
            }
            internal Action<LoadSpecifierArgs> LoadSpecifier = null;
            internal ModuleListing(string filename)
                => FileName = filename;
            internal ModuleListing(string filename, Action<LoadSpecifierArgs> loadSpecifier)
            {
                FileName = filename;
                LoadSpecifier = loadSpecifier;
            }
        }

        // Module Enumerator
        internal class ModuleEnumerator : IEnumerator
        {
            private ModuleListing[] ObjectTable;
            private int CurrentIndex = 0;
            internal ModuleEnumerator()
                => ObjectTable = Modules.ToArray();
            object IEnumerator.Current => Current;
            public ModuleListing Current { get; private set; }
            public bool MoveNext()
            {
                if ((ObjectTable == null)
                    || (ObjectTable.Length <= 0)
                    || (CurrentIndex >= ObjectTable.Length))
                    return false;
                Current = ObjectTable[CurrentIndex];
                CurrentIndex++;
                return true;
            }
            public void Reset()
            {
                CurrentIndex = 0;
                Current = default;
            }
        }
    }
}