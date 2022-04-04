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
            if (name.Name == "MelonLoader.ModHandler")
                //Redirect ModHandler to main MelonLoader dll (us)
                return Assembly.GetExecutingAssembly();

            var filename = name.Name + ".dll";

            var managedPath = Path.Combine(MelonEnvironment.MelonManagedDirectory, filename);
            var modsPath = Path.Combine(MelonEnvironment.ModsDirectory, filename);
            var userlibsPath = Path.Combine(MelonEnvironment.UserLibsDirectory, filename);
            var gameRootPath = Path.Combine(MelonEnvironment.GameRootDirectory, filename);

            var ret = TryLoad(alc, managedPath)
                ?? TryLoad(alc, modsPath)
                ?? TryLoad(alc, userlibsPath)
                ?? TryLoad(alc, gameRootPath);

            if (ret == null)
                MelonDebug.Msg($"[DotnetManagedFolder]Failed to find {filename} in any of the known search directories");

            return ret;
        }
#endif
    }
}
