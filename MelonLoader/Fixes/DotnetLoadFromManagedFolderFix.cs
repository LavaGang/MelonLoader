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

        private static System.Reflection.Assembly OnResolve(AssemblyLoadContext alc, AssemblyName name)
        {
            var potentialDllPath = Path.Combine(MelonEnvironment.MelonManagedDirectory, name.Name + ".dll");
            if (File.Exists(potentialDllPath))
                return alc.LoadFromAssemblyPath(potentialDllPath);

            return null;
        }
#endif
    }
}
