using MelonLoader.Utils;
using System.IO;

namespace MelonLoader.Modules
{
    internal static class ModuleManager
    {
        internal static MelonEngineModule GetEngine()
        {
            if (FindEngineModule(MelonEnvironment.LoadersDirectory, out MelonEngineModule module))
                return module;

            foreach (var directory in Directory.GetDirectories(MelonEnvironment.LoadersDirectory, "*", SearchOption.TopDirectoryOnly))
                if (FindEngineModule(directory, out module))
                    return module;

            if (FindEngineModule(MelonEnvironment.EngineModulesDirectory, out module))
                return module;

            foreach (var directory in Directory.GetDirectories(MelonEnvironment.EngineModulesDirectory, "*", SearchOption.TopDirectoryOnly))
                if (FindEngineModule(directory, out module))
                    return module;

            return null;
        }

        internal static MelonSupportModule GetSupport(string filePath)
            => MelonModule.Load<MelonSupportModule>(filePath);

        private static bool FindEngineModule(string directory, out MelonEngineModule module)
        {
            foreach (var filePath in Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly))
            {
                module = MelonModule.Load<MelonEngineModule>(filePath);
                if ((module == null)
                    || !module.Validate())
                    continue;
                return true;
            }

            module = null;
            return false;
        }
    }
}
