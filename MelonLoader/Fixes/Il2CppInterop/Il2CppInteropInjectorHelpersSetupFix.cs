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
            if (ShouldUseNewHook())
                instructions = ReplaceField(
                    "GenericMethodGetMethodHook", _injectorType, 
                    "GenericMethodGetMethodHook_Unity6", _injectorType, 
                    instructions);

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
    }
}

#endif
