using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader
{
    public static class ModuleManager
    {
        public static IBootstrapModule FindBootstrapModule()
        {
            foreach (var bootstrapPath in Directory.GetFiles(MelonEnvironment.ModulesDirectory, "*.Bootstrap.dll", SearchOption.AllDirectories))
            {
                if (!File.Exists(bootstrapPath))
                    continue;

                MelonDebug.Msg($"Loading {bootstrapPath}");
                var assembly = Assembly.LoadFrom(bootstrapPath);

                var type = assembly.GetValidTypes().FirstOrDefault(t => t.GetInterfaces().Any(i => i == typeof(IBootstrapModule)));
                if (type == null)
                {
                    //MelonLogger.Warning($"Failed to load BootstrapModule '{bootstrapPath}': No type deriving from BootstrapModule found.");
                    continue;
                }

                try
                {
                    var module = (IBootstrapModule)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
                    if (module.IsMyEngine)
                        return module;
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"Failed to initialize BootstrapModule '{bootstrapPath}':\n{ex}");
                    continue;
                }
            }

            return null;
        }

        internal static IEngineModule LoadModule(string modulePath)
        {
            if (!File.Exists(modulePath))
                return null;
            
            var assembly = Assembly.LoadFrom(modulePath);
            
            var type = assembly.GetValidTypes().FirstOrDefault(t => t.GetInterfaces().Any(i => i == typeof(IEngineModule)));
            if (type == null)
            {
                MelonLogger.Warning($"Failed to load EngineModule '{modulePath}': No type deriving from EngineModule found.");
                return null;
            }
            
            try
            {
                var module = (IEngineModule)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
                return module;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Failed to initialize EngineModule '{modulePath}':\n{ex}");
                return null;
            }
        }
    }
}