#if NET6_0
using MelonLoader.Utils;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
#endif

namespace MelonLoader.Fixes
{
    internal class DotnetLoadFromManagedFolderFix
    {
#if !NET6_0
        internal static void Install()
        {

        }
#else 
        internal static void Install()
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
            var filename = name.Name + ".dll";
            var managedPath = Path.Combine(MelonEnvironment.MelonManagedDirectory, filename);
            var userlibsPath = Path.Combine(MelonEnvironment.UserLibsDirectory, name.Name + ".dll");

            return TryLoad(alc, managedPath) ?? TryLoad(alc, userlibsPath);
        }
#endif
    }
}
