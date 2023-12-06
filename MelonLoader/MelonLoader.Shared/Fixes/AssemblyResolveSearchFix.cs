using MelonLoader.Utils;
using System;
using System.IO;
using System.Reflection;

#if NET6_0
using System.Runtime.Loader;
#endif

namespace MelonLoader.Fixes
{
    public static class AssemblyResolveSearchFix
    {
#if NET6_0
        private static AssemblyLoadContext _alc;
#endif

        private static string[] SearchableDirectories = new string[]
        {
#if NET6_0
            Path.Combine(MelonEnvironment.ModulesDirectory, "Mono", "net6"),
#else
            Path.GetDirectoryName(typeof(AssemblyResolveSearchFix).Assembly.Location), // Need to add the MelonLoader.Shared directory manually
            Path.Combine(Path.Combine(MelonEnvironment.ModulesDirectory, "Mono"),
#if NET35
                "net35"),
#else
                "netstandard2.1"),
#endif

#endif

            MelonEnvironment.ModulesDirectory,
            MelonEnvironment.UserLibsDirectory,
            MelonEnvironment.PluginsDirectory,
            MelonEnvironment.ModsDirectory,
            MelonEnvironment.MelonBaseDirectory,
            MelonEnvironment.GameRootDirectory
        };

        public static void Install()
        {
#if NET6_0
            AssemblyLoadContext.Default.Resolving += OnResolve;
#else
            AppDomain.CurrentDomain.AssemblyResolve += OnResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnResolveReflectionOnly;
#endif
        }

        private static Assembly FindAssembly(string name, Func<string, Assembly> tryLoad)
        {
            var filename = name + ".dll";

            Assembly ret = null;
            foreach (string folder in SearchableDirectories)
            {
                ret = tryLoad(Path.Combine(folder, filename));
                if (ret != null)
                    return ret;

                foreach (var childFolder in Directory.GetDirectories(folder))
                {
                    ret = tryLoad(Path.Combine(childFolder, filename));
                    if (ret != null)
                        return ret;
                }
            }

            if (ret == null)
                MelonDebug.Msg($"[{nameof(AssemblyResolveSearchFix)}] Failed to find {filename} in any of the known search directories");
            return ret;
        }

        private static Assembly TryLoad(string path)
        {
            if (!File.Exists(path))
                return null;

            MelonDebug.Msg($"[{nameof(AssemblyResolveSearchFix)}] Loading from {path}...");
#if NET6_0
            return _alc.LoadFromAssemblyPath(path);
#else
            return Assembly.LoadFrom(path);
#endif
        }

#if NET6_0
        private static Assembly OnResolve(AssemblyLoadContext alc, AssemblyName name)
        {
            _alc = alc;
            return FindAssembly(name.Name, TryLoad);
        }
#else
        private static Assembly OnResolve(object sender, ResolveEventArgs args)
            => FindAssembly(args.Name, TryLoad);
        private static Assembly OnResolveReflectionOnly(object sender, ResolveEventArgs args)
            => FindAssembly(args.Name, TryLoadReflectionOnly);
        private static Assembly TryLoadReflectionOnly(string path)
        {
            if (!File.Exists(path))
                return null;
            return Assembly.ReflectionOnlyLoadFrom(path);
        }
#endif
    }
}