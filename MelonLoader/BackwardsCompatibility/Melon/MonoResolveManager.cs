using System;
using System.Reflection;

namespace MelonLoader.MonoInternals
{
    [Obsolete("MelonLoader.MonoInternals.MonoResolveManager is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver instead.")]
    public static class MonoResolveManager
    {
        [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.AddSearchDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver.AddSearchDirectory instead.")]
        public static void AddSearchDirectory(string path, int priority = 0)
            => InternalUtils.Resolver.SearchDirectoryManager.Add(path, priority);

        [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.RemoveSearchDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver.RemoveSearchDirectory instead.")]
        public static void RemoveSearchDirectory(string path)
            => InternalUtils.Resolver.SearchDirectoryManager.Remove(path);

        [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.OnAssemblyLoadHandler is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver.dOnAssemblyLoad instead.")]
        public delegate void OnAssemblyLoadHandler(Assembly assembly);
        [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.OnAssemblyLoad is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver.OnAssemblyLoad instead.")]
        public static event OnAssemblyLoadHandler OnAssemblyLoad;
        internal static void SafeInvoke_OnAssemblyLoad(Assembly assembly)
            => OnAssemblyLoad?.Invoke(assembly);

        [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.OnAssemblyResolveHandler is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver.dOnAssemblyResolve instead.")]
        public delegate Assembly OnAssemblyResolveHandler(string name, Version version);
        [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.OnAssemblyLoad is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver.OnAssemblyLoad instead.")]
        public static event OnAssemblyResolveHandler OnAssemblyResolve;
        internal static Assembly SafeInvoke_OnAssemblyResolve(string name, Version version)
            => OnAssemblyResolve?.Invoke(name, version);

        [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.GetAssemblyResolveInfo is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver.GetAssemblyResolveInfo instead.")]
        public static AssemblyResolveInfo GetAssemblyResolveInfo(string name)
            => (AssemblyResolveInfo)InternalUtils.Resolver.AssemblyManager.GetInfo(name);

        [Obsolete("MelonLoader.MonoInternals.MonoResolveManager.LoadInfoFromAssembly is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.MelonAssemblyResolver.LoadInfoFromAssembly instead.")]
        public static void LoadInfoFromAssembly(Assembly assembly)
            => InternalUtils.Resolver.AssemblyManager.LoadInfo(assembly);
    }
}
