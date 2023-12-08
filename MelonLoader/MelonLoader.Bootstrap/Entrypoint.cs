using MelonLoader.Utils;

namespace MelonLoader.Bootstrap
{
    public static class Entrypoint
    {
        public static unsafe void Entry()
        {
            Core.Startup();
            
            MelonDebug.Msg("Starting Up...");
            MelonDebug.Msg(MelonEnvironment.MelonLoaderDirectory);
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
            module.Startup();
        }
    }   
}