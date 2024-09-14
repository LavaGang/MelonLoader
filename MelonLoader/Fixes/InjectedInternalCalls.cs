#if NET6_0_OR_GREATER

using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
//using System.Runtime.InteropServices;

namespace MelonLoader.Fixes
{
    internal static class InjectedInternalCalls
    {
        private static Dictionary<string, (MethodInfo, IntPtr)> _lookup = new();
        private const string _unityInjectedSuffix = "_Injected";
        private static MethodInfo _il2cpp_resolve_icall;
        private static MethodInfo _il2cpp_resolve_icall_Postfix;

        //[DllImport("GameAssembly", EntryPoint = "il2cpp_resolve_icall", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //private static extern IntPtr il2cpp_resolve_icall_original([MarshalAs(UnmanagedType.LPStr)] string name);

        internal static void Install()
        {
            try
            {
                Type il2cppType = typeof(IL2CPP);
                Type thisType = typeof(InjectedInternalCalls);

                _il2cpp_resolve_icall = il2cppType.GetMethod("il2cpp_resolve_icall", BindingFlags.Public | BindingFlags.Static);
                if (_il2cpp_resolve_icall == null)
                    throw new Exception("Failed to get IL2CPP.il2cpp_resolve_icall");

                _il2cpp_resolve_icall_Postfix = thisType.GetMethod(nameof(il2cpp_resolve_icall_Postfix), BindingFlags.NonPublic | BindingFlags.Static);

                MelonDebug.Msg("Patching Il2CppInterop IL2CPP.il2cpp_resolve_icall...");
                Core.HarmonyInstance.Patch(_il2cpp_resolve_icall,
                    null, new HarmonyMethod(_il2cpp_resolve_icall_Postfix));
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }

        internal static void Shutdown()
        {
            if (_lookup != null)
            {
                if (_lookup.Count > 0)
                    _lookup.Clear();
                _lookup = null;
            }
        }

        private static void il2cpp_resolve_icall_Postfix(string __0, ref IntPtr __result)
        {
            // Found the ICall
            if (__result != IntPtr.Zero)
                return;

            // Needs Resolving
            __result = Resolve(__0);
        }

        /*
        private static IntPtr Resolve(IntPtr signature)
        {
            // Convert Pointer to String
            string signatureStr = IL2CPP.Il2CppStringToManaged(signature);
            if (string.IsNullOrEmpty(signatureStr))
                return IntPtr.Zero;

            // Resolve to Function Pointer
            return Resolve(signatureStr);
        }
        */

        private static IntPtr Resolve(string signature)
        {
            // Check Cache
            if (_lookup.TryGetValue(signature, out var result))
                return result.Item2;

            // Run Original
            // To-Do: Implement when Harmony Patch is replaced with Native Hook
            /*
            IntPtr originalResult = IntPtr.Zero;
            if (originalResult != IntPtr.Zero)
            {
                // Cache Original Result
                _lookup[signature] = (null, originalResult);
                return originalResult;
            }
            */

            // Check if Injection is Needed
            if (!ShouldInject(signature, out MethodInfo unityShimMethod))
                return IntPtr.Zero;

            // Create Injected Function
            var pair = _lookup[signature] = GenerateTrampoline(unityShimMethod);
            return pair.Item2;
        }

        private static bool ShouldInject(string signature, out MethodInfo unityShimMethod)
        {
            unityShimMethod = null;

            // Split the Signature
            string[] split = signature.Split("::");
            string typeName = split[0];

            // Find Managed Type
            Type newType = Il2CppInteropFixes.FixedFindType(typeName);
            if (newType == null)
                return false;

            // Check if ICall was reworked
            string methodName = split[1];
            if (newType.FindMethod($"{methodName}{_unityInjectedSuffix}") == null)
                return false;

            // Find Managed Method
            MethodInfo method = newType.FindMethod(methodName);
            if (method == null)
                return false;

            // ICall needs Injecting
            unityShimMethod = method;
            return true;
        }

        private static (MethodInfo, IntPtr) GenerateTrampoline(MethodInfo unityShimMethod)
        {
            // Convert Method Parameters to Native Parameters
            var methodParams = unityShimMethod.GetParameters();
            int offset = unityShimMethod.IsStatic ? 0 : 1;
            Type[] paramTypes = new Type[methodParams.Length + offset];
            if (!unityShimMethod.IsStatic)
                paramTypes[0] = typeof(IntPtr);
            for (int i = offset; i < methodParams.Length + offset; i++)
            {
                if ((methodParams[i].ParameterType != typeof(string))
                    && methodParams[i].ParameterType.IsValueType)
                    paramTypes[i] = methodParams[i].ParameterType;
                else
                    paramTypes[i] = typeof(IntPtr);
            }

            // Convert Return Type
            Type returnType = unityShimMethod.ReturnType;
            if ((returnType == typeof(string))
                || !returnType.IsValueType)
                returnType = typeof(IntPtr);

            // Create New Injected ICall Method
            string newMethodName = $"{unityShimMethod.Name}_INative";
            var trampoline = new DynamicMethodDefinition(
                newMethodName,
                returnType,
                paramTypes);
            var bodyBuilder = trampoline.GetILGenerator();

            // Begin Try-Catch
            var tryLabel = bodyBuilder.BeginExceptionBlock();

            // Convert Method Parameters to Managed Objects
            for (var i = 0; i < methodParams.Length; i++)
            {
                // Emit Arg Index
                bodyBuilder.Emit(OpCodes.Ldarg, i);

                // Create Managed Object
                var parameterType = methodParams[i].ParameterType;
                if (parameterType == typeof(string))
                {
                    bodyBuilder.Emit(OpCodes.Call, typeof(IL2CPP).GetMethod(nameof(IL2CPP.Il2CppStringToManaged))!);
                }
                else if (!parameterType.IsValueType)
                {
                    var labelNull = bodyBuilder.DefineLabel();
                    var labelDone = bodyBuilder.DefineLabel();
                    bodyBuilder.Emit(OpCodes.Brfalse, labelNull);
                    bodyBuilder.Emit(OpCodes.Ldarg, i);
                    bodyBuilder.Emit(OpCodes.Newobj, parameterType.GetConstructor(new[] { typeof(IntPtr) })!);
                    bodyBuilder.Emit(OpCodes.Br, labelDone);
                    bodyBuilder.MarkLabel(labelNull);
                    bodyBuilder.Emit(OpCodes.Ldnull);
                    bodyBuilder.MarkLabel(labelDone);
                }
            }

            // Call Existing Method
            bodyBuilder.Emit(OpCodes.Call, unityShimMethod);

            // Convert Managed Return
            var oldreturnType = unityShimMethod.ReturnType;
            if (oldreturnType == typeof(string))
            {
                bodyBuilder.Emit(OpCodes.Call, typeof(IL2CPP).GetMethod(nameof(IL2CPP.ManagedStringToIl2Cpp))!);
            }
            else if (!oldreturnType.IsValueType)
            {
                var labelNull = bodyBuilder.DefineLabel();
                var labelDone = bodyBuilder.DefineLabel();
                bodyBuilder.Emit(OpCodes.Dup);
                bodyBuilder.Emit(OpCodes.Brfalse, labelNull);
                bodyBuilder.Emit(OpCodes.Call,
                    typeof(Il2CppObjectBase).GetProperty(nameof(Il2CppObjectBase.Pointer))!.GetMethod);
                bodyBuilder.Emit(OpCodes.Br, labelDone);
                bodyBuilder.MarkLabel(labelNull);
                bodyBuilder.Emit(OpCodes.Pop);
                bodyBuilder.Emit(OpCodes.Ldc_I4_0);
                bodyBuilder.Emit(OpCodes.Conv_I);
                bodyBuilder.MarkLabel(labelDone);
            }

            // Cache Return Value in Lcal
            LocalBuilder returnLocal = null;
            if (returnType != typeof(void))
            {
                returnLocal = bodyBuilder.DeclareLocal(returnType);
                bodyBuilder.Emit(OpCodes.Stloc, returnLocal);
            }

            // Handle Try-Catch thrown Exceptions
            var exceptionLocal = bodyBuilder.DeclareLocal(typeof(Exception));
            bodyBuilder.BeginCatchBlock(typeof(Exception));
            bodyBuilder.Emit(OpCodes.Stloc, exceptionLocal);
            bodyBuilder.Emit(OpCodes.Ldstr, "Exception in IL2CPP Injected ICall: ");
            bodyBuilder.Emit(OpCodes.Ldloc, exceptionLocal);
            bodyBuilder.Emit(OpCodes.Callvirt, typeof(object).GetMethod(nameof(ToString))!);
            bodyBuilder.Emit(OpCodes.Call,
                typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) })!);
            bodyBuilder.Emit(OpCodes.Call, typeof(MelonLogger).GetMethod(nameof(MelonLogger.Error), BindingFlags.Static | BindingFlags.Public, [typeof(string)])!);

            // End Try-Catch
            bodyBuilder.EndExceptionBlock();

            // Restore Return Value from Local
            if (returnLocal != null)
                bodyBuilder.Emit(OpCodes.Ldloc, returnLocal);

            // Return even if there is no Return Value
            bodyBuilder.Emit(OpCodes.Ret);

            // Return the New Method
            MethodInfo newMethod = trampoline.Generate();
            return (newMethod, newMethod.GetNativeStart());
        }
    }
}

#endif