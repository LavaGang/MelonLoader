#if !WINDOWS && !NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace MelonLoader.Fixes.NetFramework;

internal static class XTermFix
{
    private static int _intOffset;

    internal static void Install()
    {
        if (typeof(Console).Assembly.GetType("System.ConsoleDriver") == null)
            // Mono version is too old, this fix doesn't apply
            return;

        if (AccessTools.Method("System.TermInfoReader:DetermineVersion") != null)
            // Fix has been applied officially
            return;

        Core.HarmonyInstance.Patch(AccessTools.Method("System.TermInfoReader:ReadHeader"),
            new HarmonyMethod(typeof(XTermFix), nameof(ReadHeaderPrefix)));

        Core.HarmonyInstance.Patch(AccessTools.Method("System.TermInfoReader:Get", [AccessTools.TypeByName("System.TermInfoNumbers")
            ]), transpiler: new HarmonyMethod(typeof(XTermFix), nameof(GetTermInfoNumbersTranspiler)));

        Core.HarmonyInstance.Patch(AccessTools.Method("System.TermInfoReader:Get", [AccessTools.TypeByName("System.TermInfoStrings")
            ]), transpiler: new HarmonyMethod(typeof(XTermFix), nameof(GetTermInfoStringsTranspiler)));

        Core.HarmonyInstance.Patch(AccessTools.Method("System.TermInfoReader:GetStringBytes", [AccessTools.TypeByName("System.TermInfoStrings")
            ]), transpiler: new HarmonyMethod(typeof(XTermFix), nameof(GetTermInfoStringsTranspiler)));
    }

    private static int GetInt32(byte[] buffer, int offset)
    {
        int b1 = buffer[offset];
        int b2 = buffer[offset + 1];
        int b3 = buffer[offset + 2];
        int b4 = buffer[offset + 3];

        return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
    }

    private static short GetInt16(byte[] buffer, int offset)
    {
        int b1 = buffer[offset];
        int b2 = buffer[offset + 1];

        return (short) (b1 | (b2 << 8));
    }

    public static int GetInteger(byte[] buffer, int offset) =>
        _intOffset == 2
            ? GetInt16(buffer, offset)
            : GetInt32(buffer, offset);

    private static void DetermineVersion(short magic)
    {
        _intOffset = magic switch
        {
            0x11a => 2,
            0x21e => 4,
            _ => throw new Exception($"Unknown xterm header format: {magic}")
        };
    }

    public static bool ReadHeaderPrefix(byte[] buffer,
                                        ref int position,
                                        ref short ___boolSize,
                                        ref short ___numSize,
                                        ref short ___strOffsets)
    {
        MelonDebug.Msg("Reading header");
        var magic = GetInt16(buffer, position);
        position += 2;
        DetermineVersion(magic);

        // nameSize = GetInt16(buffer, position);
        position += 2;
        ___boolSize = GetInt16(buffer, position);
        position += 2;
        ___numSize = GetInt16(buffer, position);
        position += 2;
        ___strOffsets = GetInt16(buffer, position);
        position += 2;
        // strSize = GetInt16(buffer, position);
        position += 2;

        return false;
    }

    public static IEnumerable<CodeInstruction> GetTermInfoNumbersTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        // This implementation does not seem to have changed

        var list = instructions.ToList();

        list[31] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(XTermFix), nameof(_intOffset)));
        list[36] = new CodeInstruction(OpCodes.Nop);
        list[39] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(XTermFix), nameof(GetInteger)));

        return list;
    }

    public static IEnumerable<CodeInstruction> GetTermInfoStringsTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        // This implementation does not seem to have changed

        var list = instructions.ToList();

        list[32] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(XTermFix), nameof(_intOffset)));

        return list;
    }
}
#endif