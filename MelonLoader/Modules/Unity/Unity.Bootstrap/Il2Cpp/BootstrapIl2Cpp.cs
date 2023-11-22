using MelonLoader.Shared.Utils;

namespace MelonLoader.Unity.Il2Cpp
{
    internal static class BootstrapIl2Cpp
    {
        internal static void Startup(string gameAssemblyPath)
        {
            // Log Engine Variant
            MelonLogger.Msg("Engine Variant: Il2Cpp");
        }
    }
}
