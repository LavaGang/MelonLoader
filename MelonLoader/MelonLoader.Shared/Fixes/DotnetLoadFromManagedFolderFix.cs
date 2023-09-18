
using MelonLoader.Shared.Utils;
#if NET6_0
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
#endif

namespace MelonLoader.Shared.Fixes
{
    public static class DotnetLoadFromManagedFolderFix
    {

#if !NET6_0
        internal static void Install()
        {

        }
#else
        //TODO Update for non-windows platforms in future, or when updating runtime
        //private static readonly string OurRuntimeDir = Path.Combine(MelonEnvironment.OurRuntimeDirectory, "runtimes", "win", "lib", "net6.0"); 

        public static void Install()
        {
            AssemblyLoadContext.Default.Resolving += OnResolve;
        }

        private static Assembly TryLoad(AssemblyLoadContext alc, string path)
        {
            if (File.Exists(path))
            {
                MelonDebug.Msg($"[DotnetManagedFolder] Loading from {path}...");
                return alc.LoadFromAssemblyPath(path);
            }

            return null;
        }

        private static Assembly OnResolve(AssemblyLoadContext alc, AssemblyName name)
        {
            if (name.Name == "MelonLoader.ModHandler")
                return Assembly.GetExecutingAssembly();

            var filename = name.Name + ".dll";
            
            var gameRootPath = Path.Combine(MelonEnvironment.MelonBaseDirectory, filename);
            var ourRuntimeFolder = Path.Combine(MelonEnvironment.MelonLoaderDirectory, "net6", filename);

            var ret = TryLoad(alc, ourRuntimeFolder) ?? TryLoad(alc, gameRootPath);

            if (ret == null)
                MelonDebug.Msg($"[DotnetManagedFolder] Failed to find {filename} in any of the known search directories");

            return ret;
        }
#endif
    }
}