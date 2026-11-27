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
    // Implements: https://github.com/BepInEx/Il2CppInterop/pull/296
    internal class Il2CppInteropTypeDefinitionIndexFix
    {
        private static MethodInfo _findTargetMethod;
        private static MethodInfo _findTargetMethod_Postfix;

        internal static void Install()
        {
            try
            {
                Type thisType = typeof(Il2CppInteropTypeDefinitionIndexFix);
                Type classInjectorType = typeof(ClassInjector);

                Type hookType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.Injection.Hooks.MetadataCache_GetTypeInfoFromTypeDefinitionIndex_Hook");
                if (hookType == null)
                    throw new Exception("Failed to get MetadataCache_GetTypeInfoFromTypeDefinitionIndex_Hook");
                
                _findTargetMethod =
                    hookType.GetMethod("FindTargetMethod", BindingFlags.Public | BindingFlags.Instance);
                if (_findTargetMethod == null)
                    throw new Exception("Failed to get MetadataCache_GetTypeInfoFromTypeDefinitionIndex_Hook.FindTargetMethod");
            
                _findTargetMethod_Postfix = thisType.GetMethod(nameof(FindTargetMethod_Postfix),
                    BindingFlags.NonPublic | BindingFlags.Static);
                    
                MelonDebug.Msg("Patching Il2CppInterop MetadataCache_GetTypeInfoFromTypeDefinitionIndex_Hook.FindTargetMethod...");
                Core.HarmonyInstance.Patch(_findTargetMethod,
                    null,
                    new HarmonyMethod(_findTargetMethod_Postfix));
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }

        private static void FindTargetMethod_Postfix(ref nint __result)
        {
            while (__result != IntPtr.Zero)
            {
                var targets = XrefScannerLowLevel.JumpTargets(__result).ToArray();
                if (targets.Length < 2 || targets.Distinct().Count() != 1) break;
                __result = targets[0];
            }
        }
    }
}

#endif
