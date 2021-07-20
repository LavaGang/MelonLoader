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

            // Demeo Integration
            new ModuleListing("Demeo.dll", x =>
            {
                x.ShouldLoad = x.SetupType == SetupType.OnApplicationStart;
                x.ShouldDelete = MelonUtils.IsGameIl2Cpp() || !MelonUtils.IsDemeo;
            }),
        };

        internal static void Setup()
        {
            BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MelonUtils.BaseDirectory, "MelonLoader"), "Dependencies"), "CompatibilityLayers");

            string versionending = ", Version=";
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
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

            CompatibilityLayers.Melon_Resolver.Setup();
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
                        args.SetupType = setupType;
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

            MelonBase instance = Activator.CreateInstance(creationData.Info.SystemType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null) as MelonBase;
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

        // Assembly to Resolver Conversion
        private delegate Resolver AssemblyToResolverDelegate(Assembly assembly, string filepath);
        private static event Func<Assembly, string, Resolver> AssemblyToResolverEvents;
        public static void AddAssemblyToResolverEvent(Func<Assembly, string, Resolver> evt) => AssemblyToResolverEvents += evt;
        internal static Resolver GetResolverFromAssembly(Assembly assembly, string filepath)
        {
            Delegate[] invoke_list = AssemblyToResolverEvents.GetInvocationList();
            if (invoke_list.Length <= 0)
                return null;
            for (int i = 0; i < invoke_list.Length; i++)
            {
                AssemblyToResolverDelegate func = new AssemblyToResolverDelegate((Func<Assembly, string, Resolver>)invoke_list[i]);
                Resolver resolver = func(assembly, filepath);
                if (resolver != null)
                    return resolver;
            }
            return null;
        }

        // Refresh Event - Plugins
        private static event Action RefreshPluginsEvents;
        public static void AddRefreshPluginsEvent(Action evt) => RefreshPluginsEvents += evt;
        public static void RefreshPlugins() => RefreshPluginsEvents?.Invoke();

        // Refresh Event - Mods
        private static event Action RefreshModsEvents;
        public static void AddRefreshModsEvent(Action evt) => RefreshModsEvents += evt;
        public static void RefreshMods() => RefreshModsEvents?.Invoke();

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
        public class Module { public virtual void Setup() { } }

        // Module Listing
        internal class ModuleListing
        {
            internal string FileName = null;
            internal Module Interface = null;
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