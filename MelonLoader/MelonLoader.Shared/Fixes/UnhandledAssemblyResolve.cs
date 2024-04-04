using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace MelonLoader.Fixes
{
    public static class UnhandledAssemblyResolve
    {
#if NET6_0_OR_GREATER
        private static AssemblyLoadContext _alc;
#endif

        private static List<string> SearchableDirectories = new List<string>
        {
#if NET6_0_OR_GREATER
            Path.Combine(MelonEnvironment.ModulesDirectory, "Mono", "net6"),
#else
            Path.GetDirectoryName(typeof(UnhandledAssemblyResolve).Assembly.Location), // Need to add the MelonLoader.Shared directory manually
            Path.Combine(Path.Combine(MelonEnvironment.ModulesDirectory, "Mono"),
#if NET35
                "net35"),
#else
                "netstandard2.1"),
#endif

#endif

            MelonEnvironment.ModulesDirectory,
            MelonEnvironment.UserLibsDirectory,
            MelonEnvironment.MelonsDirectory,
            MelonEnvironment.MelonBaseDirectory,
            MelonEnvironment.GameRootDirectory
        };

        internal static void Install()
        {
#if NET6_0_OR_GREATER
            AssemblyLoadContext.Default.Resolving += OnResolve;
#else
            AppDomain.CurrentDomain.AssemblyResolve += OnResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnResolveReflectionOnly;
#endif
        }

        public static void AddSearchDirectoryToFront(string path)
        {
            if (SearchableDirectories.Contains(path))
                return;

            string[] dirArr = SearchableDirectories.ToArray();
            SearchableDirectories.Clear();
            SearchableDirectories.Add(path);
            SearchableDirectories.AddRange(dirArr);
        }

        public static void AddSearchDirectory(string path)
        {
            if (SearchableDirectories.Contains(path))
                return;
            SearchableDirectories.Add(path);
        }

        public static void RemoveSearchDirectory(string path)
        {
            if (!SearchableDirectories.Contains(path))
                return;
            SearchableDirectories.Remove(path);
        }

        private static Assembly FindAssembly(string name, Func<string, Assembly> tryLoad)
        {
            var filename = name + ".dll";

            Assembly ret = null;
            foreach (string folder in SearchableDirectories)
            {
                if (!Directory.Exists(folder))
                    continue;

                ret = tryLoad(Path.Combine(folder, filename));
                if (ret != null)
                    return ret;

                foreach (var childFolder in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories))
                {
                    ret = tryLoad(Path.Combine(childFolder, filename));
                    if (ret != null)
                        return ret;
                }
            }

            if (ret == null)
                MelonDebug.Msg($"[{nameof(UnhandledAssemblyResolve)}] Failed to find {filename} in any of the known search directories");
            return ret;
        }

        private static Assembly TryLoad(string path)
        {
            if (!File.Exists(path))
                return null;

            MelonDebug.Msg($"[{nameof(UnhandledAssemblyResolve)}] Loading from {path}...");
#if NET6_0_OR_GREATER
            return _alc.LoadFromAssemblyPath(path);
#else
            return Assembly.LoadFrom(path);
#endif
        }

#if NET6_0_OR_GREATER
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