#if NET6_0_OR_GREATER

using AsmResolver.PE.DotNet.Metadata;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using MethodInfo = System.Reflection.MethodInfo;

namespace MelonLoader.Fixes;

public static class AsmResolverFix
{
    //adds https://github.com/Washi1337/AsmResolver/pull/609
    public static void Install()
    {
        MelonDebug.Msg("Patching AsmResolver SerializedTableStream GetCodedIndexSize...");
        var methodInfo = AccessTools.Method(typeof(SerializedTableStream).GetNestedType("<>c__DisplayClass18_0", AccessTools.all), "<GetCodedIndexSize>b__1");
        Core.HarmonyInstance.Patch(methodInfo, null, null, new HarmonyMethod(typeof(AsmResolverFix).GetMethod(nameof(GetCodexIndexSizePatch))));
    }

    public static IEnumerable<CodeInstruction> GetCodexIndexSizePatch(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        for (var i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Clt)
            {
                codes[i].opcode = OpCodes.Cgt;
                codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldc_I4_0));
                codes.Insert(i + 2, new CodeInstruction(OpCodes.Ceq));
                break;
            }
        }

        return codes;
    }
}
#endif
