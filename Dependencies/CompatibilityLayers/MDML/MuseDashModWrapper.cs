using ModHelper;

namespace MelonLoader
{
    internal class MuseDashModWrapper : MelonMod
    {
        internal IMod modInstance;
        public override void OnInitializeMelon() => modInstance.DoPatching();
    }
}