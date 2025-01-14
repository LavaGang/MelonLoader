using MelonLoader.Bootstrap.Logging;
using MelonLoader.Bootstrap.Utils;
using System.Drawing;

namespace MelonLoader.Bootstrap;

internal class InternalLogger(ColorRGB sectionColor, string sectionName)
{
    private static readonly bool hideWarnings = ArgParser.IsDefined("melonloader.hidewarnings");

    public void Msg(string msg)
    {
        MelonLogger.Log(Color.LightGray, msg, sectionColor, sectionName);
    }

    public void Error(string msg)
    {
        MelonLogger.LogError(msg, sectionName);
    }

    public void Warning(string msg)
    {
        if (hideWarnings)
            return;

        MelonLogger.Log(Color.Yellow, msg, Color.Yellow, sectionName);
    }
}
