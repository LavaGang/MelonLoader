using System;
using System.Collections.Generic;
using MelonLoader.Modules;
using MelonLoader.NativeUtils;
#pragma warning disable 0649

namespace MelonLoader.Runtime.Il2Cpp
{
    public unsafe static class Il2CppLoader
    {
        #region Private Members

        private static MelonNativeDetour<Il2CppLibrary.d_il2cpp_init_version> il2cpp_init_version_detour;
        private static MelonNativeDetour<Il2CppLibrary.d_il2cpp_runtime_invoke> il2cpp_runtime_invoke_detour;

        private static IntPtr il2CppDomain;

        #endregion

        #region Public Members

        public static MelonEngineModule EngineModule { get; private set; }
        public static Il2CppRuntimeInfo RuntimeInfo { get; private set; }

        #endregion

        #region Loader

        public static void Initialize(MelonEngineModule engineModule,
            Il2CppRuntimeInfo runtimeInfo)
        {
            // Apply the information
            EngineModule = engineModule;
            RuntimeInfo = runtimeInfo;

            // Check if it found any Il2Cpp variant library
            if (RuntimeInfo == null)
            {
                MelonLogger.ThrowInternalFailure("Invalid Runtime Info Passed!");
                return;
            }

            // Load Library
            if (!LoadLibrary())
                return;

            // Check Exports
            if (!CheckExports())
                return;

            // Get Il2Cpp Domain
            il2CppDomain = Il2CppLibrary.Instance.il2cpp_domain_get();

            // Handle loading if Domain is already Created
            if (il2CppDomain == IntPtr.Zero)
            {
                // Attach il2cpp_init_version Detour
                MelonDebug.Msg($"Attaching il2cpp_init_version Detour...");
                il2cpp_init_version_detour = new(Il2CppLibrary.Instance.il2cpp_init_version, h_il2cpp_init_version);
                il2cpp_init_version_detour.Attach();
            }
            else
            {
                // Initiate Stage2
                Stage2();
            }
        }

        private static bool LoadLibrary()
        {
            // Check if there is a Posix Helper included with the Mono variant library
            if (string.IsNullOrEmpty(RuntimeInfo.LibraryPath))
                return true;

            // Load Library
            Il2CppLibrary.Load(RuntimeInfo.LibraryPath);
            if (Il2CppLibrary.Instance == null)
            {
                MelonLogger.ThrowInternalFailure($"Failed to load Il2Cpp from {RuntimeInfo.LibraryPath}");
                return false;
            }

            // Return Success
            return true;
        }

        private static bool CheckExports()
        {
            Dictionary<string, Delegate> listOfExports = new();

            listOfExports[nameof(Il2CppLibrary.Instance.il2cpp_domain_get)] = Il2CppLibrary.Instance.il2cpp_domain_get;
            listOfExports[nameof(Il2CppLibrary.Instance.il2cpp_init_version)] = Il2CppLibrary.Instance.il2cpp_init_version;
            listOfExports[nameof(Il2CppLibrary.Instance.il2cpp_method_get_name)] = Il2CppLibrary.Instance.il2cpp_method_get_name;
            listOfExports[nameof(Il2CppLibrary.Instance.il2cpp_runtime_invoke)] = Il2CppLibrary.Instance.il2cpp_runtime_invoke;

            foreach (var exportPair in listOfExports)
            {
                if (exportPair.Value != null)
                    continue;

                MelonLogger.ThrowInternalFailure($"Failed to find {exportPair.Key} Export in Il2Cpp Library!");
                return false;
            }

            return true;
        }

        #endregion

        #region Hooks

        private static IntPtr h_il2cpp_init_version(IntPtr name, IntPtr version)
        {
            // Invoke Domain Creation
            il2CppDomain = il2cpp_init_version_detour.Trampoline(name, version);

            // Detach il2cpp_init_version Detour
            il2cpp_init_version_detour.Detach();

            // Initiate Stage2
            Stage2();

            // Return Domain
            return il2CppDomain;
        }

        private static IntPtr h_il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc)
        {
            // Get Method Name
            string methodName = Il2CppLibrary.Instance.il2cpp_method_get_name(method).ToAnsiString();

            // Check for Trigger Method
            foreach (string triggerMethod in RuntimeInfo.TriggerMethods)
            {
                if (!methodName.Contains(triggerMethod))
                    continue;

                // Detach il2cpp_runtime_invoke Detour
                il2cpp_runtime_invoke_detour.Detach();

                // Initiate Stage3
                Stage3();

                // Return original Invoke without Trampoline
                return Il2CppLibrary.Instance.il2cpp_runtime_invoke(method, obj, param, ref exc);
            }

            // Return original Invoke
            return il2cpp_runtime_invoke_detour.Trampoline(method, obj, param, ref exc);
        }

        #endregion
        
        #region MelonLoader Assembly Wrapping

        private static void Stage2()
        {
            // Initiate Stage2
            EngineModule.Stage2();

            // Attach il2cpp_runtime_invoke Detour
            MelonDebug.Msg($"Attaching il2cpp_runtime_invoke Detour...");
            il2cpp_runtime_invoke_detour = new(Il2CppLibrary.Instance.il2cpp_runtime_invoke, h_il2cpp_runtime_invoke, false);
            il2cpp_runtime_invoke_detour.Attach();
        }

        private static void Stage3()
        {
            // Initiate Stage3
            EngineModule.Stage3(RuntimeInfo.SupportModulePath);
        }

        #endregion
    }
}
