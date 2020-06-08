using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace MelonLoader
{
    public interface ISupportModule
    {
        string GetUnityVersion();
        float GetUnityDeltaTime();
        int GetActiveSceneIndex();
        object StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(object coroutineToken);
        void UnityDebugLog(string msg);
    }

    internal static class SupportModule
    {
        private static Assembly assembly = null;
        private static Type type = null;
        internal static ISupportModule supportModule = null;

        internal static void Initialize()
        {
            try
            {
                string basedir = Path.Combine(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader"), "Dependencies"), "SupportModules");
                string filepath = null;
                if (Imports.IsIl2CppGame())
                    filepath = Path.Combine(basedir, "MelonLoader.Support.Il2Cpp.dll");
                else
                {
                    if (File.Exists(Path.Combine(Imports.GetAssemblyDirectory(), "UnityEngine.CoreModule.dll")))
                        filepath = Path.Combine(basedir, "MelonLoader.Support.Mono.dll");
                    else
                    {
                        if (IsOldUnity())
                            filepath = Path.Combine(basedir, "MelonLoader.Support.Mono.Pre2017.2.dll");
                        else
                            filepath = Path.Combine(basedir, "MelonLoader.Support.Mono.Pre2017.dll");
                    }
                }
                if (File.Exists(filepath))
                {
                    byte[] data = File.ReadAllBytes(filepath);
                    if (data.Length > 0)
                    {
                        assembly = Assembly.Load(data);
                        if (!assembly.Equals(null))
                        {
                            type = assembly.GetType("MelonLoader.Support.Main");
                            if (!type.Equals(null))
                            {
                                MethodInfo method = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);
                                if (!method.Equals(null))
                                    supportModule = (ISupportModule)method.Invoke(null, new object[0]);
                            }
                        }
                    }
                }
                else
                {
                    MelonModLogger.LogError("Unable to load Support Module! Support Module is Missing!");
                    MelonModLogger.Log("------------------------------");
                }
            }
            catch (Exception e)
            {
                MelonModLogger.LogError("Unable to load Support Module!\n" + e.ToString());
                MelonModLogger.Log("------------------------------");
            }
        }

        internal static bool IsOldUnity()
        {
            string unityVersion = Main.UnityVersion.Substring(0, Main.UnityVersion.LastIndexOf('.'));
            int unityVersion_major = 0;
            int unityVersion_minor = 0;
            return (int.TryParse(unityVersion.Substring(0, unityVersion.LastIndexOf('.')), out unityVersion_major)
                && (unityVersion_major <= 5)
                && int.TryParse(unityVersion.Substring(unityVersion.LastIndexOf('.') + 1), out unityVersion_minor)
                && (unityVersion_minor < 3)
                );
        }

        internal static string GetUnityVersion() => supportModule?.GetUnityVersion();
        internal static float GetUnityDeltaTime() => supportModule?.GetUnityDeltaTime() ?? 0f;
        internal static int GetActiveSceneIndex() => supportModule?.GetActiveSceneIndex() ?? -9;
        internal static void UnityDebugLog(string msg) => supportModule?.UnityDebugLog(msg);
    }
}
