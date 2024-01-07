using HarmonyLib;
using MelonLoader.Utils;

namespace MelonLoader.Melons;

public abstract class MelonBase
{
    public MelonInfoAttribute Info { get; private set; }
    public Harmony HarmonyInstance { get; private set; }
    
    public MelonLogger.Instance LoggerInstance { get; internal set; }

    public virtual void OnEarlyInitializeMelon() { }
    public virtual void OnInitializeMelon() { }
    public virtual void OnDeinitializeMelon() { }
}