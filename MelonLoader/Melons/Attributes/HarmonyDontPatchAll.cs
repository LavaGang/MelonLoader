using System;

namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class HarmonyDontPatchAll : Attribute { public HarmonyDontPatchAll() { } }
}
