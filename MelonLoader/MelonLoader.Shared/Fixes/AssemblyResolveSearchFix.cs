using MelonLoader.Utils;
using System.IO;
using System.Reflection;

#if NET6_0
using System.Runtime.Loader;
#else
using System;
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
#elif NET35
            Path.Combine(Path.Combine(MelonEnvironment.ModulesDirectory, "Mono"), "net35"),
#else
            Path.Combine(MelonEnvironment.ModulesDirectory, "Mono", "netstandard2.1"),
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
#endif
        }

        private static Assembly FindAssembly(string name)
        {
            var filename = name + ".dll";

            Assembly ret = null;
            foreach (string folder in SearchableDirectories)
            {
                ret = TryLoad(Path.Combine(folder, filename));
                if (ret != null)
                    return ret;

                foreach (var childFolder in Directory.GetDirectories(folder))
                {
                    ret = TryLoad(Path.Combine(childFolder, filename));
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
            if (File.Exists(path))
            {
                MelonDebug.Msg($"[{nameof(AssemblyResolveSearchFix)}] Loading from {path}...");
#if NET6_0
                return _alc.LoadFromAssemblyPath(path);
#else
                return Assembly.LoadFrom(path);
#endif
            }

            return null;
        }

#if NET6_0
        private static Assembly OnResolve(AssemblyLoadContext alc, AssemblyName name)
        {
            _alc = alc;
            return FindAssembly(name.Name);
        }
#else
        private static Assembly OnResolve(object sender, ResolveEventArgs args)
            => FindAssembly(args.Name);
#endif
    }
}