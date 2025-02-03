using Il2CppInterop.Common;
using Il2CppInterop.HarmonySupport;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Startup;
using MelonLoader.Modules;
using System;
using System.Reflection;
using UnityEngine;

namespace MelonLoader.Engine.Unity.Il2Cpp
{
    internal class Il2CppSupportModule : MelonSupportModule
    {
        internal Il2CppSceneHandler sceneHandler;

        internal GameObject obj;
        internal Il2CppSupportComponent component;

        private Assembly Il2Cppmscorlib;
        private Type streamType;

        private Il2CppInteropRuntime runtime;

        public override void Initialize()
        {
            // Initialize Tomlet Unity Object Serialization
            Il2CppTomletProvider.Initialize();

            // Create Il2CppInterop Runtime
            runtime = Il2CppInteropRuntime.Create(new()
            {
                DetourProvider = new Il2CppDetourProvider(),
                UnityVersion = new Version(
                    UnityInformationHandler.EngineVersion.Major,
                    UnityInformationHandler.EngineVersion.Minor,
                    UnityInformationHandler.EngineVersion.Build)
            }).AddLogger(new Il2CppLogProvider())
              .AddHarmonySupport();

            //if (!LoaderConfig.Current.UnityEngine.DisableConsoleLogCleaner)
                ConsoleCleaner();

            // Register Custom Types
            ClassInjector.RegisterTypeInIl2Cpp<Il2CppSupportComponent>();
            Il2CppEnumeratorWrapper.Register();

            // Create Scene Handler
            sceneHandler = new(this);

            // Create GameObject and Component
            if (component == null)
                CreateGameObject();

            // Start Il2CppInterop Runtime
            runtime.Start();
        }

        internal void CreateGameObject()
        {
            // Create Support GameObject
            obj = new GameObject();
            GameObject.DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.DontSave;

            // Create Support Component
            component = obj.AddComponent(Il2CppType.Of<Il2CppSupportComponent>()).TryCast<Il2CppSupportComponent>();
            component.SiblingFix();

            // Create Interop for Coroutine Management
            MelonCoroutines.Interop = new Il2CppCoroutineInterop(component);
        }

        private void ConsoleCleaner()
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
}
