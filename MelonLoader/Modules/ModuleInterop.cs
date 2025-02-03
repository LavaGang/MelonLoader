namespace MelonLoader.Modules
{
    internal static class ModuleInterop
    {
        internal static MelonEngineModule Engine { get; private set; }
        internal static MelonSupportModule Support { get; private set; }

        internal static void StartEngine()
        {
            Engine = ModuleManager.GetEngine();
            if (Engine == null)
            {
                MelonLogger.Warning("No Engine Module Found! Using Fallback Environment...");
                Core.Stage2();
                Core.Stage3(null);
                return;
            }

            MelonLogger.Msg($"Engine Module Found: {Engine.GetType().Assembly.Location}");
            Engine.Initialize();
        }

        internal static void StartSupport(string path)
        {
            Support = ModuleManager.GetSupport(path);
            if (Support == null)
                return;

            MelonLogger.Msg($"Support Module Found: {path}");
            Support.Initialize();
        }
    }
}
