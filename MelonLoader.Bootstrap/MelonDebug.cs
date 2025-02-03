using System.Drawing;

namespace MelonLoader.Bootstrap;

internal static class MelonDebug
{
    private static readonly InternalLogger logger = new(Color.CornflowerBlue, "BS DEBUG");

    public static void Log(string msg)
    {
        if (!LoaderConfig.Current.Loader.DebugMode)
            return;

        logger.Msg(msg);
    }
}
