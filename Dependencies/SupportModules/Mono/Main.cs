using MelonLoader.Modules;
using MelonLoader.Support.Preferences;
using System.Reflection;
using UnityEngine;

[assembly: MelonLoader.PatchShield]

namespace MelonLoader.Support;

internal static class Main
{
    internal static ISupportModuleFrom Interface = null;
    internal static GameObject obj = null;
    internal static SM_Component component = null;

    private static ISupportModuleTo Initialize(ISupportModuleFrom interface_from)
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
            var unityengine = Assembly.Load("UnityEngine");
            if (unityengine == null)
                return true;
            var scenemanager = unityengine.GetType("UnityEngine.SceneManagement.SceneManager");
            if (scenemanager == null)
                return true;
            var sceneLoaded = scenemanager.GetEvent("sceneLoaded");
            return sceneLoaded == null;
        }
        catch
        {
            return true;
        }
    }
}