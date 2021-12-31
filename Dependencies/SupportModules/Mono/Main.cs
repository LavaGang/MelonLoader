using System;
using System.Reflection;
using MelonLoader.Support.Preferences;
using UnityEngine;

namespace MelonLoader.Support
{
    internal static class Main
    {
        internal static ISupportModule_From Interface = null;
        internal static GameObject obj = null;
        internal static SM_Component component = null;

        private static ISupportModule_To Initialize(ISupportModule_From interface_from)
        {
            Interface = interface_from;
            UnityMappers.RegisterMappers();

            if (IsUnity53OrLower())
                SM_Component.Create();
            else
                SceneHandler.Init();

            return new SupportModule_To();
        }

        private static bool IsUnity53OrLower()
        {
            try
            {
                Assembly unityengine = Assembly.Load("UnityEngine");
                if (unityengine == null)
                    return true;
                Type scenemanager = unityengine.GetType("UnityEngine.SceneManagement.SceneManager");
                if (scenemanager == null)
                    return true;
                EventInfo sceneLoaded = scenemanager.GetEvent("sceneLoaded");
                if (sceneLoaded == null)
                    return true;
                return false;
            }
            catch { return true; }
        }
    }
}