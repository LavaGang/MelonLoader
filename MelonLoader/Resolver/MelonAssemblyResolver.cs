using System;
using System.IO;
using System.Reflection;
using MelonLoader.Utils;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace MelonLoader.Resolver
{
    public static class MelonAssemblyResolver
    {
        internal static void Setup()
        {
#if NET6_0_OR_GREATER
            AssemblyLoadContext.Default.Resolving += Resolve;
#endif

            // Setup all Loaded Assemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                AssemblyManager.LoadInfo(assembly);

            // Add All Folders in Modules Directory as Searchable
            AddSearchDirectories(MelonEnvironment.LoadersDirectory,
                Path.Combine(MelonEnvironment.LoadersDirectory, MelonEnvironment.OurRuntimeName));
            foreach (string directory in Directory.GetDirectories(MelonEnvironment.LoadersDirectory, "*", SearchOption.TopDirectoryOnly))
                AddSearchDirectories(directory, Path.Combine(directory, MelonEnvironment.OurRuntimeName));

            AddSearchDirectories(MelonEnvironment.RuntimeModulesDirectory,
                Path.Combine(MelonEnvironment.RuntimeModulesDirectory, MelonEnvironment.OurRuntimeName));
            foreach (string directory in Directory.GetDirectories(MelonEnvironment.RuntimeModulesDirectory, "*", SearchOption.TopDirectoryOnly))
                AddSearchDirectories(directory, Path.Combine(directory, MelonEnvironment.OurRuntimeName));

            AddSearchDirectories(MelonEnvironment.EngineModulesDirectory,
                Path.Combine(MelonEnvironment.EngineModulesDirectory, MelonEnvironment.OurRuntimeName));
            foreach (string directory in Directory.GetDirectories(MelonEnvironment.EngineModulesDirectory, "*", SearchOption.TopDirectoryOnly))
                AddSearchDirectories(directory, Path.Combine(directory, MelonEnvironment.OurRuntimeName));

            // Setup Search Directories
            AddSearchDirectories(
                MelonEnvironment.UserLibsDirectory,
                MelonEnvironment.PluginsDirectory,
                MelonEnvironment.ModsDirectory,
                MelonEnvironment.OurRuntimeDirectory,
                MelonEnvironment.MelonBaseDirectory,
                MelonEnvironment.ApplicationRootDirectory);

            // Setup Redirections
            OverrideBaseAssembly();

            // Resolve Default Runtime Assemblies
            ForceResolveRuntime(
                "Mono.Cecil.dll",
                "MonoMod.exe",
                "MonoMod.Utils.dll",
                "MonoMod.RuntimeDetour.dll");

            MelonDebug.Msg("[MelonAssemblyResolver] Setup Successful!");
        }

        private static void OverrideBaseAssembly()
        {
            Assembly base_assembly = typeof(MelonAssemblyResolver).Assembly;
            GetAssemblyResolveInfo(base_assembly.GetName().Name).Override = base_assembly;
            GetAssemblyResolveInfo("MelonLoader").Override = base_assembly;
            GetAssemblyResolveInfo("MelonLoader.ModHandler").Override = base_assembly;
        }

        private static void ForceResolveRuntime(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                string filePath = Path.Combine(MelonEnvironment.OurRuntimeDirectory, fileName);
                if (!File.Exists(filePath))
                    return;

                Assembly assembly = null;
                try
                {
#if NET6_0_OR_GREATER
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);
#else
                    assembly = Assembly.LoadFrom(filePath);
#endif
                }
                catch { assembly = null; }

                if (assembly == null)
                    return;

                GetAssemblyResolveInfo(Path.GetFileNameWithoutExtension(fileName)).Override = assembly;
            }
        }

        // Search Directories

        public static void AddSearchDirectories(params string[] directories)
        {
            foreach (string directory in directories)
                AddSearchDirectory(directory);
        }

        public static void AddSearchDirectories(int priority, params string[] directories)
        {
            foreach (string directory in directories)
                AddSearchDirectory(directory, priority);
        }

        public static void AddSearchDirectories(params LemonTuple<string, int>[] directories)
        {
            foreach (var pair in directories)
                AddSearchDirectory(pair.Item1, pair.Item2);
        }

        public static void AddSearchDirectory(string path, int priority = 0)
            => SearchDirectoryManager.Add(path, priority);
        public static void RemoveSearchDirectory(string path)
            => SearchDirectoryManager.Remove(path);

        // Assembly
        public delegate void OnAssemblyLoadHandler(Assembly assembly);
        public static event OnAssemblyLoadHandler OnAssemblyLoad;
        internal static void SafeInvoke_OnAssemblyLoad(Assembly assembly)
            => OnAssemblyLoad?.Invoke(assembly);

        public delegate Assembly OnAssemblyResolveHandler(string name, Version version);
        public static event OnAssemblyResolveHandler OnAssemblyResolve;
        internal static Assembly SafeInvoke_OnAssemblyResolve(string name, Version version)
            => OnAssemblyResolve?.Invoke(name, version);

        public static AssemblyResolveInfo GetAssemblyResolveInfo(string name)
            => AssemblyManager.GetInfo(name);
        public static void LoadInfoFromAssembly(Assembly assembly)
            => AssemblyManager.LoadInfo(assembly);

        public static Assembly Resolve(string requested_name, Version requested_version, bool is_preload)
            => AssemblyManager.Resolve(requested_name, requested_version, is_preload);

#if NET6_0_OR_GREATER
        private static Assembly? Resolve(AssemblyLoadContext alc, AssemblyName name)
            => AssemblyManager.Resolve(name.Name, name.Version, true);
#endif
    }
}
