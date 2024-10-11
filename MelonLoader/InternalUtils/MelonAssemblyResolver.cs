using System;
using System.IO;
using System.Reflection;
using MelonLoader.InternalUtils.Resolver;
using MelonLoader.Utils;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

#pragma warning disable CS0618 // Type or member is obsolete

namespace MelonLoader.InternalUtils
{
    public class MelonAssemblyResolver
    {
        internal static void Setup()
        {
            if (!AssemblyManager.Setup())
                return;

            // Setup Search Directories
            string[] searchdirlist =
            {
                MelonEnvironment.UserLibsDirectory,
                MelonEnvironment.PluginsDirectory,
                MelonEnvironment.ModsDirectory,
                MelonEnvironment.MelonBaseDirectory,
                MelonEnvironment.GameRootDirectory,
                MelonEnvironment.OurRuntimeDirectory,
                MelonEnvironment.Il2CppAssembliesDirectory,
                MelonEnvironment.UnityGameManagedDirectory,
            };
            foreach (string path in searchdirlist)
                AddSearchDirectory(path);

            ForceResolveRuntime("Mono.Cecil.dll");
            ForceResolveRuntime("MonoMod.exe");
            ForceResolveRuntime("MonoMod.Utils.dll");
            ForceResolveRuntime("MonoMod.RuntimeDetour.dll");

            // Setup Redirections
            string[] assembly_list =
            {
                "MelonLoader",
                "MelonLoader.ModHandler",
            };
            Assembly base_assembly = typeof(MelonAssemblyResolver).Assembly;
            foreach (string assemblyName in assembly_list)
                GetAssemblyResolveInfo(assemblyName).Override = base_assembly;

            MelonDebug.Msg("[MelonAssemblyResolver] Setup Successful!");
        }

        private static void ForceResolveRuntime(string fileName)
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

        // Search Directories
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
