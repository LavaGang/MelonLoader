#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using MelonLoader.InternalUtils;
using MonoMod.Utils;

namespace MelonLoader.Fixes.Il2CppInterop
{
    // Herp: This fixes an XRef issue with Il2CppInterop's GenericMethod::GetMethod Hook on some Unity Builds
    // Unity 2020.3.x
    // Unity 6000.x.x+
    internal static class Il2CppInteropInjectorHelpersSetupFix
    {
        private static Type _injectorType;
        private static MethodInfo _setupMethod;
        private static MethodInfo _setupTranspiler;
        
        internal static void Install()
        {
            try
            {
                Type thisType = typeof(Il2CppInteropInjectorHelpersSetupFix);
                
                _injectorType = typeof(ClassInjector).Assembly.GetType("Il2CppInterop.Runtime.Injection.InjectorHelpers", false);
                if (_injectorType == null)
                    throw new Exception($"Failed to get InjectorHelpers");
                
                _setupMethod = _injectorType.GetMethod("Setup", BindingFlags.NonPublic | BindingFlags.Static);
                if (_setupMethod == null)
                    throw new Exception("Failed to get InjectorHelpers.Setup");

                MelonDebug.Msg($"Patching Il2CppInterop InjectorHelpers.Setup...");
                _setupTranspiler = thisType.GetMethod(nameof(Setup_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
                Core.HarmonyInstance.Patch(_setupMethod,
                    null,
                    null,
                    new HarmonyMethod(_setupTranspiler));
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }
        
        private static bool ShouldUseNewHook()
        {
            if (UnityInformationHandler.EngineVersion.Major >= 6000)
                return true;

            if ((UnityInformationHandler.EngineVersion.Major == 2020)
                && (UnityInformationHandler.EngineVersion.Minor == 3)
                && (UnityInformationHandler.EngineVersion.Build >= 48))
                return true;

            if ((UnityInformationHandler.EngineVersion.Major == 2022)
                && (UnityInformationHandler.EngineVersion.Minor == 3)
                && (UnityInformationHandler.EngineVersion.Build >= 62))
                return true;

            return false;
        }

        private static IEnumerable<CodeInstruction> Setup_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
#if OSX
            // This hook isn't reliable on macOS due to potential inlining by the player's compiler resulting in il2cppinterop
            // thinking it found the right function, but it actually found one that takes a pointer which it would then incorrectly hook
            // resulting in a crash. While it can hinder class injection functionality, it isn't needed for the game to boot,
            // and it didn't prevent UnityExplorer to function
            instructions = RemoveField(
                "GetTypeInfoFromTypeDefinitionIndexHook", _injectorType,
                instructions);
#endif
            
            if (!ShouldUseNewHook())
                return instructions;
            
            instructions = ReplaceField(
                "GenericMethodGetMethodHook", _injectorType, 
                "GenericMethodGetMethodHook_Unity6", _injectorType, 
                instructions);
            
#if LINUX
            instructions = RemoveField(
                "GetFieldDefaultValueHook", _injectorType,
                instructions);
#endif

            return instructions;
        }
        
        private static IEnumerable<CodeInstruction> ReplaceField(
            string targetFieldName, 
            Type targetContainingType,
            string replacementFieldName,
            Type replacementContainingType,
            IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);
            codeMatcher.MatchStartForward([
                new(i => i.LoadsField(AccessTools.Field(targetContainingType, targetFieldName)))
            ]).Operand = AccessTools.Field(replacementContainingType, replacementFieldName);
            return codeMatcher.InstructionEnumeration();
        }
        
        private static IEnumerable<CodeInstruction> RemoveField(
            string targetFieldName,
            Type targetContainingType,
            IEnumerable<CodeInstruction> instructions)
        {
            var field = AccessTools.Field(targetContainingType, targetFieldName);

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                new CodeMatch(i => i.LoadsField(field))
            );

            if (!matcher.IsValid)
                return instructions;

            // Capture labels from the first instruction being removed
            var labels = matcher.Instruction.labels;

            // Move to next instruction AFTER the removed block
            matcher.Advance(2);

            // Reattach labels so branches still work
            matcher.Instruction.labels.AddRange(labels);

            // Go back and remove the original instructions
            matcher.Advance(-2);
            matcher.RemoveInstructions(2);

            return matcher.InstructionEnumeration();
        }
    }
}

#endif