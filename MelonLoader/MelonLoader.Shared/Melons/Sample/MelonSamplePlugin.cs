using System.Reflection;
using MelonLoader;
using MelonLoader.Interfaces;
using MelonLoader.Melons.Sample;

[assembly:MelonInfo(typeof(MelonSamplePlugin), "InternalSamplePlugin", 1, 0, 0, "LavaGang")]

namespace MelonLoader.Melons.Sample;

public class MelonSamplePlugin : MelonPlugin
{
    public override void OnApplicationStart()
    {
        Logger.Msg("Hello, world!");
    }
}