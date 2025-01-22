using HarmonyLib;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MelonLoader.Fixes;

// fixes: https://github.com/MonoMod/MonoMod/pull/102
// based-on: https://github.com/Hamunii/DetourContext.Dispose_Fix/blob/main/DetourContext_Dispose_Fix.cs
internal static class DetourContextDisposeFix
{
    private static MethodInfo _dispose;
    private static MethodInfo _disposeTranspiler;
    private static FieldInfo _isDisposed;

    internal static void Install()
    {
        var detourContextType = typeof(DetourContext);
        var thisType = typeof(DetourContextDisposeFix);

        _isDisposed = detourContextType.GetField("IsDisposed", BindingFlags.NonPublic | BindingFlags.Instance);
        _dispose = detourContextType.GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance);
        _disposeTranspiler = thisType.GetMethod(nameof(DetourContext_Dispose_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

        MelonDebug.Msg("Patching MonoMod DetourContext.Dispose...");
        Core.HarmonyInstance.Patch(_dispose,
            null,
            null,
            new HarmonyMethod(_disposeTranspiler));
    }

    private static IEnumerable<CodeInstruction> DetourContext_Dispose_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;
        CodeInstruction lastInstruction = null;
        foreach (var instruction in instructions)
        {
            if (!found
                && (lastInstruction != null)
                && lastInstruction.LoadsField(_isDisposed)
                && (instruction.opcode == OpCodes.Brtrue))
            {
                found = true;
                instruction.opcode = OpCodes.Brfalse_S;
                MelonDebug.Msg("Patched MonoMod DetourContext.Dispose -> IsDisposed");
            }

            yield return instruction;
            lastInstruction = instruction;
        }
    }
}
