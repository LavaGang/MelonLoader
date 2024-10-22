#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Il2CppInterop.Generator.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.Class;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.MethodInfo;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.Type;
using HarmonyLib;
using System.IO;
using MelonLoader.Utils;
using Il2CppInterop.Generator.Contexts;
using AsmResolver.DotNet;
using Il2CppInterop.HarmonySupport;

#pragma warning disable CS8632

namespace MelonLoader.Fixes
{
    // fixes: https://github.com/BepInEx/Il2CppInterop/pull/103
    // fixes: https://github.com/BepInEx/Il2CppInterop/issues/135
    // reverts: https://github.com/BepInEx/Il2CppInterop/commit/18e58ef5db42a71d6012ab0387b107a4132101eb
    // fixes the rest of: https://github.com/BepInEx/Il2CppInterop/pull/134
    internal unsafe static class Il2CppInteropFixes
    {
        private static MelonLogger.Instance _logger = new("Il2CppInterop");

        private static Dictionary<RewriteGlobalContext, Dictionary<string, AssemblyRewriteContext>> _assemblyLookup = new();
        private static Dictionary<IntPtr, Type> _typeLookup = new();
        private static Dictionary<string, Type> _typeNameLookup = new();

        private static MethodInfo _getType;
        private static MethodInfo _fixedFindType;
        private static MethodInfo _fixedAddTypeToLookup;
        private static MethodInfo _rewriteType;
        private static MethodInfo _rewriteType_Prefix;
        private static MethodInfo _systemTypeFromIl2CppType;
        private static MethodInfo _systemTypeFromIl2CppType_Prefix;
        private static MethodInfo _systemTypeFromIl2CppType_Transpiler;
        private static MethodInfo _injectorHelpers_AddTypeToLookup;
        private static MethodInfo _registerTypeInIl2Cpp;
        private static MethodInfo _registerTypeInIl2Cpp_Transpiler;
        private static MethodInfo _isTypeSupported;
        private static MethodInfo _isTypeSupported_Transpiler;
        private static MethodInfo _convertMethodInfo;
        private static MethodInfo _convertMethodInfo_Transpiler;
        private static MethodInfo _getIl2CppTypeFullName;
        private static MethodInfo _fixedIsByRef;
        private static MethodInfo _get_IsByRef;
        private static MethodInfo _fixedFindAbstractMethods;
        private static MethodInfo _emitObjectToPointer;
        private static MethodInfo _emitObjectToPointer_Prefix;
        private static MethodInfo _rewriteGlobalContext_AddAssemblyContext;
        private static MethodInfo _rewriteGlobalContext_AddAssemblyContext_Postfix;
        private static MethodInfo _rewriteGlobalContext_Dispose;
        private static MethodInfo _rewriteGlobalContext_Dispose_Prefix;
        private static MethodInfo _rewriteGlobalContext_GetNewAssemblyForOriginal;
        private static MethodInfo _rewriteGlobalContext_GetNewAssemblyForOriginal_Prefix;
        private static MethodInfo _rewriteGlobalContext_TryGetNewTypeForOriginal;
        private static MethodInfo _rewriteGlobalContext_TryGetNewTypeForOriginal_Prefix;
        private static MethodInfo _reportException;
        private static MethodInfo _reportException_Prefix;

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
                Type typeType = typeof(Type);
                Type thisType = typeof(Il2CppInteropFixes);
                Type classInjectorType = typeof(ClassInjector);
                Type ilGeneratorEx = typeof(ILGeneratorEx);
                Type rewriteGlobalContextType = typeof(RewriteGlobalContext);
                Type il2cppType = typeof(IL2CPP);
                Type harmonySupportType = typeof(HarmonySupport);

                Type injectorHelpersType = classInjectorType.Assembly.GetType("Il2CppInterop.Runtime.Injection.InjectorHelpers");
                if (injectorHelpersType == null)
                    throw new Exception("Failed to get InjectorHelpers");

                Type detourMethodPatcherType = harmonySupportType.Assembly.GetType("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher");
                if (detourMethodPatcherType == null)
                    throw new Exception("Failed to get Il2CppDetourMethodPatcher");

