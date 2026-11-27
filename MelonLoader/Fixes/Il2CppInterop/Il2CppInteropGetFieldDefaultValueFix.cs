#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Iced.Intel;
using Il2CppInterop.Common.XrefScans;
using Il2CppInterop.Runtime.Injection;
using FlowControl = Iced.Intel.FlowControl;

namespace MelonLoader.Fixes.Il2CppInterop
{
    // Herp: This allows custom signatures to be added to Il2CppInterop's Class::GetFieldDefaultValue Hook
    // Also implements: https://github.com/BepInEx/Il2CppInterop/pull/290
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
        
        private static MethodInfo _findClassGetFieldDefaultValueXref;
        private static MethodInfo _findClassGetFieldDefaultValueXref_Transpiler;
        
        private static MethodInfo _jumpTargets;
        private static MethodInfo _decoderForAddress;
        private static MethodInfo _getStaticFieldValueInternal_Fixed;

        private static Array _replacementSignatures;
        private static List<LemonTuple<string, string, int, bool>> _signaturesToAdd = new()
        {
            // Unity 6000.3.x
            // https://github.com/BepInEx/Il2CppInterop/issues/284
            new(
                "\x48\x89\x5C\x24\x08\x48\x89\x74\x24\x10\x57\x48\x81\xEC\xCC\xCC\xCC\xCC\x48\x8B\x79\x10\x48\x8B\xD9\x48\x8B\xF2\x48\x2B\x9F", 
                "xxxxxxxxxxxxxx????xxxxxxxxxxxxx", 
                0,
                false),
            
            // https://github.com/BepInEx/Il2CppInterop/pull/291
            new(
                "\x48\x89\x5C\x24\x08\x57\x48\x81\xEC\x00\x00\x00\x00\x48\x8B\x41\x10\x48\x8B\xD9\x48\x8B\xFA\x48\x2B\x98", 
                "xxxxxxxxx????xxxxxxxxxxxxx", 
                0,
                false),
        };

