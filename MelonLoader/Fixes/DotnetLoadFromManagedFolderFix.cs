#if NET6_0_OR_GREATER
using MelonLoader.Utils;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace MelonLoader.Fixes
{
    internal static class DotnetLoadFromManagedFolderFix
    {
        //TODO Update for non-windows platforms in future, or when updating runtime
        private static readonly string OurRuntimeDir = Path.Combine(MelonEnvironment.OurRuntimeDirectory, "runtimes", "win", "lib", "net6.0"); 

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
            var ret = TryFind(alc, name);
            if (ret == null)
                MelonDebug.Msg($"[DotnetManagedFolder] Failed to find {name.Name} in any of the known search directories");
            return ret;
        }

        internal static Assembly TryFind(AssemblyLoadContext alc, AssemblyName name)
        {
            // Redirect ModHandler to main MelonLoader dll (us)
            if (name.Name == "MelonLoader.ModHandler")
                return Assembly.GetExecutingAssembly();

            var ret = TryLoadFromFolders(alc, name.Name + ".dll");
            if (ret == null)
                TryLoadFromFolders(alc, name.Name + ".exe");

            return ret;
        }

        private static Assembly TryLoadFromFolders(AssemblyLoadContext alc, string filename)
        {
            var osSpecificPath = Path.Combine(OurRuntimeDir, filename);
            var il2cppPath = Path.Combine(MelonEnvironment.Il2CppAssembliesDirectory, filename);
            var managedPath = Path.Combine(MelonEnvironment.MelonManagedDirectory, filename);
            var modsPath = Path.Combine(MelonEnvironment.ModsDirectory, filename);
            var userlibsPath = Path.Combine(MelonEnvironment.UserLibsDirectory, filename);
            var gameRootPath = Path.Combine(MelonEnvironment.GameRootDirectory, filename);
            var runtimeSpecificPath = Path.Combine(MelonEnvironment.OurRuntimeDirectory, filename);

            var ret = TryLoad(alc, osSpecificPath)
                ?? TryLoad(alc, il2cppPath)
                ?? TryLoad(alc, managedPath)
                ?? TryLoad(alc, modsPath)
                ?? TryLoad(alc, userlibsPath)
                ?? TryLoad(alc, runtimeSpecificPath)
                ?? TryLoad(alc, gameRootPath);

            return ret;
        }
    }
}
#endif