﻿using System;
using System.Reflection;
using MelonLoader.MonoInternals.ResolveInternals;

namespace MelonLoader.MonoInternals
{
    public static class MonoResolveManager
    {
        internal static bool Setup()
        {
            if (!AssemblyManager.Setup())
                return false;

            // Setup Search Directories
            string[] searchdirlist =
            {
                MelonUtils.UserLibsDirectory,
                MelonHandler.PluginsDirectory,
                MelonHandler.ModsDirectory,
                MelonUtils.BaseDirectory,
                MelonUtils.GameDirectory
            };
            foreach (string path in searchdirlist)
                AddSearchDirectory(path);

            // Setup Redirections
            string[] assembly_list =
            {
                "MelonLoader",
                "MelonLoader.ModHandler",
                "Mono.Cecil",
                "Mono.Cecil.Mdb",
                "Mono.Cecil.Pdb",
                "Mono.Cecil.Rocks",
                "MonoMod.RuntimeDetour",
                "MonoMod.Utils",
                "0Harmony",
                "Tomlet"
            };
            Assembly base_assembly = typeof(MonoResolveManager).Assembly;
            foreach (string assemblyName in assembly_list)
                GetAssemblyResolveInfo(assemblyName).Override = base_assembly;

            MelonDebug.Msg("[MonoResolveManager] Setup Successful!");

            return true;
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
            => OnAssemblyLoad?.Invoke(assembly);

        public delegate Assembly OnAssemblyResolveHandler(string name, Version version);
        public static event OnAssemblyResolveHandler OnAssemblyResolve;
        internal static Assembly SafeInvoke_OnAssemblyResolve(string name, Version version)
            => OnAssemblyResolve?.Invoke(name, version);

        public static AssemblyResolveInfo GetAssemblyResolveInfo(string name)
            => AssemblyManager.GetInfo(name);
        public static void LoadInfoFromAssembly(Assembly assembly)
            => AssemblyManager.LoadInfo(assembly);
    }
}
