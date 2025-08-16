#if !NET6_0_OR_GREATER
using System;
using System.Reflection;
using HarmonyLib;

namespace MelonLoader.InternalUtils;

public static class MonoCoreEntrypoint
{
    private static bool _monoCoreStartEntrypointAlreadyCalled;
    private static MethodInfo _monoCoreStartHookMethod;

    internal static void Init()
    {
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }
    
    // First step of the Mono Core.Start entrypoint: Harmony patch the main Unity assembly with a suitable hook
    private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        const string sceneManagerTypeName = "UnityEngine.SceneManagement.SceneManager";
        const string displayTypeName = "UnityEngine.Display";

        var assembly = args.LoadedAssembly;
        var assemblyName = assembly.GetName().Name;

        if (assemblyName is not ("UnityEngine.CoreModule" or "UnityEngine"))
            return;

        try
        {
            AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;

            var sceneManagerType = assembly.GetType(sceneManagerTypeName, false);
            if (sceneManagerType != null)
            {
                _monoCoreStartHookMethod = sceneManagerType.GetMethod("Internal_ActiveSceneChanged",
                    BindingFlags.NonPublic | BindingFlags.Static);
                Core.HarmonyInstance.Patch(_monoCoreStartHookMethod,
                    prefix: new HarmonyMethod(typeof(MonoCoreEntrypoint), nameof(Entrypoint)));
                MelonLogger.Msg($"Hooked into {_monoCoreStartHookMethod.FullDescription()}");
                return;
            }

            var displayType = assembly.GetType(displayTypeName, false);
            if (displayType != null)
            {
                _monoCoreStartHookMethod =
                    displayType.GetMethod("RecreateDisplayList", BindingFlags.NonPublic | BindingFlags.Static);
                Core.HarmonyInstance.Patch(_monoCoreStartHookMethod,
                    postfix: new HarmonyMethod(typeof(MonoCoreEntrypoint), nameof(Entrypoint)));
                MelonLogger.Msg($"Hooked into {_monoCoreStartHookMethod.FullDescription()}");
                return;
            }

            MelonLogger.Error(
                $"Couldn't find a suitable Core.Start entrypoint in the {assemblyName} assembly because " +
                $"{sceneManagerTypeName} or {displayTypeName} do not exist in the assembly");
        }
        catch (Exception e)
        {
            MelonLogger.Error($"Unexpected error occured when trying to hook into {assemblyName}: {e}");
        }
    }

    // Second step of the Mono Core.Start entrypoint: undo the Harmony patch and call the Core.Start method
    private static void Entrypoint()
    {
        if (_monoCoreStartEntrypointAlreadyCalled)
            return;

        _monoCoreStartEntrypointAlreadyCalled = true;
        try
        {
            Core.HarmonyInstance.Unpatch(_monoCoreStartHookMethod, HarmonyPatchType.All, Properties.BuildInfo.Name);
        }
        catch (Exception e)
        {
            MelonLogger.Error($"Unexpected error when trying to unhook the Core.Start entrypoint: {e}");
        }

        Core.Start();
    }
}
#endif
