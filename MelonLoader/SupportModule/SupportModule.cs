using System;
using System.IO;
using System.Reflection;
using MelonLoader.Utils;

namespace MelonLoader.SupportModule
{
    internal static class SupportModule
    {
        internal static ISupportModule_To Interface;

        internal static bool Initialize()
        {
            MelonLogger.Msg("Loading Support Module...");
            string BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MelonUtils.BaseDirectory, "MelonLoader"), "Dependencies"), "SupportModules");
            if (!Directory.Exists(BaseDirectory))
            {
                MelonLogger.Error("Failed to Find SupportModules Directory!");
                return false;
            }
            string ModuleName = MelonUtils.IsGameIl2Cpp()
                ? "Il2Cpp.dll"
                : File.Exists(Path.Combine(MelonUtils.GetManagedDirectory(), "UnityEngine.CoreModule.dll"))
                    ? "Mono.dll"
                    : IsOldUnity() 
                        ? "Mono.Pre-5.dll"
                        : "Mono.Pre-2017.dll";
            string ModulePath = Path.Combine(BaseDirectory, ModuleName);
            if (!File.Exists(ModulePath))
            {
                MelonLogger.Error("Failed to Find Support Module " + ModuleName + "!");
                return false;
            }
            try
            {
                Assembly assembly = Assembly.LoadFrom(ModulePath);
                if (assembly == null)
                {
                    MelonLogger.Error("Failed to Load Assembly from " + ModuleName + "!");
                    return false;
                }
                Type type = assembly.GetType("MelonLoader.Support.Main");
                if (type == null)
                {
                    MelonLogger.Error("Failed to Get Type MelonLoader.Support.Main!");
                    return false;
                }
                MethodInfo method = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);
                if (method == null)
                {
                    MelonLogger.Error("Failed to Get Method Initialize!");
                    return false;
                }
                Interface = (ISupportModule_To)method.Invoke(null, new object[] { new SupportModule_From() });
                if (Interface == null)
                {
                    MelonLogger.Error("Failed to Initialize Interface!");
                    return false;
                }
            }
            catch(Exception ex) { MelonLogger.Error(ex.ToString()); return false; }
            return true;
        }

        private static bool IsOldUnity()
        {
            try
            {
                Assembly unityengine = Assembly.Load("UnityEngine");
                Type scenemanager = unityengine?.GetType("UnityEngine.SceneManagement.SceneManager");
                EventInfo sceneLoaded = scenemanager?.GetEvent("sceneLoaded");
                if (sceneLoaded == null)
                    return true;
                return false;
            }
            catch { return true; }
        }
    }
}