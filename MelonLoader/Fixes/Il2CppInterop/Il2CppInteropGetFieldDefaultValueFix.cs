#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace MelonLoader.Fixes.Il2CppInterop
{
    // Herp: This just allows custom signatures to be added to Il2CppInterop's Class::GetFieldDefaultValue Hook
    internal class Il2CppInteropGetFieldDefaultValueFix
    {
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
            ("\x48\x89\x5C\x24\x08\x48\x89\x74\x24\x10\x57\x48\x81\xEC\xCC\xCC\xCC\xCC\x48\x8B\x79\x10\x48\x8B\xD9\x48\x8B\xF2\x48\x2B\x9F",
             "xxxxxxxxxxxxxx????xxxxxxxxxxxxx",
             0, false),
        };

        internal static void Install()
        {
            if (_signaturesToAdd.Count <= 0)
                return;

            try
            {
                Type thisType = typeof(Il2CppInteropGetFieldDefaultValueFix);
                Type classInjectorType = typeof(ClassInjector);

                Type hookType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.Injection.Hooks.Class_GetFieldDefaultValue_Hook");
                if (hookType == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook");

                Type memoryUtilsType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.MemoryUtils");
                if (memoryUtilsType == null)
                    throw new Exception("Failed to get MemoryUtils");

                _findTargetMethod = hookType.GetMethod("FindTargetMethod", BindingFlags.Public | BindingFlags.Instance);
                if (_findTargetMethod == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook.FindTargetMethod");

                _s_Signatures = hookType.GetField("s_Signatures", BindingFlags.NonPublic | BindingFlags.Static);
                if (_s_Signatures == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook.s_Signatures");

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

                _replacementSigArray = thisType.GetField("_replacementSignatures", BindingFlags.NonPublic | BindingFlags.Static);
                _findTargetMethod_Transpiler = thisType.GetMethod(nameof(FindTargetMethod_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                MelonDebug.Msg("Getting Il2CppInterop Class_GetFieldDefaultValue_Hook Signatures...");
                GetSignatures();

                MelonDebug.Msg("Patching Il2CppInterop Class_GetFieldDefaultValue_Hook.FindTargetMethod...");
                Core.HarmonyInstance.Patch(_findTargetMethod,
                    null,
                    null,
                    new HarmonyMethod(_findTargetMethod_Transpiler));
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
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
                    MelonDebug.Msg("Patched Il2CppInterop Class_GetFieldDefaultValue_Hook._s_Signatures");
                }

                yield return instruction;
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
