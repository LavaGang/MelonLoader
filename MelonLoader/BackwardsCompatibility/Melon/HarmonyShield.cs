using System;

namespace Harmony
{
    [Obsolete("Harmony.HarmonyShield is Only Here for Compatibility Reasons. Please use MelonLoader.PatchShield instead.")]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
    public class HarmonyShield : MelonLoader.PatchShield { }
}
