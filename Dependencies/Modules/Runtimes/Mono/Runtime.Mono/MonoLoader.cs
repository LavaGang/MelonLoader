using System;
using System.Collections.Generic;
using MelonLoader.Modules;
using MelonLoader.NativeUtils;
#pragma warning disable 0649

namespace MelonLoader.Runtime.Mono
{
    public unsafe static class MonoLoader
    {
        #region Private Members

        private static MonoLibrary _lib;

        private static MelonNativeDetour<MonoLibrary.d_mono_runtime_invoke> mono_runtime_invoke_detour;

        #endregion

        #region Public Members

        public static MelonEngineModule EngineModule { get; private set; }
        public static MonoRuntimeInfo RuntimeInfo { get; private set; }

        #endregion

        #region Loader

        public static void Initialize(MelonEngineModule engineModule,
            MonoRuntimeInfo runtimeInfo)
        {
            // Apply the information
            EngineModule = engineModule;
            RuntimeInfo = runtimeInfo;

            // Check if it found any Mono variant library
            if (RuntimeInfo == null
                || string.IsNullOrEmpty(RuntimeInfo.LibPath))
            {
                MelonLogger.ThrowInternalFailure("Failed to find Mono Library!");
                return;
            }

            // Load Library
            if (!LoadLib())
                return;

            // Check Exports
            if (!CheckExports())
                return;

            // Create mono_runtime_invoke Detour
            mono_runtime_invoke_detour = new(_lib.mono_runtime_invoke, h_mono_runtime_invoke, false);

            // Attach mono_runtime_invoke Detour
            MelonDebug.Msg($"Attaching mono_runtime_invoke Detour...");
            mono_runtime_invoke_detour.Attach();
        }

        private static bool LoadLib()
        {
            // Load the Mono variant library
            MelonDebug.Msg($"Loading Mono Library...");
            _lib = new MelonNativeLibrary<MonoLibrary>(MelonNativeLibrary.LoadLib(RuntimeInfo.LibPath)).Instance;

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

            listOfExports[nameof(_lib.mono_method_get_name)] = _lib.mono_method_get_name;
            listOfExports[nameof(_lib.mono_runtime_invoke)] = _lib.mono_runtime_invoke;

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

        private static IntPtr h_mono_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc)
        {
            // Get Method Name
            string methodName = _lib.mono_method_get_name(method).ToAnsiString();

            // Check for Trigger Method
            foreach (string triggerMethod in RuntimeInfo.TriggerMethods)
            {
                if (!methodName.Contains(triggerMethod))
                    continue;

                // Detach mono_runtime_invoke Detour
                mono_runtime_invoke_detour.Detach();

                // Initiate Stage2
                EngineModule.Stage2();

                // Return original Invoke without Trampoline
                return _lib.mono_runtime_invoke(method, obj, param, ref exc);
            }

            // Return original Invoke
            return mono_runtime_invoke_detour.Trampoline(method, obj, param, ref exc);
        }

        #endregion
    }
}
