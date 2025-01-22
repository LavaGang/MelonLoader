﻿using MonoMod.Utils;
using System;
using System.Reflection;

namespace Harmony;

[Obsolete("Harmony.PatchInfoSerialization is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfoSerialization instead. This will be removed in a future version.", true)]
public static class PatchInfoSerialization
{
    private delegate HarmonyLib.PatchInfo HarmonyLib_PatchInfoSerialization_Deserialize_Delegate(byte[] bytes);
    private static readonly HarmonyLib_PatchInfoSerialization_Deserialize_Delegate HarmonyLib_PatchInfoSerialization_Deserialize
        = HarmonyLib.AccessTools.Method("HarmonyLib.PatchInfoSerialization:Deserialize").CreateDelegate<HarmonyLib_PatchInfoSerialization_Deserialize_Delegate>();

    private delegate int HarmonyLib_PatchInfoSerialization_PriorityComparer_Delegate(object obj, int index, int priority);
    private static readonly HarmonyLib_PatchInfoSerialization_PriorityComparer_Delegate HarmonyLib_PatchInfoSerialization_PriorityComparer
        = HarmonyLib.AccessTools.Method("HarmonyLib.PatchInfoSerialization:PriorityComparer").CreateDelegate<HarmonyLib_PatchInfoSerialization_PriorityComparer_Delegate>();

    [Obsolete("Harmony.PatchInfoSerialization.Deserialize is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfoSerialization.Deserialize instead. This will be removed in a future version.", true)]
    public static PatchInfo Deserialize(byte[] bytes) => (PatchInfo)HarmonyLib_PatchInfoSerialization_Deserialize(bytes);
    [Obsolete("Harmony.PatchInfoSerialization.PriorityComparer is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfoSerialization.PriorityComparer instead. This will be removed in a future version.", true)]
    public static int PriorityComparer(object obj, int index, int priority, string[] before, string[] after) => HarmonyLib_PatchInfoSerialization_PriorityComparer(obj, index, priority);
}

[Obsolete("Harmony.PatchInfo is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfo instead. This will be removed in a future version.", true)]
[Serializable]
public class PatchInfo : HarmonyLib.PatchInfo { }

[Obsolete("Harmony.Patch is Only Here for Compatibility Reasons. Please use HarmonyLib.Patch instead. This will be removed in a future version.", true)]
[Serializable]
public class Patch(MethodInfo patch, int index, string owner, int priority, string[] before, string[] after) : IComparable
{
    public readonly MethodInfo patch = patch;
    private readonly HarmonyLib.Patch patchWrapper = new(patch, index, owner, priority, before, after, false);

    public MethodInfo GetMethod(MethodBase original) => patchWrapper.GetMethod(original);
    public override bool Equals(object obj) => patchWrapper.Equals(obj);
    public int CompareTo(object obj) => patchWrapper.CompareTo(obj);
    public override int GetHashCode() => patchWrapper.GetHashCode();
}