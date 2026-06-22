using Cpp2IL.Core.Logging;
using Il2CppInterop.Generator;
using MelonLoader.Bootstrap.Logging;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal static class Il2CppInteropGeneration
{
    static Il2CppInteropGeneration()
    {
        Logger.InfoLog += (message, _) => MelonLogger.LogInfo(message, "Il2CppInterop");
        Logger.WarningLog += (message, _) => MelonLogger.LogWarning(message, "Il2CppInterop");
        Logger.ErrorLog += (message, _) => MelonLogger.LogError(message, "Il2CppInterop");
#if DEBUG
        Logger.VerboseLog += (message, _) => MelonLogger.LogInfo(message, "Il2CppInterop");
#endif
    }

    public static void Run(string gameExePath, string outputFolder, string unstripDirectory)
    {
        try
        {
            KeyValuePair<string, string>[] extraData = string.IsNullOrEmpty(unstripDirectory)
                ? []
                : [new KeyValuePair<string, string>(UnstripBaseProcessingLayer.DirectoryKey, unstripDirectory)];
            Il2CppGame.Process(
                gameExePath,
                outputFolder,
                new AsmResolverDllOutputFormatBinding(),
                Il2CppGame.GetDefaultProcessingLayers(),
                extraData);
        }
        catch (Exception ex)
        {
            MelonLogger.LogError(ex.ToString(), "Il2CppInterop");
        }
    }
}