        internal static void Install()
        {
            try
            {
                Type thisType = typeof(Il2CppInteropGetFieldDefaultValueFix);
                Type classInjectorType = typeof(ClassInjector);
                Type xrefScannerType = typeof(XrefScanner);
                Type xrefScannerLowLevelType = typeof(XrefScannerLowLevel);

                Type hookType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.Injection.Hooks.Class_GetFieldDefaultValue_Hook");
                if (hookType == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook");
                
                _findClassGetFieldDefaultValueXref =
                    hookType.GetMethod("FindClassGetFieldDefaultValueXref", BindingFlags.NonPublic | BindingFlags.Static);
                if (_findClassGetFieldDefaultValueXref == null)
                    throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook.FindClassGetFieldDefaultValueXref");
                
                _decoderForAddress =
                    xrefScannerType.GetMethod("DecoderForAddress", BindingFlags.NonPublic | BindingFlags.Static, 
                    [typeof(IntPtr), typeof(int)]);
                if (_decoderForAddress == null)
                    throw new Exception("Failed to get XrefScanner.DecoderForAddress");
                
                _jumpTargets = xrefScannerLowLevelType.GetMethod("JumpTargets", BindingFlags.Public | BindingFlags.Static);
                if (_jumpTargets == null)
                    throw new Exception("Failed to get XrefScannerLowLevel.JumpTargets");

                _getStaticFieldValueInternal_Fixed = thisType.GetMethod(nameof(JumpTargets_getStaticFieldValueInternal_Fixed), BindingFlags.NonPublic | BindingFlags.Static);
                _findClassGetFieldDefaultValueXref_Transpiler = thisType.GetMethod(nameof(FindClassGetFieldDefaultValueXref_Transpiler),
                    BindingFlags.NonPublic | BindingFlags.Static);

                if (_signaturesToAdd.Count > 0)
                {
                    Type memoryUtilsType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.MemoryUtils");
                    if (memoryUtilsType == null)
                        throw new Exception("Failed to get MemoryUtils");

                    _findTargetMethod =
                        hookType.GetMethod("FindTargetMethod", BindingFlags.Public | BindingFlags.Instance);
                    if (_findTargetMethod == null)
                        throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook.FindTargetMethod");

                    _s_Signatures = hookType.GetField("s_Signatures", BindingFlags.NonPublic | BindingFlags.Static);
                    if (_s_Signatures == null)
                        throw new Exception("Failed to get Class_GetFieldDefaultValue_Hook.s_Signatures");

                    _signatureDefinition = memoryUtilsType.GetNestedType("SignatureDefinition",
                        BindingFlags.Public | BindingFlags.Instance);
                    if (_signatureDefinition == null)
                        throw new Exception("Failed to get MemoryUtils.SignatureDefinition");

                    _signatureDefinition_pattern =
                        _signatureDefinition.GetField("pattern", BindingFlags.Public | BindingFlags.Instance);
                    if (_signatureDefinition_pattern == null)
                        throw new Exception("Failed to get SignatureDefinition.pattern");

                    _signatureDefinition_mask =
                        _signatureDefinition.GetField("mask", BindingFlags.Public | BindingFlags.Instance);
                    if (_signatureDefinition_mask == null)
                        throw new Exception("Failed to get SignatureDefinition.mask");

                    _signatureDefinition_offset =
                        _signatureDefinition.GetField("offset", BindingFlags.Public | BindingFlags.Instance);
                    if (_signatureDefinition_offset == null)
                        throw new Exception("Failed to get SignatureDefinition.offset");

                    _signatureDefinition_xref =
                        _signatureDefinition.GetField("xref", BindingFlags.Public | BindingFlags.Instance);
                    if (_signatureDefinition_xref == null)
                        throw new Exception("Failed to get SignatureDefinition.xref");

                    _replacementSigArray = thisType.GetField("_replacementSignatures",
                        BindingFlags.NonPublic | BindingFlags.Static);
                    _findTargetMethod_Transpiler = thisType.GetMethod(nameof(FindTargetMethod_Transpiler),
                        BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (_signaturesToAdd.Count > 0)
                {
                    MelonDebug.Msg("Getting Il2CppInterop Class_GetFieldDefaultValue_Hook Signatures...");
                    GetSignatures();

                    MelonDebug.Msg("Patching Il2CppInterop Class_GetFieldDefaultValue_Hook.FindTargetMethod...");
                    Core.HarmonyInstance.Patch(_findTargetMethod,
                        null,
                        null,
                        new HarmonyMethod(_findTargetMethod_Transpiler));
                }
                
                MelonDebug.Msg("Patching Il2CppInterop Class_GetFieldDefaultValue_Hook.FindClassGetFieldDefaultValueXref...");
                Core.HarmonyInstance.Patch(_findClassGetFieldDefaultValueXref,
                    null,
                    null,
                    new HarmonyMethod(_findClassGetFieldDefaultValueXref_Transpiler));
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }

        private static IEnumerable<CodeInstruction> FindTargetMethod_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            for (int i = 0; i < codeInstructions.Count(); i++)
            {
                CodeInstruction instruction = codeInstructions.ElementAt(i);

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
        
        private static IEnumerable<CodeInstruction> FindClassGetFieldDefaultValueXref_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionsList = instructions.ToList();
            
            CodeInstruction jmpTargetsInst = instructionsList.LastOrDefault(x =>
                x.Calls(_jumpTargets));
            if (jmpTargetsInst != null)
            {
                jmpTargetsInst.opcode = OpCodes.Call;
                jmpTargetsInst.operand = _getStaticFieldValueInternal_Fixed;
                MelonDebug.Msg("Patched Il2CppInterop Class_GetFieldDefaultValue_Hook XrefScannerLowLevel.JumpTargets(getStaticFieldValueInternal)");
            }
            return instructionsList;
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
                (int)_signatureDefinition_offset.GetValue(sig)!,
                (bool)_signatureDefinition_xref.GetValue(sig)!
                );

        private static void GetSignatures()
        {
            // Get Current List from Field
            List<object> replacementSignatures = new();
            Array currentSigs = (Array)_s_Signatures.GetValue(null);
            foreach (var item in currentSigs!)
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

            _replacementSignatures = Array.CreateInstance(_s_Signatures.FieldType.GetElementType()!, replacementSignatures.Count);
            for (int i = 0; i < replacementSignatures.Count; i++)
                _replacementSignatures.SetValue(replacementSignatures[i], i);
        }

        // Clang lays the literal-field branch out as Class::GetDefaultFieldValue(field, &type) followed by
        // BlobReader::GetConstantValueFromBlob, so the position-based choice picks the wrong call.
        // The call that receives the address of a stack slot (the 'type' out parameter) is the one we want.
        private static IEnumerable<IntPtr> JumpTargets_getStaticFieldValueInternal_Fixed(IntPtr codeStart, bool ignoreRetn = false)
        {
            var origRet = (IEnumerable<IntPtr>)_jumpTargets.Invoke(null, [ codeStart, ignoreRetn ]);

            if (OperatingSystem.IsWindows())
                return origRet;
            
            nint callAddr = FindCallWithStackAddressInSecondArgument(codeStart);
            if (callAddr == 0)
                return origRet;
            
            return [ callAddr ];
        }
        
        /// <summary>
        /// The target of the only direct call in <paramref name="function"/> (up to its first return) whose second
        /// System V argument (rsi) was just set to the address of a stack slot (lea or mov from rsp/rbp); 0 if there is none or more than one.
        /// </summary>
        private static nint FindCallWithStackAddressInSecondArgument(nint function)
        {
            Decoder decoder = (Decoder)_decoderForAddress.Invoke(null, [function, 1000]);
            nint found = 0;
            var secondArgumentIsStackAddress = false;
            while (true)
            {
                decoder.Decode(out var instruction);
                if (decoder.LastError == DecoderError.NoMoreBytes || instruction.Mnemonic == Mnemonic.Int3 ||
                    instruction.FlowControl == FlowControl.Return)
                    break;

                if (instruction.FlowControl == FlowControl.Call)
                {
                    if (secondArgumentIsStackAddress && instruction.Op0Kind is OpKind.NearBranch32 or OpKind.NearBranch64)
                    {
                        if (found != 0)
                            return 0;
                        found = (nint)instruction.NearBranchTarget;
                    }
                    secondArgumentIsStackAddress = false;
                }
                else if (instruction is { Mnemonic: Mnemonic.Lea, Op0Register: Register.RSI })
                {
                    secondArgumentIsStackAddress = instruction.MemoryBase is Register.RSP or Register.RBP;
                }
                else if (instruction is { Mnemonic: Mnemonic.Mov, Op0Register: Register.RSI, Op1Kind: OpKind.Register, Op1Register: Register.RSP or Register.RBP })
                {
                    // Some clang builds (seen on Linux, Unity 6000.0) pass the address of the slot at [rsp] as "mov rsi, rsp".
                    secondArgumentIsStackAddress = true;
                }
                else if (instruction.FlowControl != FlowControl.Next ||
                         (instruction.Op0Kind == OpKind.Register && instruction.Op0Register.GetFullRegister() == Register.RSI))
                {
                    secondArgumentIsStackAddress = false;
                }
            }
            return found;
        }
    }
}

#endif
