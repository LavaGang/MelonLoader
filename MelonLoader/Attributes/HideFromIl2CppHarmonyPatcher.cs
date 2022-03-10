using System;

namespace MelonLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HideFromIl2CppHarmonyPatcherAttribute : Attribute { public HideFromIl2CppHarmonyPatcherAttribute() { } }
}
