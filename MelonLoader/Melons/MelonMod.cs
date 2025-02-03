using System;

namespace MelonLoader
{
    public abstract class MelonMod : MelonTypeBase<MelonMod>
    {
        static MelonMod()
        {
            TypeName = "Mod";
        }

        protected private override bool RegisterInternal()
        {
            try
            {
                OnPreSupportModule();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to register {MelonTypeName} '{MelonAssembly.Location}': Melon failed to initialize in the deprecated OnPreSupportModule callback!");
                MelonLogger.Error(ex.ToString());
                return false;
            }

            if (!base.RegisterInternal())
                return false;

            if (MelonEvents.MelonHarmonyInit.Disposed)
                HarmonyInit();
            else
                MelonEvents.MelonHarmonyInit.Subscribe(HarmonyInit, Priority, true);

            return true;
        }

        private void HarmonyInit()
        {
            if (!MelonAssembly.HarmonyDontPatchAll)
                HarmonyInstance.PatchAll(MelonAssembly.Assembly);
        }
    }
}