using MelonLoader.Bootstrap.Logging;
using MelonLoader.Logging;

namespace MelonLoader.Bootstrap;

internal class InternalLogger(ColorARGB sectionColor, string sectionName)
{
    public void Msg(string msg)
    {
        MelonLogger.Log(ColorARGB.LightGray, msg, sectionColor, sectionName);
    }

    public void Error(string msg)
    {
        MelonLogger.LogError(msg, sectionName);
    }

    public void Warning(string msg)
    {
        MelonLogger.LogWarning(msg, sectionName);
    }
}
