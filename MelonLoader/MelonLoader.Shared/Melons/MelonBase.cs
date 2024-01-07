using HarmonyLib;

namespace MelonLoader.Melons;

public abstract class MelonBase<T> where T : MelonBase<T>
{
    public MelonInfoAttribute Info { get; private set; }
    public Harmony HarmonyInstance { get; private set; }

    public static string TypeName { get; protected internal set; }

    static MelonBase() => System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
    
    public virtual void OnEarlyInitializeMelon() { }
    public virtual void OnInitializeMelon() { }
    public virtual void OnDeinitializeMelon() { }
}