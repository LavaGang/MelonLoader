using System;
using System.Reflection;

namespace MelonLoader.MonoInternals;

[Obsolete("MelonLoader.MonoInternals.MonoResolveManager is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver instead. This will be removed in a future version.", true)]
public static class MonoResolveManager
{
    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.AddSearchDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver.AddSearchDirectory instead. This will be removed in a future version.", true)]
    public static void AddSearchDirectory(string path, int priority = 0)
        => Resolver.SearchDirectoryManager.Add(path, priority);

    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.RemoveSearchDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver.RemoveSearchDirectory instead. This will be removed in a future version.", true)]
    public static void RemoveSearchDirectory(string path)
        => Resolver.SearchDirectoryManager.Remove(path);

    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.OnAssemblyLoadHandler is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver.dOnAssemblyLoad instead. This will be removed in a future version.", true)]
    public delegate void OnAssemblyLoadHandler(Assembly assembly);
    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.OnAssemblyLoad is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver.OnAssemblyLoad instead. This will be removed in a future version.", true)]
    public static event OnAssemblyLoadHandler OnAssemblyLoad;
    internal static void SafeInvoke_OnAssemblyLoad(Assembly assembly)
        => OnAssemblyLoad?.Invoke(assembly);

    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.OnAssemblyResolveHandler is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver.dOnAssemblyResolve instead. This will be removed in a future version.", true)]
    public delegate Assembly OnAssemblyResolveHandler(string name, Version version);
    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.OnAssemblyLoad is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver.OnAssemblyLoad instead. This will be removed in a future version.", true)]
    public static event OnAssemblyResolveHandler OnAssemblyResolve;
    internal static Assembly SafeInvoke_OnAssemblyResolve(string name, Version version)
        => OnAssemblyResolve?.Invoke(name, version);

    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.GetAssemblyResolveInfo is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver.GetAssemblyResolveInfo instead. This will be removed in a future version.", true)]
    public static AssemblyResolveInfo GetAssemblyResolveInfo(string name)
        => (AssemblyResolveInfo)Resolver.AssemblyManager.GetInfo(name);

    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.LoadInfoFromAssembly is Only Here for Compatibility Reasons. Please use MelonLoader.Resolver.MelonAssemblyResolver.LoadInfoFromAssembly instead. This will be removed in a future version.", true)]
    public static void LoadInfoFromAssembly(Assembly assembly)
        => Resolver.AssemblyManager.LoadInfo(assembly);
}
