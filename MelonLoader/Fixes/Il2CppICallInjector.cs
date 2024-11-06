#if NET6_0_OR_GREATER

using MelonLoader.NativeUtils;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Il2CppInterop.HarmonySupport;
using HarmonyLib;

#pragma warning disable 0649

namespace MelonLoader.Fixes
{
    internal static class Il2CppICallInjector
    {
        private static Dictionary<string, (object, DynamicMethodDefinition, MethodInfo, IntPtr)> _lookup = new();

        private delegate IntPtr dil2cpp_resolve_icall(IntPtr signature);
        private static NativeHook<dil2cpp_resolve_icall> il2cpp_resolve_icall_hook;

        private delegate void dil2cpp_add_internal_call(IntPtr signature, IntPtr funcPtr);
        private static dil2cpp_add_internal_call il2cpp_add_internal_call;

        private static Type _il2CppDetourMethodPatcher;
        private static MethodInfo _generateNativeToManagedTrampoline;

        private static bool _extendedDebug;
        private static MelonLogger.Instance _logger;

        internal static unsafe void Install()
        {
            try
            {
                _logger = new MelonLogger.Instance(nameof(Il2CppICallInjector));

                _il2CppDetourMethodPatcher = typeof(HarmonySupport).Assembly.GetType("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher");
                if (_il2CppDetourMethodPatcher == null)
                    throw new Exception("Failed to get Il2CppDetourMethodPatcher");

                _generateNativeToManagedTrampoline = _il2CppDetourMethodPatcher.GetMethod("GenerateNativeToManagedTrampoline", BindingFlags.NonPublic | BindingFlags.Instance);
                if (_generateNativeToManagedTrampoline == null)
                    throw new Exception("Failed to get Il2CppDetourMethodPatcher.GenerateNativeToManagedTrampoline");

                string gameAssemblyName = "GameAssembly";
                NativeLibrary gameAssemblyLib = NativeLibrary.Load(gameAssemblyName);
                if (gameAssemblyLib == null)
                    throw new Exception($"Failed to load {gameAssemblyName} Native Library");

                IntPtr il2cpp_resolve_icall = gameAssemblyLib.GetExport(nameof(il2cpp_resolve_icall));
                if (il2cpp_resolve_icall == IntPtr.Zero)
                    throw new Exception($"Failed to get {nameof(il2cpp_resolve_icall)} Native Export");

                il2cpp_add_internal_call = gameAssemblyLib.GetExport<dil2cpp_add_internal_call>(nameof(il2cpp_add_internal_call));
                if (il2cpp_add_internal_call == null)
                    throw new Exception($"Failed to get {nameof(il2cpp_add_internal_call)} Native Export");

                MelonDebug.Msg("Patching il2cpp_resolve_icall...");
                IntPtr detourPtr = Marshal.GetFunctionPointerForDelegate((dil2cpp_resolve_icall)il2cpp_resolve_icall_Detour);
                il2cpp_resolve_icall_hook = new NativeHook<dil2cpp_resolve_icall>(il2cpp_resolve_icall, detourPtr);
                il2cpp_resolve_icall_hook.Attach();
            }
            catch (Exception e)
            {
                LogDebugWarning(e.ToString());
            }
        }

        internal static void Shutdown()
        {
            if (il2cpp_resolve_icall_hook != null)
            {
                if (il2cpp_resolve_icall_hook.IsHooked)
                    il2cpp_resolve_icall_hook.Detach();
                il2cpp_resolve_icall_hook = null;
            }

            if (_lookup != null)
            {
                if (_lookup.Count > 0)
                    _lookup.Clear();
                _lookup = null;
            }
        }

        private static void LogMsg(string msg)
            => _logger.Msg(msg);
        private static void LogError(string msg)
            => _logger.Error(msg);
        private static void LogDebugMsg(string msg)
        {
            if (!_extendedDebug
                || !MelonDebug.IsEnabled())
                return;
            _logger.Msg(msg);
        }
        private static void LogDebugWarning(string msg)
        {
            if (!_extendedDebug
                || !MelonDebug.IsEnabled())
                return;
            _logger.Warning(msg);
        }

