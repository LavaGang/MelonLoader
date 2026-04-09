/*
#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;

namespace MelonLoader.Fixes.Il2CppInterop
{
    internal static class Il2CppInteropIl2CppObjectBaseFix
    {
        private static Type _baseType;
        
        private static FieldInfo _isWrappedField;
        private static FieldInfo _pooledPtrField;
        
        private static MethodInfo _finalizeMethod;
        private static MethodInfo _finalizePrefix;
        
        private static MethodInfo _wasCollectedMethod;
        private static MethodInfo _wasCollectedPrefix;
        
        private static MethodInfo _pointerMethod;
        private static MethodInfo _pointerTranspiler;
        
        private static MethodInfo _createGCHandleMethod;
        private static MethodInfo _createGCHandleTranspiler;
        
        internal static void Install()
        {
            try
            {
                Type thisType = typeof(Il2CppInteropIl2CppObjectBaseFix);
                _baseType = typeof(Il2CppObjectBase);

                _isWrappedField = _baseType.GetField("isWrapped", BindingFlags.NonPublic | BindingFlags.Instance);
                _pooledPtrField = _baseType.GetField("pooledPtr", BindingFlags.NonPublic | BindingFlags.Instance);

                MelonDebug.Msg($"Patching Il2CppInterop Il2CppObjectBase.Finalize...");
                _finalizeMethod = _baseType.GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance);
                _finalizePrefix = thisType.GetMethod(nameof(Finalize_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                Core.HarmonyInstance.Patch(_finalizeMethod, new HarmonyMethod(_finalizePrefix));
                
                MelonDebug.Msg($"Patching Il2CppInterop Il2CppObjectBase.get_WasCollected...");
                _wasCollectedMethod = _baseType.GetProperty("WasCollected", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
                _wasCollectedPrefix = thisType.GetMethod(nameof(WasCollected_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                Core.HarmonyInstance.Patch(_wasCollectedMethod, new HarmonyMethod(_wasCollectedPrefix));
                
                // Need to use Transpiler for get_Pointer and CreateGCHandle
                // Prefix/Postfix was causing Crashes
                
                MelonDebug.Msg($"Patching Il2CppInterop Il2CppObjectBase.get_Pointer...");
                _pointerMethod = _baseType.GetProperty("Pointer", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
                _pointerTranspiler = thisType.GetMethod(nameof(Pointer_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
                Core.HarmonyInstance.Patch(_pointerMethod, null, null, new HarmonyMethod(_pointerTranspiler));
                
                MelonDebug.Msg($"Patching Il2CppInterop Il2CppObjectBase.CreateGCHandle...");
                _createGCHandleMethod = _baseType.GetMethod("CreateGCHandle", BindingFlags.NonPublic | BindingFlags.Instance);
                _createGCHandleTranspiler = thisType.GetMethod(nameof(CreateGCHandle_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
                Core.HarmonyInstance.Patch(_createGCHandleMethod, null, null, new HarmonyMethod(_createGCHandleTranspiler));
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }
        
        private static bool IsWrapped(object obj)
            => (bool)_isWrappedField.GetValue(obj);
        private static IntPtr GetPooledPointer(object obj)
            => (IntPtr)_pooledPtrField.GetValue(obj);
        
        private static bool Finalize_Prefix(object __instance)
        {
            if (__instance == null)
                return true;
            return !IsWrapped(__instance);
        }
        
        private static bool WasCollected_Prefix(object __instance, ref bool __result)
        {
            if ((__instance == null)
                || !IsWrapped(__instance))
                return true;
            __result = GetPooledPointer(__instance) == IntPtr.Zero;
            return false;
        }

        private static IEnumerable<CodeInstruction> Pointer_Transpiler(IEnumerable<CodeInstruction> instructions)
            => WrappedCheckTranspiler(false, instructions);
        private static IEnumerable<CodeInstruction> CreateGCHandle_Transpiler(IEnumerable<CodeInstruction> instructions)
            => WrappedCheckTranspiler(true, instructions);
        
        private static IEnumerable<CodeInstruction> WrappedCheckTranspiler(bool isSetter, IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = new List<CodeInstruction>(instructions);
            var returnLabel = new Label();
            if (newInstructions.Count > 0)
            {
                CodeInstruction firstInstruction = newInstructions[0];
                returnLabel = firstInstruction.labels.Count > 0
                    ? firstInstruction.labels[0]
                    : new Label();
                firstInstruction.labels.Add(returnLabel);
            }

            var wrappedCheck = new List<CodeInstruction>
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, _isWrappedField),
                new(OpCodes.Brfalse_S, returnLabel),
            };
            if (isSetter)
            {
                wrappedCheck.Add(new(OpCodes.Ldarg_0));
                wrappedCheck.Add(new(OpCodes.Ldarg_1));
                wrappedCheck.Add(new(OpCodes.Stfld, _pooledPtrField));
            }
            else
            {
                wrappedCheck.Add(new(OpCodes.Ldarg_0));
                wrappedCheck.Add(new(OpCodes.Ldfld, _pooledPtrField));
            }
            wrappedCheck.Add(new(OpCodes.Ret));
            
            newInstructions.InsertRange(0, wrappedCheck);
            return newInstructions;
        }
    }
}

#endif
*/