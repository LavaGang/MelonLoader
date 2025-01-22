using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony;

[Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead. This will be removed in a future version.", true)]
public class HarmonyMethod : HarmonyLib.HarmonyMethod
{
    [Obsolete("Harmony.HarmonyMethod.prioritiy is obsolete. Please use HarmonyLib.HarmonyMethod.priority instead. This will be removed in a future version.", true)]
    public int prioritiy = -1;
    [Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead. This will be removed in a future version.", true)]
    public HarmonyMethod() : base() { }
    [Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead. This will be removed in a future version.", true)]
    public HarmonyMethod(MethodInfo method) : base(method) { }
    [Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead. This will be removed in a future version.", true)]
    public HarmonyMethod(Type type, string name, Type[] parameters = null) : base(type, name, parameters) { }
    [Obsolete("Harmony.HarmonyMethod.Merge is obsolete. Please use HarmonyLib.HarmonyMethod.Merge instead. This will be removed in a future version.", true)]
    public static HarmonyMethod Merge(List<HarmonyMethod> attributes) => (HarmonyMethod)Merge(Array.ConvertAll(attributes.ToArray(), x => (HarmonyLib.HarmonyMethod)x).ToList());
    public override string ToString() => base.ToString();
}

[Obsolete("Harmony.HarmonyMethodExtensions is obsolete. Please use HarmonyLib.HarmonyMethodExtensions instead. This will be removed in a future version.", true)]
public static class HarmonyMethodExtensions
{
    [Obsolete("Harmony.HarmonyMethodExtensions.CopyTo is obsolete. Please use HarmonyLib.HarmonyMethodExtensions.CopyTo instead. This will be removed in a future version.", true)]
    public static void CopyTo(this HarmonyMethod from, HarmonyMethod to) => HarmonyLib.HarmonyMethodExtensions.CopyTo(from, to);
    [Obsolete("Harmony.HarmonyMethodExtensions.Clone is obsolete. Please use HarmonyLib.HarmonyMethodExtensions.Clone instead. This will be removed in a future version.", true)]
    public static HarmonyMethod Clone(this HarmonyMethod original) => (HarmonyMethod)HarmonyLib.HarmonyMethodExtensions.Clone(original);
    [Obsolete("Harmony.HarmonyMethodExtensions.Merge is obsolete. Please use HarmonyLib.HarmonyMethodExtensions.Merge instead. This will be removed in a future version.", true)]
    public static HarmonyMethod Merge(this HarmonyMethod master, HarmonyMethod detail) => (HarmonyMethod)HarmonyLib.HarmonyMethodExtensions.Merge(master, detail);
    [Obsolete("Harmony.HarmonyMethodExtensions.GetHarmonyMethods(Type) is obsolete. Please use HarmonyLib.HarmonyMethodExtensions.GetFromType instead. This will be removed in a future version.", true)]
    public static List<HarmonyMethod> GetHarmonyMethods(this Type type) => [.. Array.ConvertAll(HarmonyLib.HarmonyMethodExtensions.GetFromType(type).ToArray(), x => (HarmonyMethod)x)];
    [Obsolete("Harmony.HarmonyMethodExtensions.GetHarmonyMethods(MethodBase) is obsolete. Please use HarmonyLib.HarmonyMethodExtensions.GetFromMethod instead. This will be removed in a future version.", true)]
    public static List<HarmonyMethod> GetHarmonyMethods(this MethodBase method) => [.. Array.ConvertAll(HarmonyLib.HarmonyMethodExtensions.GetFromMethod(method).ToArray(), x => (HarmonyMethod)x)];
}