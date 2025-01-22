﻿using MelonLoader.Utils;
using System;
using System.IO;
using System.Reflection;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace MelonLoader.Resolver;

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
            MelonUtils.IsGameIl2Cpp()
                ? MelonEnvironment.Il2CppAssembliesDirectory
                : MelonEnvironment.UnityGameManagedDirectory,
            MelonEnvironment.OurRuntimeDirectory,
            MelonEnvironment.MelonBaseDirectory,
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
        var base_assembly = typeof(MelonAssemblyResolver).Assembly;
        GetAssemblyResolveInfo(base_assembly.GetName().Name).Override = base_assembly;
        GetAssemblyResolveInfo("MelonLoader").Override = base_assembly;
        GetAssemblyResolveInfo("MelonLoader.ModHandler").Override = base_assembly;
    }

    private static void ForceResolveRuntime(params string[] fileNames)
    {
        foreach (var fileName in fileNames)
        {
            var filePath = Path.Combine(MelonEnvironment.OurRuntimeDirectory, fileName);
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
            catch
            {
                assembly = null;
            }

            if (assembly == null)
                return;

            GetAssemblyResolveInfo(Path.GetFileNameWithoutExtension(fileName)).Override = assembly;
        }
    }

    // Search Directories

    public static void AddSearchDirectories(params string[] directories)
    {
        foreach (var directory in directories)
            AddSearchDirectory(directory);
    }

    public static void AddSearchDirectories(int priority, params string[] directories)
    {
        foreach (var directory in directories)
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
#pragma warning disable CS0618 // Type or member is obsolete
        InvokeObsoleteOnAssemblyLoad(assembly);
#pragma warning restore CS0618 // Type or member is obsolete
#endif
        OnAssemblyLoad?.Invoke(assembly);
    }

#if !NET6_0_OR_GREATER
    [Obsolete("Used to make the obsolete event still function.")]
    private static void InvokeObsoleteOnAssemblyLoad(Assembly assembly)
    {
        MonoInternals.MonoResolveManager.SafeInvoke_OnAssemblyLoad(assembly);
    }
#endif

    public delegate Assembly OnAssemblyResolveHandler(string name, Version version);
    public static event OnAssemblyResolveHandler OnAssemblyResolve;

    internal static Assembly SafeInvoke_OnAssemblyResolve(string name, Version version)
    {
#if NET6_0_OR_GREATER

        return OnAssemblyResolve?.Invoke(name, version);

#else

#pragma warning disable CS0618 // Type or member is obsolete
        var assembly = InvokeObsoleteOnAssemblyResolve(name, version);
#pragma warning restore CS0618 // Type or member is obsolete

        assembly ??= OnAssemblyResolve?.Invoke(name, version);
        return assembly;

#endif
    }

#if !NET6_0_OR_GREATER
    [Obsolete("Used to make the obsolete event still function.")]
    private static Assembly InvokeObsoleteOnAssemblyResolve(string name, Version version)
    {
        return MonoInternals.MonoResolveManager.SafeInvoke_OnAssemblyResolve(name, version);
    }
#endif

    public static AssemblyResolveInfo GetAssemblyResolveInfo(string name)
        => AssemblyManager.GetInfo(name);
    public static void LoadInfoFromAssembly(Assembly assembly)
        => AssemblyManager.LoadInfo(assembly);
}
