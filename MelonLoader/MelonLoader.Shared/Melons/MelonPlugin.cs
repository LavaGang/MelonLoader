using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader.Melons;

public class MelonPlugin : IMelonBase, IMelonPlugin
{
    public MelonLogger.Instance Logger { get; set; }
    public MelonPlugin(bool subscribe = true)
    {
        if (!subscribe)
            return;
        
        MelonEvents.OnApplicationPreStart += OnApplicationPreStart;
        MelonEvents.OnApplicationStart += OnApplicationStart;
    }

    public virtual void OnApplicationPreStart()
    {
    }

    public virtual void OnApplicationStart()
    {
    }
}