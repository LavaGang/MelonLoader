using MelonLoader;
using MelonLoader.Melons.Sample;

[assembly:MelonInfo(typeof(MelonSamplePlugin), "InternalSamplePlugin", 1, 0, 0, "LavaGang")]

namespace MelonLoader.Melons.Sample;

public class MelonSamplePlugin : MelonPlugin
{
    public override void OnApplicationPreStart()
    {
        Logger.Msg("Hello, big bang!");
    }

    public override void OnApplicationStart()
    {
        Logger.Msg("Hello, world!");
    }
}