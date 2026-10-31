#if NET6_0_OR_GREATER && OSX && ARM64

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MelonLoader.NativeUtils.MachOParser;

namespace MelonLoader.Fixes.Il2CppInterop
{
    // Il2CppInterop finds the native functions it hooks for class injection by following jumps with its
    // x86-only xref scanner, which finds nothing in ARM64 code. GameAssembly on macOS keeps its local
    // symbol table, so we resolve those functions by their mangled names instead
    internal static class Il2CppInteropMacOSSymbolHooksFix
    {
        private const string HooksNamespace = "Il2CppInterop.Runtime.Injection.Hooks.";
        private const string ClassInitSymbol = "__ZN6il2cpp2vm5Class4InitEP11Il2CppClass";

        // Hook type -> mangled target. GenericMethod_GetMethod_Hook is deliberately absent: its detour takes
        // (Il2CppGenericMethod*, bool), which doesn't match the 1-param GetMethod overload in Unity 6 builds
        private static readonly Dictionary<string, string> HookSymbols = new()
        {
            [HooksNamespace + "GenericMethod_GetMethod_Unity6_Hook"] = "__ZN6il2cpp8metadata13GenericMethod9GetMethodEPK10MethodInfoPK17Il2CppGenericInstS7_",
            [HooksNamespace + "Class_GetFieldDefaultValue_Hook"] = "__ZN6il2cpp2vm5Class20GetFieldDefaultValueEPK9FieldInfoPPK10Il2CppType",
            [HooksNamespace + "Class_FromIl2CppType_Hook"] = "__ZN6il2cpp2vm5Class14FromIl2CppTypeEPK10Il2CppTypeb",
            [HooksNamespace + "Class_FromName_Hook"] = "__ZN6il2cpp2vm5Class8FromNameEPK11Il2CppImagePKcS6_",
        };

        private static Dictionary<string, IntPtr> _resolved;
        private static FieldInfo _classInitField;

        internal static void Install()
        {
            try
            {
                Type thisType = typeof(Il2CppInteropMacOSSymbolHooksFix);
                Assembly runtimeAssembly = typeof(ClassInjector).Assembly;

                Type injectorType = runtimeAssembly.GetType("Il2CppInterop.Runtime.Injection.InjectorHelpers", false);
                if (injectorType == null)
                    throw new Exception("Failed to get InjectorHelpers");

                _classInitField = injectorType.GetField("ClassInit", BindingFlags.NonPublic | BindingFlags.Static);
                if (_classInitField == null)
                    throw new Exception("Failed to get InjectorHelpers.ClassInit");

                MethodInfo setupMethod = injectorType.GetMethod("Setup", BindingFlags.NonPublic | BindingFlags.Static);
                if (setupMethod == null)
                    throw new Exception("Failed to get InjectorHelpers.Setup");

                MelonDebug.Msg("Patching Il2CppInterop InjectorHelpers.Setup for macOS symbol resolution...");
                Core.HarmonyInstance.Patch(setupMethod,
                    new HarmonyMethod(thisType.GetMethod(nameof(Setup_Prefix), BindingFlags.NonPublic | BindingFlags.Static)));

                HarmonyMethod findTargetPrefix = new(thisType.GetMethod(nameof(FindTargetMethod_Prefix), BindingFlags.NonPublic | BindingFlags.Static));
                foreach (string hookTypeName in HookSymbols.Keys)
                {
                    Type hookType = runtimeAssembly.GetType(hookTypeName, false);
                    if (hookType == null)
                        throw new Exception($"Failed to get {hookTypeName}");

                    MethodInfo findTargetMethod = hookType.GetMethod("FindTargetMethod", BindingFlags.Public | BindingFlags.Instance);
                    if (findTargetMethod == null)
                        throw new Exception($"Failed to get {hookTypeName}.FindTargetMethod");

                    MelonDebug.Msg($"Patching {hookType.Name}.FindTargetMethod...");
                    Core.HarmonyInstance.Patch(findTargetMethod, findTargetPrefix);
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }

        // Setup only looks for Class::Init when ClassInit is unset, so providing it here skips FindClassInit's scanner
        private static void Setup_Prefix()
        {
            if (_classInitField.GetValue(null) != null)
                return;

            IntPtr classInit = Resolve(ClassInitSymbol);
            if (classInit == IntPtr.Zero)
                return;

            _classInitField.SetValue(null, Marshal.GetDelegateForFunctionPointer(classInit, _classInitField.FieldType));
            MelonDebug.Msg($"Using Class::Init from GameAssembly symbols: 0x{classInit:X}");
        }

        private static bool FindTargetMethod_Prefix(object __instance, ref IntPtr __result)
        {
            if (!HookSymbols.TryGetValue(__instance.GetType().FullName, out string symbol))
                return true;

            IntPtr target = Resolve(symbol);
            if (target == IntPtr.Zero)
                return true; // fall back to Il2CppInterop's own lookup

            MelonDebug.Msg($"Using {__instance.GetType().Name} target from GameAssembly symbols: 0x{target:X}");
            __result = target;
            return false;
        }

        private static IntPtr Resolve(string symbol)
        {
            _resolved ??= ResolveAll();
            return _resolved.TryGetValue(symbol, out IntPtr address) ? address : IntPtr.Zero;
        }

        private static Dictionary<string, IntPtr> ResolveAll()
        {
            var resolved = new Dictionary<string, IntPtr>();
            try
            {
                Dictionary<string, IntPtr> addresses = GameAssemblySymbols.Resolve(HookSymbols.Values.Append(ClassInitSymbol).ToList());

                // Class::Init is only called, never patched, so it doesn't need to be a patchable function
                if (addresses.TryGetValue(ClassInitSymbol, out IntPtr classInit))
                {
                    resolved[ClassInitSymbol] = classInit;
                    MelonDebug.Msg($"Resolved {ClassInitSymbol} -> 0x{classInit:X}");
                }
                else
                    MelonLogger.Warning($"{ClassInitSymbol} not found in GameAssembly's symbol table, falling back to Il2CppInterop's lookup");

                foreach (string symbol in HookSymbols.Values)
                {
                    if (!addresses.TryGetValue(symbol, out IntPtr address))
                    {
                        MelonLogger.Warning($"{symbol} not found in GameAssembly's symbol table, falling back to Il2CppInterop's lookup");
                        continue;
                    }

                    IntPtr target = GameAssemblySymbols.GetPatchableTarget(address);
                    if (target == IntPtr.Zero)
                        continue;

                    resolved[symbol] = target;
                    MelonDebug.Msg(target == address
                        ? $"Resolved {symbol} -> 0x{address:X}"
                        : $"Resolved {symbol} -> 0x{address:X} (branch thunk, using its target 0x{target:X})");
                }
            }
            catch (Exception e)
            {
                MelonLogger.Error("Failed to resolve Il2CppInterop hook targets from GameAssembly's symbol table", e);
            }
            return resolved;
        }
    }
}
#endif