                _systemTypeFromIl2CppType = classInjectorType.GetMethod("SystemTypeFromIl2CppType", BindingFlags.NonPublic | BindingFlags.Static);
                if (_systemTypeFromIl2CppType == null)
                    throw new Exception("Failed to get ClassInjector.SystemTypeFromIl2CppType");

                _getIl2CppTypeFullName = classInjectorType.GetMethod("GetIl2CppTypeFullName", BindingFlags.NonPublic | BindingFlags.Static);
                if (_getIl2CppTypeFullName == null)
                    throw new Exception("Failed to get ClassInjector.GetIl2CppTypeFullName");

                _rewriteType = classInjectorType.GetMethod("RewriteType", BindingFlags.NonPublic | BindingFlags.Static);
                if (_rewriteType == null)
                    throw new Exception("Failed to get ClassInjector.RewriteType");

                _registerTypeInIl2Cpp = classInjectorType.GetMethod("RegisterTypeInIl2Cpp",
                    BindingFlags.Public | BindingFlags.Static, 
                    [typeof(Type), typeof(RegisterTypeOptions)]);
                if (_registerTypeInIl2Cpp == null)
                    throw new Exception("Failed to get ClassInjector.RegisterTypeInIl2Cpp");

                _isTypeSupported = classInjectorType.GetMethod("IsTypeSupported", BindingFlags.NonPublic | BindingFlags.Static);
                if (_isTypeSupported == null)
                    throw new Exception("Failed to get ClassInjector.IsTypeSupported");

                _convertMethodInfo = classInjectorType.GetMethod("ConvertMethodInfo", BindingFlags.NonPublic | BindingFlags.Static);
                if (_convertMethodInfo == null)
                    throw new Exception("Failed to get ClassInjector.ConvertMethodInfo");

                _emitObjectToPointer = ilGeneratorEx.GetMethod("EmitObjectToPointer", BindingFlags.Public | BindingFlags.Static);
                if (_emitObjectToPointer == null)
                    throw new Exception("Failed to get ILGeneratorEx.EmitObjectToPointer");

                _injectorHelpers_AddTypeToLookup = injectorHelpersType.GetMethod("AddTypeToLookup", 
                    BindingFlags.NonPublic | BindingFlags.Static, 
                    [typeof(Type), typeof(IntPtr)]);
                if (_injectorHelpers_AddTypeToLookup == null)
                    throw new Exception("Failed to get InjectorHelpers.AddTypeToLookup");

                _getType = typeType.GetMethod("GetType", BindingFlags.Public | BindingFlags.Static, [typeof(string)]);
                if (_getType == null)
                    throw new Exception("Failed to get Type.GetType");

                _get_IsByRef = typeType.GetProperty("IsByRef", BindingFlags.Public | BindingFlags.Instance)?.GetGetMethod();
                if (_get_IsByRef == null)
                    throw new Exception("Failed to get Type.IsByRef.get");

