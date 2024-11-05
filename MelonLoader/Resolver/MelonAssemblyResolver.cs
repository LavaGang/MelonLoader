using System;
using System.IO;
using System.Reflection;
using MelonLoader.Utils;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

#pragma warning disable CS0618 // Type or member is obsolete

namespace MelonLoader.Resolver
{
    public class MelonAssemblyResolver
    {
        internal static void Setup()
        {
            if (!AssemblyManager.Setup())
                return;

            // Setup Search Directories
            AddSearchDirectories(
                MelonEnvironment.UserLibsDirectory,
                MelonEnvironment.PluginsDirectory,
                MelonEnvironment.ModsDirectory,
                (MelonUtils.IsGameIl2Cpp()
                    ? MelonEnvironment.Il2CppAssembliesDirectory
                    : MelonEnvironment.UnityGameManagedDirectory),
                MelonEnvironment.OurRuntimeDirectory,
                MelonEnvironment.GameRootDirectory);

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

        public static void AddSearchDirectories(params (string, int)[] directories)
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
        {
#if !NET6_0_OR_GREATER
            // Backwards Compatibility
            MonoInternals.MonoResolveManager.SafeInvoke_OnAssemblyLoad(assembly);
#endif
            OnAssemblyLoad?.Invoke(assembly);
        }

        public delegate Assembly OnAssemblyResolveHandler(string name, Version version);
        public static event OnAssemblyResolveHandler OnAssemblyResolve;
        internal static Assembly SafeInvoke_OnAssemblyResolve(string name, Version version)
        {
#if NET6_0_OR_GREATER

            return OnAssemblyResolve?.Invoke(name, version);

#else

            // Backwards Compatibility
            var assembly = MonoInternals.MonoResolveManager.SafeInvoke_OnAssemblyResolve(name, version);
            if (assembly == null)
                assembly = OnAssemblyResolve?.Invoke(name, version);
            return assembly;

#endif
        }

        public static AssemblyResolveInfo GetAssemblyResolveInfo(string name)
            => AssemblyManager.GetInfo(name);
        public static void LoadInfoFromAssembly(Assembly assembly)
            => AssemblyManager.LoadInfo(assembly);
    }
}
