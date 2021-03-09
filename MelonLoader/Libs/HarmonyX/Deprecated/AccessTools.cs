using System;
using System.Reflection;

namespace Harmony
{
    [Obsolete("Harmony.AccessTools is obsolete. Please use HarmonyLib.AccessTools instead.")]
    public static class AccessTools
    {
        [Obsolete("Harmony.AccessTools.Method is obsolete. Please use HarmonyLib.AccessTools.Method instead.")]
        public static MethodInfo Method(Type type, string name, Type[] parameters = null, Type[] generics = null) => HarmonyLib.AccessTools.Method(type, name, parameters, generics);
        [Obsolete("Harmony.AccessTools.Method is obsolete. Please use HarmonyLib.AccessTools.Method instead.")]
        public static MethodInfo Method(string typeColonMethodname, Type[] parameters = null, Type[] generics = null) => HarmonyLib.AccessTools.Method(typeColonMethodname, parameters, generics);
    }
}