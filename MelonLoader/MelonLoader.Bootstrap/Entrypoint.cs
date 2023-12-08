using MelonLoader.Fixes;
using MelonLoader.Utils;
using System.IO;

namespace MelonLoader.Bootstrap
{
    public static class Entrypoint
    {
        public static unsafe void Entry()
        {
            Core.Startup();
            
            MelonDebug.Msg("Starting Up...");
            MelonDebug.Msg(MelonEnvironment.MelonLoaderDirectory);

            MelonLogger.Msg($"Executable: {MelonEnvironment.GameExecutableName}");

            var module = ModuleManager.FindBootstrapModule();
            if (module == null)
            {
                MelonLogger.Warning("Engine is UNKNOWN! Continuing anyways...");
                Core.OnApplicationPreStart();
                Core.OnApplicationStart();
                return;
            }
            
            MelonDebug.Msg("Found Bootstrap Module: " + module.GetType().FullName);
            MelonLogger.Msg($"Running Engine: {module.EngineName}");

            // Fix Resolving Issue
            UnhandledAssemblyResolve.AddSearchDirectoryToFront(Path.Combine(MelonEnvironment.ModulesDirectory, module.EngineName, "net6"));

            module.Startup(); // TO-DO: Implement Fallback Handling for when a Module Fails to Startup
        }
    }   
}