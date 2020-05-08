using System;
using System.IO;
using System.Reflection;

namespace MelonLoader
{
    internal static class SupportModule
    {
        private static Assembly assembly = null;
        private static Type type = null;
        private static MethodInfo getUnityDeltaTime = null;
        private static MethodInfo getComponent = null;
        private static MethodInfo getMonoBehaviourType = null;
        private static MethodInfo getCoroutineType = null;
        private static MethodInfo getWaitForSecondsType = null;
        private static MethodInfo getCustomYieldInstructionType = null;
        private static MethodInfo getWaitForFixedUpdateType = null;
        private static MethodInfo getWaitForEndOfFrameType = null;
        private static MethodInfo getActiveScene = null;
        private static MethodInfo getActiveSceneIndex = null;

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
                                {
                                    method.Invoke(null, new object[0]);
                                    getUnityDeltaTime = type.GetMethod("GetUnityDeltaTime", BindingFlags.NonPublic | BindingFlags.Static);
                                    getComponent = type.GetMethod("GetComponent", BindingFlags.NonPublic | BindingFlags.Static);
                                    getMonoBehaviourType = type.GetMethod("GetMonoBehaviourType", BindingFlags.NonPublic | BindingFlags.Static);
                                    getCoroutineType = type.GetMethod("GetCoroutineType", BindingFlags.NonPublic | BindingFlags.Static);
                                    getWaitForSecondsType = type.GetMethod("GetWaitForSecondsType", BindingFlags.NonPublic | BindingFlags.Static);
                                    getCustomYieldInstructionType = type.GetMethod("GetCustomYieldInstructionType", BindingFlags.NonPublic | BindingFlags.Static);
                                    getWaitForFixedUpdateType = type.GetMethod("GetCustomYieldInstructionType", BindingFlags.NonPublic | BindingFlags.Static);
                                    getWaitForEndOfFrameType = type.GetMethod("GetWaitForEndOfFrameType", BindingFlags.NonPublic | BindingFlags.Static);
                                    getActiveScene = type.GetMethod("GetActiveScene", BindingFlags.NonPublic | BindingFlags.Static);
                                    getActiveSceneIndex = type.GetMethod("GetActiveSceneIndex", BindingFlags.NonPublic | BindingFlags.Static);
                                }
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

        internal static float GetUnityDeltaTime()
        {
            if (!getUnityDeltaTime.Equals(null))
                return (float)getUnityDeltaTime.Invoke(null, new object[0]);
            return 0f;
        }

        internal static object GetComponent()
        {
            if (!getComponent.Equals(null))
                return getComponent.Invoke(null, new object[0]);
            return null;
        }

        internal static Type GetMonoBehaviourType()
        {
            if (!getMonoBehaviourType.Equals(null))
                return (Type)getMonoBehaviourType.Invoke(null, new object[0]);
            return null;
        }

        internal static Type GetCoroutineType()
        {
            if (!getCoroutineType.Equals(null))
                return (Type)getCoroutineType.Invoke(null, new object[0]);
            return null;
        }

        internal static Type GetWaitForSecondsType()
        {
            if (!getWaitForSecondsType.Equals(null))
                return (Type)getWaitForSecondsType.Invoke(null, new object[0]);
            return null;
        }

        internal static Type GetCustomYieldInstructionType()
        {
            if (!getCustomYieldInstructionType.Equals(null))
                return (Type)getCustomYieldInstructionType.Invoke(null, new object[0]);
            return null;
        }

        internal static Type GetWaitForFixedUpdateType()
        {
            if (!getWaitForFixedUpdateType.Equals(null))
                return (Type)getWaitForFixedUpdateType.Invoke(null, new object[0]);
            return null;
        }

        internal static Type GetWaitForEndOfFrameType()
        {
            if (!getWaitForEndOfFrameType.Equals(null))
                return (Type)getWaitForEndOfFrameType.Invoke(null, new object[0]);
            return null;
        }

        internal static object GetActiveScene()
        {
            if (!getActiveScene.Equals(null))
                return getActiveScene.Invoke(null, new object[0]);
            return null;
        }

        internal static int GetActiveSceneIndex()
        {
            if (!getActiveSceneIndex.Equals(null))
                return (int)getActiveSceneIndex.Invoke(null, new object[0]);
            return -9;
        }
    }
}
