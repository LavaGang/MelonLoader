using System;
using System.Reflection;
using System.Reflection.Emit;
#pragma warning disable 0618

namespace Harmony;

[Obsolete("Harmony.HarmonyInstance is obsolete. Please use HarmonyLib.Harmony instead. This will be removed in a future version.", true)]
public class HarmonyInstance(string id) : HarmonyLib.Harmony(id)
{
    public static HarmonyInstance Create(string id)
    {
        return id == null ? throw new Exception("id cannot be null") : new HarmonyInstance(id);
    }

    public DynamicMethod Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
    {
        base.Patch(original, prefix, postfix, transpiler);
        return null;
    }

    public void Unpatch(MethodBase original, HarmonyPatchType type, string harmonyID = null) => Unpatch(original, (HarmonyLib.HarmonyPatchType)type, harmonyID);
}