                _rewriteGlobalContext_AddAssemblyContext = rewriteGlobalContextType.GetMethod("AddAssemblyContext", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (_rewriteGlobalContext_AddAssemblyContext == null)
                    throw new Exception("Failed to get RewriteGlobalContext.AddAssemblyContext");

                _rewriteGlobalContext_Dispose = rewriteGlobalContextType.GetMethod("Dispose",
                    BindingFlags.Public | BindingFlags.Instance);
                if (_rewriteGlobalContext_Dispose == null)
                    throw new Exception("Failed to get RewriteGlobalContext.Dispose");

                _rewriteGlobalContext_GetNewAssemblyForOriginal = rewriteGlobalContextType.GetMethod("GetNewAssemblyForOriginal",
                    BindingFlags.Public | BindingFlags.Instance);
                if (_rewriteGlobalContext_GetNewAssemblyForOriginal == null)
                    throw new Exception("Failed to get RewriteGlobalContext.GetNewAssemblyForOriginal");

                _rewriteGlobalContext_TryGetNewTypeForOriginal = rewriteGlobalContextType.GetMethod("TryGetNewTypeForOriginal",
                    BindingFlags.Public | BindingFlags.Instance);
                if (_rewriteGlobalContext_TryGetNewTypeForOriginal == null)
                    throw new Exception("Failed to get RewriteGlobalContext.TryGetNewTypeForOriginal");

                _reportException = detourMethodPatcherType.GetMethod("ReportException",
                    BindingFlags.NonPublic | BindingFlags.Static);
                if (_reportException == null)
                    throw new Exception("Failed to get Il2CppDetourMethodPatcher.ReportException");

                _fixedFindType = thisType.GetMethod(nameof(FixedFindType), BindingFlags.NonPublic | BindingFlags.Static);
                _fixedAddTypeToLookup = thisType.GetMethod(nameof(FixedAddTypeToLookup), BindingFlags.NonPublic | BindingFlags.Static);
                _fixedIsByRef = thisType.GetMethod(nameof(FixedIsByRef), BindingFlags.NonPublic | BindingFlags.Static);
                _fixedFindAbstractMethods = thisType.GetMethod(nameof(FixedFindAbstractMethods), BindingFlags.NonPublic | BindingFlags.Static);
                _systemTypeFromIl2CppType_Prefix = thisType.GetMethod(nameof(SystemTypeFromIl2CppType_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                _systemTypeFromIl2CppType_Transpiler = thisType.GetMethod(nameof(SystemTypeFromIl2CppType_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
                _rewriteType_Prefix = thisType.GetMethod(nameof(RewriteType_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                _registerTypeInIl2Cpp_Transpiler = thisType.GetMethod(nameof(RegisterTypeInIl2Cpp_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
                _isTypeSupported_Transpiler = thisType.GetMethod(nameof(IsTypeSupported_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
                _convertMethodInfo_Transpiler = thisType.GetMethod(nameof(ConvertMethodInfo_Transpiler), BindingFlags.NonPublic | BindingFlags.Static);
                _emitObjectToPointer_Prefix = thisType.GetMethod(nameof(EmitObjectToPointer_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                _rewriteGlobalContext_AddAssemblyContext_Postfix = thisType.GetMethod(nameof(RewriteGlobalContext_AddAssemblyContext_Postfix), BindingFlags.NonPublic | BindingFlags.Static);
                _rewriteGlobalContext_Dispose_Prefix = thisType.GetMethod(nameof(RewriteGlobalContext_Dispose_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                _rewriteGlobalContext_GetNewAssemblyForOriginal_Prefix = thisType.GetMethod(nameof(RewriteGlobalContext_GetNewAssemblyForOriginal_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                _rewriteGlobalContext_TryGetNewTypeForOriginal_Prefix = thisType.GetMethod(nameof(RewriteGlobalContext_TryGetNewTypeForOriginal_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
                _reportException_Prefix = thisType.GetMethod(nameof(ReportException_Prefix), BindingFlags.NonPublic | BindingFlags.Static);

                LogDebugMsg("Patching Il2CppInterop ClassInjector.SystemTypeFromIl2CppType...");
                Core.HarmonyInstance.Patch(_systemTypeFromIl2CppType,
                    new HarmonyMethod(_systemTypeFromIl2CppType_Prefix), 
                    null,
                    new HarmonyMethod(_systemTypeFromIl2CppType_Transpiler));

                LogDebugMsg("Patching Il2CppInterop ClassInjector.RegisterTypeInIl2Cpp...");
                Core.HarmonyInstance.Patch(_registerTypeInIl2Cpp,
                    null,
                    null,
                    new HarmonyMethod(_registerTypeInIl2Cpp_Transpiler));

                LogDebugMsg("Patching Il2CppInterop ClassInjector.IsTypeSupported...");
                Core.HarmonyInstance.Patch(_isTypeSupported,
                    null,
                    null,
                    new HarmonyMethod(_isTypeSupported_Transpiler));

                LogDebugMsg("Patching Il2CppInterop ClassInjector.RewriteType...");
                Core.HarmonyInstance.Patch(_rewriteType,
                    new HarmonyMethod(_rewriteType_Prefix));

                LogDebugMsg("Patching Il2CppInterop ClassInjector.ConvertMethodInfo...");
                Core.HarmonyInstance.Patch(_convertMethodInfo,
                    null,
                    null,
                    new HarmonyMethod(_convertMethodInfo_Transpiler));

                LogDebugMsg("Patching Il2CppInterop ILGeneratorEx.EmitObjectToPointer...");
                Core.HarmonyInstance.Patch(_emitObjectToPointer,
                    new HarmonyMethod(_emitObjectToPointer_Prefix));

                LogDebugMsg("Patching Il2CppInterop RewriteGlobalContext.AddAssemblyContext...");
                Core.HarmonyInstance.Patch(_rewriteGlobalContext_AddAssemblyContext,
                    null, new HarmonyMethod(_rewriteGlobalContext_AddAssemblyContext_Postfix));

                LogDebugMsg("Patching Il2CppInterop RewriteGlobalContext.Dispose...");
                Core.HarmonyInstance.Patch(_rewriteGlobalContext_Dispose,
                    new HarmonyMethod(_rewriteGlobalContext_Dispose_Prefix));

                LogDebugMsg("Patching Il2CppInterop RewriteGlobalContext.GetNewAssemblyForOriginal...");
                Core.HarmonyInstance.Patch(_rewriteGlobalContext_GetNewAssemblyForOriginal,
                    new HarmonyMethod(_rewriteGlobalContext_GetNewAssemblyForOriginal_Prefix));

                LogDebugMsg("Patching Il2CppInterop RewriteGlobalContext.TryGetNewTypeForOriginal...");
                Core.HarmonyInstance.Patch(_rewriteGlobalContext_TryGetNewTypeForOriginal,
                    new HarmonyMethod(_rewriteGlobalContext_TryGetNewTypeForOriginal_Prefix));

                LogDebugMsg("Patching Il2CppInterop Il2CppDetourMethodPatcher.ReportException...");
                Core.HarmonyInstance.Patch(_reportException,
                    new HarmonyMethod(_reportException_Prefix));
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        internal static void Shutdown()
        {
            if (_assemblyLookup != null)
            {
                if (_assemblyLookup.Count > 0)
                {
                    foreach (Dictionary<string, AssemblyRewriteContext> dict in _assemblyLookup.Values)
                        dict.Clear();
                    _assemblyLookup.Clear();
                }
                _assemblyLookup = null;
            }

            if (_typeLookup != null)
            {
                if (_typeLookup.Count > 0)
                    _typeLookup.Clear();
                _typeLookup = null;
            }
        }

        private static bool FixedIsByRef(Type type)
            => (type != null) && (type.IsByRef || type.IsPointer);

        internal static Type FixedFindType(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName))
                return null;

            if (_typeNameLookup.TryGetValue(typeFullName, out Type result))
                return result;

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a == null)
                    continue;

                result = a.GetValidType($"Il2Cpp.{typeFullName}");
                if (result == null)
                    result = a.GetValidType($"Il2Cpp{typeFullName}");
                if (result == null)
                    result = a.GetValidType(typeFullName);

                if (result != null)
                {
                    _typeNameLookup[result.FullName] = result;
                    return result;
                }
            }

            return null;
        }

        private static void FixedAddTypeToLookup(Type type, IntPtr typePointer)
        {
            if ((type == null)
                || (typePointer == IntPtr.Zero))
                return;

            _injectorHelpers_AddTypeToLookup.Invoke(null, [type, typePointer]);

            typePointer = IL2CPP.il2cpp_class_get_type(typePointer);
            if (typePointer !=  IntPtr.Zero)
                _typeLookup.Add(typePointer, type);
        }

        private static bool ReportException_Prefix(Exception __0)
        {
            LogError("During invoking native->managed trampoline", __0);
            return false;
        }

        private static bool EmitObjectToPointer_Prefix(bool __7, ref bool __8)
        {
            __8 = __7;
            return true;
        }

        private static bool RewriteType_Prefix(Type __0, ref Type __result)
        {
            if (__0 == null)
                return true;

            if (__0 == typeof(void*))
            {
                __result = __0;
                return false;
            }

            return true;
        }

        private static void RewriteGlobalContext_AddAssemblyContext_Postfix(RewriteGlobalContext __instance,
            AssemblyRewriteContext __1)
        {
            if ((__instance == null)
                || (__1 == null)
                || __1.OriginalAssembly == null)
                return;

            if (!_assemblyLookup.TryGetValue(__instance, out Dictionary<string, AssemblyRewriteContext> contexts)
                || (contexts == null))
                contexts = _assemblyLookup[__instance] = new();

            string assemblyName = __1.OriginalAssembly.Name;
            if (string.IsNullOrEmpty(assemblyName))
                return;

            contexts[assemblyName] = __1;
            //LogDebugMsg($"[RewriteGlobalContext] Added: {assemblyName}");
        }

        private static bool RewriteGlobalContext_Dispose_Prefix(RewriteGlobalContext __instance)
        {
            if ((__instance == null)
                || !_assemblyLookup.ContainsKey(__instance)
                || !_assemblyLookup.Remove(__instance, out Dictionary<string, AssemblyRewriteContext> contexts)
                || (contexts == null))
                return true;

            contexts.Clear();
            return true;
        }

        private static bool RewriteGlobalContext_GetNewAssemblyForOriginal_Prefix(RewriteGlobalContext __instance,
            AssemblyDefinition __0,
            ref AssemblyRewriteContext __result)
        {
            if ((__instance == null)
                || (__0 == null)
                || !_assemblyLookup.TryGetValue(__instance, out Dictionary<string, AssemblyRewriteContext> contexts)
                || (contexts == null))
                return true;

            string assemblyName = __0.Name;
            if (contexts.TryGetValue(assemblyName, out __result))
            {
                //LogDebugMsg($"[RewriteGlobalContext] Found: {assemblyName}");
                return false;
            }

            if (assemblyName.StartsWith("Il2Cpp"))
                assemblyName = assemblyName.Remove(0, 6);
            else
                assemblyName = $"Il2Cpp{assemblyName}";

            if (contexts.TryGetValue(assemblyName, out __result))
            {
                //LogDebugMsg($"[RewriteGlobalContext] Found: {assemblyName}");
                return false;
            }

            return true;
        }

        private static bool RewriteGlobalContext_TryGetNewTypeForOriginal_Prefix(RewriteGlobalContext __instance,
            TypeDefinition __0,
            ref TypeRewriteContext? __result)
        {
            if ((__instance == null)
                || (__0 == null)
                || (__0.Module == null)
                || (__0.Module.Assembly == null)
                || !_assemblyLookup.TryGetValue(__instance, out Dictionary<string, AssemblyRewriteContext> contexts)
                || (contexts == null))
                return true;

            string assemblyName = __0.Module.Assembly.Name;
            if (string.IsNullOrEmpty(assemblyName)) 
                return false;

            AssemblyRewriteContext rewriteContext = null;
            if (contexts.TryGetValue(assemblyName, out rewriteContext))
            {
                //LogDebugMsg($"[RewriteGlobalContext] Found: {assemblyName}");
                __result = rewriteContext.TryGetContextForOriginalType(__0);
                return false;
            }

            if (assemblyName.StartsWith("Il2Cpp"))
                assemblyName = assemblyName.Remove(0, 6);
            else
                assemblyName = $"Il2Cpp{assemblyName}";
            if (contexts.TryGetValue(assemblyName, out rewriteContext))
            {
                //LogDebugMsg($"[RewriteGlobalContext] Found: {assemblyName}");
                __result = rewriteContext.TryGetContextForOriginalType(__0);
                return false;
            }

            return true;
        }

        private static bool SystemTypeFromIl2CppType_Prefix(Il2CppTypeStruct* __0, ref Type __result)
        {
            if ((IntPtr)__0 == IntPtr.Zero)
                return false;
			
            INativeTypeStruct wrappedType = UnityVersionHandler.Wrap(__0);
            if ((IntPtr)wrappedType.TypePointer == IntPtr.Zero)
                return false;
			
            if (_typeLookup.TryGetValue((IntPtr)wrappedType.TypePointer, out Type type))
            {
                __result = (Type)_rewriteType.Invoke(null, [type]);
                return false;
            }

            IntPtr klass = IL2CPP.il2cpp_class_from_type((IntPtr)wrappedType.TypePointer);
            if (klass == IntPtr.Zero)
                return true;

            IntPtr image = IL2CPP.il2cpp_class_get_image(klass);
            if (image == IntPtr.Zero)
                return true;

            IntPtr klassNamespace = IL2CPP.il2cpp_class_get_namespace(klass);
            if (klassNamespace == IntPtr.Zero)
                return true;

            IntPtr klassName = IL2CPP.il2cpp_class_get_name(klass);
            if (klassName == IntPtr.Zero)
                return true;

            string klassNameStr = Marshal.PtrToStringAnsi(klassName);
            if (string.IsNullOrEmpty(klassNameStr))
                return true;

            string fullTypeName = klassNameStr;
            if (klassNamespace != IntPtr.Zero)
            {
                string klassNamespaceStr = Marshal.PtrToStringAnsi(klassNamespace);
                if (!string.IsNullOrEmpty(klassNamespaceStr))
                    fullTypeName = $"{klassNamespaceStr}.{klassNameStr}";
            }

            IntPtr fileName = IL2CPP.il2cpp_image_get_filename(image);
            if (fileName == IntPtr.Zero)
                return true;

            string fileNameStr = Marshal.PtrToStringAnsi(fileName);
            if (string.IsNullOrEmpty(fileNameStr))
                return true;

            string il2cppAssemblyPath = Path.Combine(MelonEnvironment.Il2CppAssembliesDirectory, fileNameStr);
            if (!File.Exists(il2cppAssemblyPath))
                il2cppAssemblyPath = Path.Combine(MelonEnvironment.Il2CppAssembliesDirectory, $"Il2Cpp{fileNameStr}");
            if (File.Exists(il2cppAssemblyPath))
            {
                Assembly asm = Assembly.LoadFrom(il2cppAssemblyPath);
                if (asm != null)
                {
                    __result = asm.GetType($"Il2Cpp.{fullTypeName}");
                    if (__result == null)
                        __result = asm.GetType($"Il2Cpp{fullTypeName}");
                    if (__result == null)
                        __result = asm.GetType(fullTypeName);
                    if (__result != null)
                        return false;
                }
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
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = _fixedFindType;

                    LogDebugMsg("Patched Il2CppInterop ClassInjector.SystemTypeFromIl2CppType -> Type.GetType");
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
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = _fixedAddTypeToLookup;
                    LogDebugMsg("Patched Il2CppInterop ClassInjector.RegisterTypeInIl2Cpp -> InjectorHelpers.AddTypeToLookup");
                }

                if (!found2
                    && instruction.ToString()
                    .Contains("FindAbstractMethods"))
                {
                    found2 = true;
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = _fixedFindAbstractMethods;
                    LogDebugMsg("Patched Il2CppInterop ClassInjector.RegisterTypeInIl2Cpp -> FindAbstractMethods");
                }

                yield return instruction;
            }
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
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = _fixedIsByRef;
                    LogDebugMsg("Patched Il2CppInterop ClassInjector.ConvertMethodInfo -> Type.IsByRef");
                }

                yield return instruction;
            }
        }

        private static IEnumerable<CodeInstruction> IsTypeSupported_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!found
                    && instruction.Calls(_get_IsByRef))
                {
                    found = true;
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = _fixedIsByRef;
                    LogDebugMsg("Patched Il2CppInterop ClassInjector.IsTypeSupported -> Type.IsByRef");
                }

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

                            string parameterTypeName = (string)_getIl2CppTypeFullName.Invoke(null, [(IntPtr)parameterInfo.ParameterType]);
                            string otherParameterTypeName = (string)_getIl2CppTypeFullName.Invoke(null, [(IntPtr)otherParameterInfo.ParameterType]);

                            if ((parameterTypeName != $"Il2Cpp.{otherParameterTypeName}")
                                && (parameterTypeName != $"Il2Cpp{otherParameterTypeName}")
                                && ($"Il2Cpp.{parameterTypeName}" != otherParameterTypeName)
                                && ($"Il2Cpp{parameterTypeName}" != otherParameterTypeName)
                                && ($"Il2Cpp.{parameterTypeName}" != $"Il2Cpp.{otherParameterTypeName}")
                                && ($"Il2Cpp{parameterTypeName}" != $"Il2Cpp{otherParameterTypeName}")
                                && (parameterTypeName != otherParameterTypeName))
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