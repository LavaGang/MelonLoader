using MelonLoader.Interfaces;
using MelonLoader.Utils;

namespace MelonLoader.Melons;

public class MelonMod : IMelonBase
{
    public MelonLogger.Instance Logger { get; set; }

    public virtual void OnApplicationStart() { }
}