using Il2CppInterop.HarmonySupport;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Startup;
using MelonLoader.Support.Preferences;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader.CoreClrUtils;
using UnityEngine;
using Il2CppInterop.Common;
using Microsoft.Extensions.Logging;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static ISupportModule_From Interface;
        internal static InteropInterface Interop;
        internal static GameObject obj = null;
        internal static SM_Component component = null;

        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;
            UnityMappers.RegisterMappers();

            Il2CppInteropRuntime runtime = Il2CppInteropRuntime.Create(new()
            {
                DetourProvider = new MelonDetourProvider(),
                UnityVersion = new Version(
                    InternalUtils.UnityInformationHandler.EngineVersion.Major,
                    InternalUtils.UnityInformationHandler.EngineVersion.Minor,
                    InternalUtils.UnityInformationHandler.EngineVersion.Build)
            }).AddLogger(new InteropLogger())
              .AddHarmonySupport();

            if (MelonLaunchOptions.Console.CleanUnityLogs)
                ConsoleCleaner();

            SceneHandler.Init();

            MonoEnumeratorWrapper.Register();

            ClassInjector.RegisterTypeInIl2Cpp<SM_Component>();
            SM_Component.Create();

            Interop = new InteropInterface();
            Interface.SetInteropSupportInterface(Interop);
            //HarmonyLib.Public.Patching.PatchManager.ResolvePatcher += HarmonyMethodPatcher.TryResolve;
            runtime.Start();

            return new SupportModule_To();
        }

        private static Assembly Il2Cppmscorlib = null;
        private static Type streamType = null;
        private static void ConsoleCleaner()
        {
            // Il2CppSystem.Console.SetOut(new Il2CppSystem.IO.StreamWriter(Il2CppSystem.IO.Stream.Null));
            try
            {
                Il2Cppmscorlib = Assembly.Load("Il2Cppmscorlib");
                if (Il2Cppmscorlib == null)
                    throw new Exception("Unable to Find Assembly Il2Cppmscorlib!");

                streamType = Il2Cppmscorlib.GetType("Il2CppSystem.IO.Stream");
                if (streamType == null)
                    throw new Exception("Unable to Find Type Il2CppSystem.IO.Stream!");

                PropertyInfo propertyInfo = streamType.GetProperty("Null", BindingFlags.Static | BindingFlags.Public);
                if (propertyInfo == null)
                    throw new Exception("Unable to Find Property Il2CppSystem.IO.Stream.Null!");

                MethodInfo nullStreamField = propertyInfo.GetGetMethod();
                if (nullStreamField == null)
                    throw new Exception("Unable to Find Get Method of Property Il2CppSystem.IO.Stream.Null!");

                object nullStream = nullStreamField.Invoke(null, new object[0]);
                if (nullStream == null)
                    throw new Exception("Unable to Get Value of Property Il2CppSystem.IO.Stream.Null!");

                Type streamWriterType = Il2Cppmscorlib.GetType("Il2CppSystem.IO.StreamWriter");
                if (streamWriterType == null)
                    throw new Exception("Unable to Find Type Il2CppSystem.IO.StreamWriter!");

                ConstructorInfo streamWriterCtor = streamWriterType.GetConstructor(new[] { streamType });
                if (streamWriterCtor == null)
                    throw new Exception("Unable to Find Constructor of Type Il2CppSystem.IO.StreamWriter!");

                object nullStreamWriter = streamWriterCtor.Invoke(new[] { nullStream });
                if (nullStreamWriter == null)
                    throw new Exception("Unable to Invoke Constructor of Type Il2CppSystem.IO.StreamWriter!");

                Type consoleType = Il2Cppmscorlib.GetType("Il2CppSystem.Console");
                if (consoleType == null)
                    throw new Exception("Unable to Find Type Il2CppSystem.Console!");

                MethodInfo setOutMethod = consoleType.GetMethod("SetOut", BindingFlags.Static | BindingFlags.Public);
                if (setOutMethod == null)
                    throw new Exception("Unable to Find Method Il2CppSystem.Console.SetOut!");

                setOutMethod.Invoke(null, new[] { nullStreamWriter });
            }
            catch (Exception ex) { MelonLogger.Warning($"Console Cleaner Failed: {ex}"); }
        }
    }

    internal sealed class MelonDetourProvider : IDetourProvider
    {
        public IDetour Create<TDelegate>(nint original, TDelegate target) where TDelegate : Delegate
        {
            return new MelonDetour(original, target);
        }

        private sealed class MelonDetour : IDetour
        {
            private nint _detourFrom;
            private nint _originalPtr;
            
            private Delegate _target;
            private IntPtr _targetPtr;

            /// <summary>
            /// Original method
            /// </summary>
            public nint Target => _detourFrom;

            public nint Detour => _targetPtr;

            public nint OriginalTrampoline => _originalPtr;
//             {
//                 get
//                 {
//                     MelonLogger.Msg($"Getting original trampoline => 0x{_originalPtr:X}");
//                     return _originalPtr;
//                 }
//             }
            
            public MelonDetour(nint detourFrom, Delegate target)
            {
                _detourFrom = detourFrom;
                _target = target;
                
                Apply(); //We have to apply immediately because we're gonna be asked for a trampoline right away
            }

            public unsafe void Apply()
            {
                if(_targetPtr != IntPtr.Zero)
                    // Already applied
                    return;

                _targetPtr = Marshal.GetFunctionPointerForDelegate(_target);
                
//                 MelonLogger.Msg($"About to detour 0x{_detourFrom:X} to 0x{_targetPtr:X} for method {_target.Method.Name}");

                var addr = _detourFrom;
                MelonUtils.NativeHookAttachDirect((nint) (&addr), _targetPtr);
                _originalPtr = addr;
                
//                 MelonLogger.Msg($"Applied detour from {_detourFrom:X} to {_targetPtr:X} for method {_target.Method.Name}, original is now: {_originalPtr:X})");
                
                NativeStackWalk.RegisterHookAddr((ulong)_targetPtr, $"Harmony Hook to {_target.Method.Name}");
            }

            public unsafe void Dispose()
            {
//                 MelonLogger.Msg($"Removing detour from 0x{_detourFrom:X} to 0x{_targetPtr:X} for method {_target.Method.Name}");
                var addr = _detourFrom;
                MelonUtils.NativeHookDetach((nint) (&addr), _targetPtr);
                _targetPtr = IntPtr.Zero;
//                 _originalPtr = IntPtr.Zero;
//                 MelonLogger.Msg($"Address after removing detour {_target.Method.Name}: {addr:X}");
            }

            public T GenerateTrampoline<T>() where T : Delegate
            {
//                 MelonLogger.Msg($"Getting delegate for original method at 0x{_originalPtr:X}, type: {typeof(T)}, method name {_target.Method.Name}");
                return Marshal.GetDelegateForFunctionPointer<T>(_originalPtr);
            }
        }
    }

    internal class InteropLogger : Microsoft.Extensions.Logging.ILogger {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            if (logLevel is LogLevel.Debug or LogLevel.Trace)
                MelonDebug.Msg(formatter(state, exception));
            else
                MelonLogger.Msg(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel switch {
            LogLevel.Debug or LogLevel.Trace => MelonDebug.IsEnabled(),
            _ => true
        };

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }
}
