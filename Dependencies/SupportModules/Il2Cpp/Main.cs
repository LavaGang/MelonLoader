using System;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader.Support.Preferences;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static ISupportModule_From Interface;
        internal static UnhollowerInterface unhollower;
        internal static GameObject obj = null;
        internal static SM_Component component = null;

        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;
            UnityMappers.RegisterMappers();

            LogSupport.RemoveAllHandlers();
            if (MelonDebug.IsEnabled())
                LogSupport.InfoHandler += MelonLogger.Msg;
            LogSupport.WarningHandler += MelonLogger.Warning;
            LogSupport.ErrorHandler += MelonLogger.Error;
            if (MelonDebug.IsEnabled())
                LogSupport.TraceHandler += MelonLogger.Msg;

            ClassInjector.Detour = new UnhollowerDetour();
            UnityVersionHandler.Initialize(
                InternalUtils.UnityInformationHandler.EngineVersion.Major,
                InternalUtils.UnityInformationHandler.EngineVersion.Minor,
                InternalUtils.UnityInformationHandler.EngineVersion.Build);

            if (MelonLaunchOptions.Console.CleanUnityLogs)
                ConsoleCleaner();

            SceneHandler.Init();

            MonoEnumeratorWrapper.Register();

            ClassInjector.RegisterTypeInIl2Cpp<SM_Component>();
            SM_Component.Create();

            unhollower = new UnhollowerInterface();
            Interface.SetUnhollowerSupportInterface(unhollower);

            HarmonyLib.Public.Patching.PatchManager.ResolvePatcher += HarmonyMethodPatcher.TryResolve;

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

    internal class UnhollowerDetour : IManagedDetour
    {
        private static readonly List<object> PinnedDelegates = new List<object>();

        unsafe public T Detour<T>(IntPtr @from, T to) where T : Delegate
        {
            IntPtr* targetVarPointer = &from;
            PinnedDelegates.Add(to);
            MelonUtils.NativeHookAttach((IntPtr)targetVarPointer, to.GetFunctionPointer());
            return from.GetDelegate<T>();
        }
    }
}