using System;
using System.IO;
using System.Reflection;

namespace MelonLoader
{
    internal static class SupportModule
    {
        private static Assembly assembly = null;
        private static Type type = null;
        internal static ISupportModule supportModule = null; 

        internal static void Initialize()
        {
            try
            {
                string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MelonLoader", (Imports.IsIl2CppGame() ? "MelonLoader.Support.Il2Cpp.dll" : "MelonLoader.Support.Mono.dll"));
                if (File.Exists(filepath))
                {
                    byte[] data = File.ReadAllBytes(filepath);
                    if (data.Length > 0)
                    {
                        assembly = Assembly.Load(data);
                        if (!assembly.Equals(null))
                        {
                            type = assembly.GetType("MelonLoader.SupportModule.Main");
                            if (!type.Equals(null))
                            {
                                MethodInfo method = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);
                                if (!method.Equals(null))
                                    supportModule = (ISupportModule) method.Invoke(null, new object[0]);
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

        internal static float GetUnityDeltaTime() => supportModule?.GetUnityDeltaTime() ?? 0f;

        internal static int GetActiveSceneIndex() => supportModule?.GetActiveSceneIndex() ?? -9;
    }
}
