#if NET6_0
using System;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using HarmonyLib;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.Type;
using Il2CppInterop.Runtime.Runtime;
using System.Collections.Generic;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.MethodInfo;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.Class;
using System.Runtime.InteropServices;
using System.Linq;
using System.Reflection.Emit;
using Il2CppInterop.Generator.Extensions;

namespace MelonLoader.Fixes
{
    // fixes: https://github.com/BepInEx/Il2CppInterop/pull/103
    // fixes: https://github.com/BepInEx/Il2CppInterop/pull/134
    // fixes: https://github.com/BepInEx/Il2CppInterop/issues/135
    // reverts: https://github.com/BepInEx/Il2CppInterop/commit/18e58ef5db42a71d6012ab0387b107a4132101eb
    internal unsafe static class Il2CppInteropFixes
    {
        private static Dictionary<IntPtr, Type> _typeLookup = new();

        private static MethodInfo _getType;
        private static MethodInfo _fixedFindType;
        private static MethodInfo _fixedAddTypeToLookup;
        private static MethodInfo _rewriteType;
        private static MethodInfo _systemTypeFromIl2CppType;
        private static MethodInfo _systemTypeFromIl2CppType_Prefix;
        private static MethodInfo _systemTypeFromIl2CppType_Transpiler;
        private static MethodInfo _injectorHelpers_AddTypeToLookup;
        private static MethodInfo _registerTypeInIl2Cpp;
        private static MethodInfo _registerTypeInIl2Cpp_Transpiler;
        private static MethodInfo _isTypeSupported;
        private static MethodInfo _isTypeSupported_Prefix;
        private static MethodInfo _convertMethodInfo;
        private static MethodInfo _convertMethodInfo_Transpiler;
        private static MethodInfo _getIl2CppTypeFullName;
        private static MethodInfo _fixedIsByRef;
        private static MethodInfo _get_IsByRef;
        private static MethodInfo _fixedFindAbstractMethods;
        private static MethodInfo _emitObjectToPointer;
        private static MethodInfo _emitObjectToPointer_Prefix;

