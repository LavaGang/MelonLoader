#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Il2CppInterop.Common.XrefScans;
using Il2CppInterop.Runtime.Injection;

namespace MelonLoader.Fixes.Il2CppInterop
{
    // fixes: https://github.com/BepInEx/Il2CppInterop/pull/90
    internal class Il2CppInteropGetFieldDefaultValueFix
    {
        private static MelonLogger.Instance _logger = new("Il2CppInterop");

        private static MethodInfo _jumpTargets;
        private static MethodInfo _getStaticFieldValue_Fixed;
        private static MethodInfo _findClassGetFieldDefaultValueXref;
        private static MethodInfo _findClassGetFieldDefaultValueXref_Transpiler;

        private static FieldInfo _s_Signatures;

        private static Type _signatureDefinition;
        private static FieldInfo _signatureDefinition_pattern;
        private static FieldInfo _signatureDefinition_mask;
        private static FieldInfo _signatureDefinition_offset;
        private static FieldInfo _signatureDefinition_xref;

        private static FieldInfo _replacementSigArray;
        private static MethodInfo _findTargetMethod;
        private static MethodInfo _findTargetMethod_Transpiler;

        private static Array _replacementSignatures;
        private static List<LemonTuple<string, string, int, bool>> _signaturesToAdd = new List<LemonTuple<string, string, int, bool>>
        {
            // Idle Slayer - Unity 2021.3.23 (x64)
            new(
                "\x40\x53\x48\x83\xEC\x20\x48\x8B\xDA\xE8\xCC\xCC\xCC\xCC\x4C",
                "xxxxxxxxxx????x",
                0,
                false
            )
        };

        private static void LogMsg(string msg)
            => _logger.Msg(msg);
        private static void LogError(Exception ex)
            => _logger.Error(ex);
        private static void LogError(string msg, Exception ex)
            => _logger.Error(msg, ex);
        private static void LogDebugMsg(string msg)
        {
            if (!MelonDebug.IsEnabled())
                return;
            _logger.Msg(msg);
        }

