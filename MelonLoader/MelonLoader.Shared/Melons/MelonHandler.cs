using MelonLoader.Utils;

namespace MelonLoader.Melons;

public static class MelonHandler
{
    public static void LoadMelonsFromDirectory<T>(string path) where T : MelonBase<T>
    {
        MelonLogger.WriteSpacer();
        MelonLogger.Msg($"Loading {MelonBase<T>.TypeName}s from '{path}'...");
    }
}