        internal static void Install()
        {
            try
            {
                Type typeType = typeof(Type);
                Type thisType = typeof(Il2CppInteropFixes);
                Type classInjectorType = typeof(ClassInjector);
                Type ilGeneratorEx = typeof(ILGeneratorEx);

                _getType = typeType.GetMethod("GetType", BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
                _get_IsByRef = typeType.GetProperty("IsByRef", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
                
                _fixedFindType = thisType.GetMethod(nameof(FixedFindType), BindingFlags.NonPublic | BindingFlags.Static);
                _fixedAddTypeToLookup = thisType.GetMethod(nameof(FixedAddTypeToLookup), BindingFlags.NonPublic | BindingFlags.Static);
                _fixedIsByRef = thisType.GetMethod(nameof(FixedIsByRef), BindingFlags.NonPublic | BindingFlags.Static);
                _fixedFindAbstractMethods = thisType.GetMethod(nameof(FixedFindAbstractMethods), BindingFlags.NonPublic | BindingFlags.Static);

                _systemTypeFromIl2CppType = classInjectorType.GetMethod("SystemTypeFromIl2CppType", BindingFlags.NonPublic | BindingFlags.Static);
                _systemTypeFromIl2CppType_Prefix = thisType.GetMethod(nameof(SystemTypeFromIl2CppType_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                _systemTypeFromIl2CppType_Transpiler = thisType.GetMethod(nameof(SystemTypeFromIl2CppType_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
                
                _getIl2CppTypeFullName = classInjectorType.GetMethod("GetIl2CppTypeFullName", BindingFlags.NonPublic | BindingFlags.Static);
                _rewriteType = classInjectorType.GetMethod("RewriteType", BindingFlags.NonPublic | BindingFlags.Static);
                
                _registerTypeInIl2Cpp = classInjectorType.GetMethod("RegisterTypeInIl2Cpp", BindingFlags.Public | BindingFlags.Static, [typeof(Type), typeof(RegisterTypeOptions)]);
                _registerTypeInIl2Cpp_Transpiler = thisType.GetMethod(nameof(RegisterTypeInIl2Cpp_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                _isTypeSupported = classInjectorType.GetMethod("IsTypeSupported", BindingFlags.NonPublic | BindingFlags.Static);
                _isTypeSupported_Prefix = thisType.GetMethod(nameof(IsTypeSupported_Prefix), BindingFlags.NonPublic | BindingFlags.Static);

                _convertMethodInfo = classInjectorType.GetMethod("ConvertMethodInfo", BindingFlags.NonPublic | BindingFlags.Static);
                _convertMethodInfo_Transpiler = thisType.GetMethod(nameof(ConvertMethodInfo_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);

                _emitObjectToPointer = ilGeneratorEx.GetMethod("EmitObjectToPointer", BindingFlags.Public | BindingFlags.Static);
                _emitObjectToPointer_Prefix = thisType.GetMethod(nameof(EmitObjectToPointer_Prefix), BindingFlags.NonPublic | BindingFlags.Static);

                _injectorHelpers_AddTypeToLookup = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.Injection.InjectorHelpers")
                    .GetMethod("AddTypeToLookup", BindingFlags.NonPublic | BindingFlags.Static, [typeof(Type), typeof(IntPtr)]);

                MelonDebug.Msg("Patching ClassInjector.SystemTypeFromIl2CppType...");
                Core.HarmonyInstance.Patch(_systemTypeFromIl2CppType,
                    new HarmonyMethod(_systemTypeFromIl2CppType_Prefix), 
                    null,
                    new HarmonyMethod(_systemTypeFromIl2CppType_Transpiler));

                MelonDebug.Msg("Patching ClassInjector.RegisterTypeInIl2Cpp...");
                Core.HarmonyInstance.Patch(_registerTypeInIl2Cpp,
                    null,
                    null,
                    new HarmonyMethod(_registerTypeInIl2Cpp_Transpiler));

                MelonDebug.Msg("Patching ClassInjector.IsTypeSupported...");
                Core.HarmonyInstance.Patch(_isTypeSupported,
                    new HarmonyMethod(_isTypeSupported_Prefix));

                MelonDebug.Msg("Patching ClassInjector.ConvertMethodInfo...");
                Core.HarmonyInstance.Patch(_convertMethodInfo,
                    null,
                    null,
                    new HarmonyMethod(_convertMethodInfo_Transpiler));

                MelonDebug.Msg("Patching ILGeneratorEx.EmitObjectToPointer...");
                Core.HarmonyInstance.Patch(_emitObjectToPointer,
                    new HarmonyMethod(_emitObjectToPointer_Prefix));
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }

        private static bool FixedIsByRef(Type type)
            => type.IsByRef || type.IsPointer;

        private static Type FixedFindType(string il2CppTypeFullName)
        {
            Type returnType = Type.GetType(il2CppTypeFullName);
            if (returnType == null)
                returnType = Type.GetType($"Il2Cpp{il2CppTypeFullName}");
            return returnType;
        }

        private static void FixedAddTypeToLookup(Type type, IntPtr typePointer)
        {
            _injectorHelpers_AddTypeToLookup.Invoke(null, [type, typePointer]);

            typePointer = IL2CPP.il2cpp_class_get_type(typePointer);
            if (typePointer !=  IntPtr.Zero)
                _typeLookup.Add(typePointer, type);
        }

        private static bool EmitObjectToPointer_Prefix(bool __7, ref bool __8)
        {
            __8 = __7;
            return true;
        }

        private static bool SystemTypeFromIl2CppType_Prefix(Il2CppTypeStruct* __0, ref Type __result)
        {
            INativeTypeStruct wrappedType = UnityVersionHandler.Wrap(__0);
            if (_typeLookup.TryGetValue((IntPtr)wrappedType.TypePointer, out Type type))
            {
                __result = (Type)_rewriteType.Invoke(null, [type]);
                return false;
            }
            return true;
        }

        private static IEnumerable<CodeInstruction> SystemTypeFromIl2CppType_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!found 
                    && instruction.Calls(_getType))
                {
                    found = true;
                    instruction.operand = _fixedFindType;
                    MelonDebug.Msg("Patched ClassInjector.SystemTypeFromIl2CppType -> Type.GetType");
                }
                yield return instruction;
            }
        }

        private static IEnumerable<CodeInstruction> RegisterTypeInIl2Cpp_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            bool found2 = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!found 
                    && instruction.Calls(_injectorHelpers_AddTypeToLookup))
                {
                    found = true;
                    instruction.operand = _fixedAddTypeToLookup;
                    MelonDebug.Msg("Patched ClassInjector.RegisterTypeInIl2Cpp -> InjectorHelpers.AddTypeToLookup");
                }

                if (!found2
                    && instruction.ToString()
                    .Contains("FindAbstractMethods"))
                {
                    found2 = true;
                    instruction.operand = _fixedFindAbstractMethods;
                    MelonDebug.Msg("Patched ClassInjector.RegisterTypeInIl2Cpp -> FindAbstractMethods");
                }

                yield return instruction;
            }
        }

        private static bool IsTypeSupported_Prefix(Type __0, ref bool __result)
        {
            if (__0.IsValueType ||
                __0 == typeof(string) ||
                __0.IsGenericParameter)
            {
                __result = true;
                return false;
            }

            if (__0.IsByRef 
                || __0.IsPointer) 
                return IsTypeSupported_Prefix(__0.GetElementType(), ref __result);

            __result = typeof(Il2CppObjectBase).IsAssignableFrom(__0);
            return false;
        }

        private static IEnumerable<CodeInstruction> ConvertMethodInfo_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!found
                    && instruction.Calls(_get_IsByRef))
                {
                    found = true;
                    yield return new(OpCodes.Call, _fixedIsByRef);
                    MelonDebug.Msg("Patched ClassInjector.ConvertMethodInfo -> Type.IsByRef");
                }
                else
                    yield return instruction;
            }
        }

        private static void FixedFindAbstractMethods(List<INativeMethodInfoStruct> list, INativeClassStruct klass)
        {
            if (klass.Parent != default) FixedFindAbstractMethods(list, UnityVersionHandler.Wrap(klass.Parent));

            for (var i = 0; i < klass.MethodCount; i++)
            {
                var baseMethod = UnityVersionHandler.Wrap(klass.Methods[i]);
                var name = Marshal.PtrToStringAnsi(baseMethod.Name)!;

                if (baseMethod.Flags.HasFlag(Il2CppMethodFlags.METHOD_ATTRIBUTE_ABSTRACT))
                    list.Add(baseMethod);
                else
                {
                    var existing = list.SingleOrDefault(m =>
                    {
                        if (Marshal.PtrToStringAnsi(m.Name) != name) return false;
                        if (m.ParametersCount != baseMethod.ParametersCount) return false;

                        for (var i = 0; i < m.ParametersCount; i++)
                        {
                            var parameterName = IL2CPP.il2cpp_method_get_param_name(baseMethod.Pointer, (uint)i);
                            var otherParameterName = IL2CPP.il2cpp_method_get_param_name(m.Pointer, (uint)i);

                            var parameterInfo = UnityVersionHandler.Wrap(baseMethod.Parameters, i);
                            var otherParameterInfo = UnityVersionHandler.Wrap(m.Parameters, i);

                            if (Marshal.PtrToStringAnsi(parameterName) != Marshal.PtrToStringAnsi(otherParameterName))
                                return false;

                            if ((string)_getIl2CppTypeFullName.Invoke(null, [(IntPtr)parameterInfo.ParameterType])
                                != (string)_getIl2CppTypeFullName.Invoke(null, [(IntPtr)otherParameterInfo.ParameterType]))
                                return false;
                        }

                        return true;
                    });

                    if (existing != null)
                        list.Remove(existing);
                }
            }
        }
    }
}
#endif