        internal static void Install()
        {
            try
            {
                Type thisType = typeof(Il2CppInteropGetFieldDefaultValueFix);

                Type classInjectorType = typeof(ClassInjector);
                Type xrefType = typeof(XrefScannerLowLevel);

                Type hookType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.Injection.Hooks.Class_GetFieldDefaultValue_Hook");
                if (hookType == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook");

                Type memoryUtilsType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.MemoryUtils");
                if (memoryUtilsType == null)
                    throw new Exception("Failed to get MemoryUtils");

                _signatureDefinition = memoryUtilsType.GetNestedType("SignatureDefinition", BindingFlags.Public | BindingFlags.Instance);
                if (_signatureDefinition == null)
                    throw new Exception("Failed to get MemoryUtils.SignatureDefinition");

                _signatureDefinition_pattern = _signatureDefinition.GetField("pattern", BindingFlags.Public | BindingFlags.Instance);
                if (_signatureDefinition_pattern == null)
                    throw new Exception("Failed to get SignatureDefinition.pattern");

                _signatureDefinition_mask = _signatureDefinition.GetField("mask", BindingFlags.Public | BindingFlags.Instance);
                if (_signatureDefinition_mask == null)
                    throw new Exception("Failed to get SignatureDefinition.mask");

                _signatureDefinition_offset = _signatureDefinition.GetField("offset", BindingFlags.Public | BindingFlags.Instance);
                if (_signatureDefinition_offset == null)
                    throw new Exception("Failed to get SignatureDefinition.offset");

                _signatureDefinition_xref = _signatureDefinition.GetField("xref", BindingFlags.Public | BindingFlags.Instance);
                if (_signatureDefinition_xref == null)
                    throw new Exception("Failed to get SignatureDefinition.xref");

                _findTargetMethod = hookType.GetMethod("FindTargetMethod", BindingFlags.Public | BindingFlags.Instance);
                if (_findTargetMethod == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook.FindTargetMethod");

                _findClassGetFieldDefaultValueXref = hookType.GetMethod("FindClassGetFieldDefaultValueXref", BindingFlags.NonPublic | BindingFlags.Static);
                if (_findClassGetFieldDefaultValueXref == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook.FindClassGetFieldDefaultValueXref");

                _s_Signatures = hookType.GetField("s_Signatures", BindingFlags.NonPublic | BindingFlags.Static);
                if (_s_Signatures == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook.s_Signatures");

                _jumpTargets = xrefType.GetMethod("JumpTargets", BindingFlags.Public | BindingFlags.Static);
                if (_jumpTargets == null)
                    throw new Exception("Failed to get XrefScannerLowLevel.JumpTargets");

                _getStaticFieldValue_Fixed = thisType.GetMethod(nameof(JumpTargets_getStaticFieldValue_Fixed), BindingFlags.NonPublic | BindingFlags.Static);
                _findClassGetFieldDefaultValueXref_Transpiler = thisType.GetMethod(nameof(FindClassGetFieldDefaultValueXref_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                _replacementSigArray = thisType.GetField("_replacementSignatures", BindingFlags.NonPublic | BindingFlags.Static);
                _findTargetMethod_Transpiler = thisType.GetMethod(nameof(FindTargetMethod_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                LogDebugMsg("Getting Signatures...");
                GetSignatures();

                LogDebugMsg("Patching Il2CppInterop Class_GetFieldDefaultValue_Hook.FindClassGetFieldDefaultValueXref...");
                Core.HarmonyInstance.Patch(_findClassGetFieldDefaultValueXref,
                    null,
                    null,
                    new HarmonyMethod(_findClassGetFieldDefaultValueXref_Transpiler));

                LogDebugMsg("Patching Il2CppInterop Class_GetFieldDefaultValue_Hook.FindTargetMethod...");
                Core.HarmonyInstance.Patch(_findTargetMethod,
                    null,
                    null,
                    new HarmonyMethod(_findTargetMethod_Transpiler));
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }
        private static IEnumerable<IntPtr> JumpTargets_getStaticFieldValue_Fixed(IntPtr codeStart, bool ignoreRetn, out bool _ret)
        {
            _ret = false;
            var getStaticFieldValueTargets = XrefScannerLowLevel.JumpTargets(codeStart, ignoreRetn).ToList();

            // Sometimes the compiler can do an optimization and omit 'retn' instruction,
            // which then causes code following to grab wrong function pointer. A correct match should not contain more than 4 jumps
            // This optimization also causes Field::StaticGetValueInternal method to be located right under Field::StaticGetValue method
            if (getStaticFieldValueTargets.Count() > 4)
            {
                _ret = true;
                return [getStaticFieldValueTargets[^2]];
            }

            return getStaticFieldValueTargets;
        }

        private static IEnumerable<CodeInstruction> FindTargetMethod_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            for (int i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction instruction = instructions.ElementAt(i);

                if (!found
                    && instruction.LoadsField(_s_Signatures))
                {
                    found = true;
                    instruction.operand = _replacementSigArray;
                    LogDebugMsg("Patched Il2CppInterop Class_GetFieldDefaultValue_Hook._s_Signatures");
                }

                yield return instruction;
            }
        }

        private static IEnumerable<CodeInstruction> FindClassGetFieldDefaultValueXref_Transpiler(
            MethodBase __originalMethod, 
            IEnumerable<CodeInstruction> instructions, 
            ILGenerator generator)
        {
            int startIndex = 0;
            int endIndex = 0;

            bool found = false;
            for (int i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction instruction = instructions.ElementAt(i);
                if (!found
                    && instruction.ToString().Contains("Field::StaticGetValueInternal:"))
                {
                    endIndex = i - 2;

                    for (int j = i; j > 0; j--)
                    {
                        CodeInstruction newInst = instructions.ElementAt(j);
                        if (newInst.Calls(_jumpTargets))
                        {
                            found = true;
                            startIndex = j;
                            break;
                        }
                    }

                    break;
                }
            }

            Label endIndexLabel = generator.DefineLabel();
            for (int i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction code = instructions.ElementAt(i);
                if (found && (i == startIndex))
                {
                    // Define _ret Local
                    int shouldReturnLocalIndex = generator.DeclareLocal(typeof(bool)).LocalIndex;

                    // JumpTargets_getStaticFieldValue_Fixed
                    yield return new(OpCodes.Ldloca_S, shouldReturnLocalIndex);
                    yield return new(OpCodes.Call, _getStaticFieldValue_Fixed);

                    // .Last()
                    yield return instructions.ElementAt(startIndex + 1);

                    // getStaticFieldValueInternal =
                    yield return instructions.ElementAt(startIndex + 2);

                    // if (_ret)
                    yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)shouldReturnLocalIndex);
                    yield return new CodeInstruction(OpCodes.Brfalse_S, endIndexLabel);

                    // return getStaticFieldValueInternal
                    yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)shouldReturnLocalIndex);
                    yield return new CodeInstruction(OpCodes.Ret);

                    // Skip to End Index of Target
                    i = endIndex;
                    LogDebugMsg("Patched Il2CppInterop XrefScannerLowLevel.JumpTargets(getStaticFieldValue)");
                }
                else
                {
                    // Apply Label to End Index
                    if (found && (i == (endIndex + 1)))
                        code.labels.Add(endIndexLabel);

                    // Add Old Instruction as-is
                    yield return code;
                }
            }
        }

        private static object ConvertTupleToSignature(LemonTuple<string, string, int, bool> tuple)
        {
            object newSig = Activator.CreateInstance(_signatureDefinition);
            _signatureDefinition_pattern.SetValue(newSig, tuple.Item1);
            _signatureDefinition_mask.SetValue(newSig, tuple.Item2);
            _signatureDefinition_offset.SetValue(newSig, tuple.Item3);
            _signatureDefinition_xref.SetValue(newSig, tuple.Item4);
            return newSig;
        }

        private static LemonTuple<string, string, int, bool> ConvertSignatureToTuple(object sig)
            => new(
                (string)_signatureDefinition_pattern.GetValue(sig),
                (string)_signatureDefinition_mask.GetValue(sig),
                (int)_signatureDefinition_offset.GetValue(sig),
                (bool)_signatureDefinition_xref.GetValue(sig)
                );

        private static void GetSignatures()
        {
            // Get Current List from Field
            List<object> replacementSignatures = new();
            Array currentSigs = (Array)_s_Signatures.GetValue(null);
            foreach (var item in currentSigs)
                replacementSignatures.Add(item);

            // Iterate through New Signatures
            foreach (var newSig in _signaturesToAdd)
            {
                // Check if Signature Exists
                bool wasFound = false;
                foreach (var sig in currentSigs)
                {
                    var sigTuple = ConvertSignatureToTuple(sig);
                    if ((sigTuple.Item1 == newSig.Item1)
                        && (sigTuple.Item2 == newSig.Item2)
                        && (sigTuple.Item3 == newSig.Item3)
                        && (sigTuple.Item4 == newSig.Item4))
                    {
                        wasFound = true;
                        break;
                    }
                }
                if (wasFound)
                    continue;

                // Add New Signature
                replacementSignatures.Add(ConvertTupleToSignature(newSig));
            }

            _replacementSignatures = Array.CreateInstance(_s_Signatures.FieldType.GetElementType(), replacementSignatures.Count);
            for (int i = 0; i < replacementSignatures.Count; i++)
                _replacementSignatures.SetValue(replacementSignatures[i], i);
        }
    }
}
#endif