using System;

namespace MelonLoader.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class HarmonyDontPatchAllAttribute : Attribute { public HarmonyDontPatchAllAttribute() { } }
}