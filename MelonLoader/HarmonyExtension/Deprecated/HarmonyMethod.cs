using System;
using System.Reflection;

namespace Harmony
{
    [Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead.")]
    public class HarmonyMethod : HarmonyLib.HarmonyMethod
    {
        [Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead.")]
        public HarmonyMethod() : base() { }
        [Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead.")]
        public HarmonyMethod(MethodInfo method) : base(method) { }
        [Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead.")]
        public HarmonyMethod(Type type, string name, Type[] parameters = null) : base(type, name, parameters) { }
    };
}