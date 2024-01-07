namespace MelonLoader.Melons;

public abstract class MelonPlugin : MelonTypeBase<MelonPlugin>
{
    static MelonPlugin()
    {
        TypeName = "Plugin";
    }
}