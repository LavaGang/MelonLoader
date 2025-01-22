using ModHelper;

namespace MelonLoader.CompatibilityLayers;

internal class MuseDashModWrapper : MelonMod
{
    internal IMod modInstance;
    public override void OnInitializeMelon() => modInstance.DoPatching();
}