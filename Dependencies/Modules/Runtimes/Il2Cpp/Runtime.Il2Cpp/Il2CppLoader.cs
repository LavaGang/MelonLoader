using System;
using System.Collections.Generic;
using MelonLoader.Il2Cpp;
using MelonLoader.Modules;
using MelonLoader.NativeUtils;
#pragma warning disable 0649

namespace MelonLoader.Runtime.Il2Cpp
{
    public unsafe static class Il2CppLoader
    {
        #region Private Members

        private static Il2CppLibrary _lib;

        private static MelonNativeDetour<Il2CppLibrary.d_il2cpp_runtime_invoke> il2cpp_runtime_invoke_detour;

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
            if (RuntimeInfo == null
                || string.IsNullOrEmpty(RuntimeInfo.LibPath))
            {
                MelonLogger.ThrowInternalFailure("Failed to find Il2Cpp Library!");
                return;
            }

            // Load Library
            if (!LoadLib())
                return;

            // Check Exports
            if (!CheckExports())
                return;

            // Create il2cpp_runtime_invoke Detour
            il2cpp_runtime_invoke_detour = new(_lib.il2cpp_runtime_invoke, h_il2cpp_runtime_invoke, false);

            // Attach il2cpp_runtime_invoke Detour
            MelonDebug.Msg($"Attaching il2cpp_runtime_invoke Detour...");
            il2cpp_runtime_invoke_detour.Attach();
        }

        private static bool LoadLib()
        {
            // Load the Il2Cpp variant library
            MelonDebug.Msg($"Loading Il2Cpp Library...");
            _lib = new MelonNativeLibrary<Il2CppLibrary>(MelonNativeLibrary.LoadLib(RuntimeInfo.LibPath)).Instance;

            // Check for Failure
            if (_lib == null)
            {
                MelonLogger.ThrowInternalFailure($"Failed to load Il2Cpp Library from {RuntimeInfo.LibPath}!");
                return false;
            }

            return true;
        }

        private static bool CheckExports()
        {
            Dictionary<string, Delegate> listOfExports = new();

            listOfExports[nameof(_lib.il2cpp_method_get_name)] = _lib.il2cpp_method_get_name;
            listOfExports[nameof(_lib.il2cpp_runtime_invoke)] = _lib.il2cpp_runtime_invoke;

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

        private static IntPtr h_il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc)
        {
            // Get Method Name
            string methodName = _lib.il2cpp_method_get_name(method).ToAnsiString();

            // Check for Trigger Method
            foreach (string triggerMethod in RuntimeInfo.TriggerMethods)
            {
                if (!methodName.Contains(triggerMethod))
                    continue;

                // Detach il2cpp_runtime_invoke Detour
                il2cpp_runtime_invoke_detour.Detach();

                // Initiate Stage2
                EngineModule.Stage2();

                // Return original Invoke without Trampoline
                return _lib.il2cpp_runtime_invoke(method, obj, param, ref exc);
            }

            // Return original Invoke
            return il2cpp_runtime_invoke_detour.Trampoline(method, obj, param, ref exc);
        }

        #endregion
    }
}