        private static IntPtr il2cpp_resolve_icall_Detour(IntPtr signature)
        {
            // Convert Pointer to String
            string signatureStr = Marshal.PtrToStringAnsi(signature);
            if (string.IsNullOrEmpty(signatureStr))
                return IntPtr.Zero;

            // Check Cache
            if (_lookup.TryGetValue(signatureStr, out var result))
            {
                LogDebugMsg($"Resolved {signatureStr} to ICall in Cache");
                return result.Item4;
            }

            // Run Original
            IntPtr originalResult = il2cpp_resolve_icall_hook.Trampoline(signature);
            if (originalResult != IntPtr.Zero)
            {
                // Cache Original Result
                LogDebugMsg($"Resolved {signatureStr} to Unity ICall");
                _lookup[signatureStr] = (null, null, null, originalResult);
                return originalResult;
            }

            // Check if Injection is Needed
            if (!ShouldInject(signatureStr, out MethodInfo unityShimMethod))
            {
                LogDebugWarning($"Unable to find suitable method to inject for {signatureStr}");
                return IntPtr.Zero;
            }

            // Create Injected Function and Cache Return
            LogDebugMsg($"Generating Trampoline for {signatureStr}");
            var pair = GenerateTrampoline(unityShimMethod);
            if (pair.Item4 == IntPtr.Zero)
            {
                LogDebugWarning($"Failed to generate trampoline for {signatureStr}");
                return IntPtr.Zero;
            }

            // Add New ICall to Il2Cpp Domain
            _lookup[signatureStr] = pair;
            il2cpp_add_internal_call(signature, pair.Item4);
            LogMsg($"Registered mono icall {signatureStr} in il2cpp domain");

            // Return New Function Pointer
            return pair.Item4;
        }

        private static Type FindType(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName))
                return null;

            Type result = null;
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
                    break;
            }

            return result;
        }

        private static bool ShouldInject(string signature, out MethodInfo unityShimMethod)
        {
            unityShimMethod = null;

            // Split the Signature
            string[] split = signature.Split("::");
            string typeName = split[0];
            string methodName = split[1];

            // Find Managed Type
            Type newType = FindType(typeName);
            if (newType == null)
                return false;

            // Find Managed Method
            MethodInfo targetMethod = null;
            try
            {
                // Get All Methods
                MethodInfo[] allMethods = newType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                foreach (var method in allMethods)
                {
                    // Validate Method
                    if ((method == null)
                        || (method.Name != methodName))
                        continue;

                    // Check for Generic Method since ICalls can't be Generic
                    if (method.IsGenericMethod)
                        continue;

                    // Check for PInvoke to prevent Recursion
                    if (method.Attributes.HasFlag(MethodAttributes.PinvokeImpl))
                        continue;

                    // Check for Extern to prevent Recursion
                    var methodImpl = method.GetMethodImplementationFlags();
                    if (methodImpl.HasFlag(MethodImplAttributes.InternalCall)
                        || methodImpl.HasFlag(MethodImplAttributes.Native)
                        || methodImpl.HasFlag(MethodImplAttributes.Unmanaged))
                        continue;

                    // Check if Method has no Body or just throws NotImplementedException
                    if (!method.HasMethodBody()
                        || method.IsNotImplemented())
                        continue;

                    // Found Shim
                    targetMethod = method;
                    break;
                }
            }
            catch { return false; }
            if (targetMethod == null)
                return false;

            // Inject ICall
            unityShimMethod = targetMethod;
            return true;
        }

        private static (object, DynamicMethodDefinition, MethodInfo, IntPtr) GenerateTrampoline(MethodInfo unityShimMethod)
        {
            // Create Patcher
            object patcher = Activator.CreateInstance(_il2CppDetourMethodPatcher, [ unityShimMethod ]);
            if (patcher == null)
                return (null, null, null, IntPtr.Zero);

            // Create New Injected ICall Method
            DynamicMethodDefinition trampoline = (DynamicMethodDefinition)_generateNativeToManagedTrampoline.Invoke(patcher, [ unityShimMethod ]);
            if (trampoline == null)
                return (null, null, null, IntPtr.Zero);
           
            // Return the New Method
            MethodInfo newMethod = trampoline.Generate().Pin();
            return (patcher, trampoline, newMethod, newMethod.GetNativeStart());
        }
    }
}

#endif