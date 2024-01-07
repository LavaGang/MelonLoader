namespace MelonLoader.Melons;

public abstract class MelonPlugin : MelonBase<MelonPlugin>
{
    static MelonPlugin() => TypeName = "Plugin";
}