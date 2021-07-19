using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    internal static class GameVersionHandler
    {
        private static MethodInfo Application_get_version = null;
        private static MethodInfo Application_get_buildGUID = null;

        internal static void Setup()
        {
            Assembly assembly = Assembly.Load(
                File.Exists(Path.Combine(MelonUtils.GetManagedDirectory(), "UnityEngine.CoreModule.dll"))
                ? "UnityEngine.CoreModule"
                : "UnityEngine");
            if (assembly != null)
            {
                Type applicationType = assembly.GetType("UnityEngine.Application");
                if (applicationType != null)
                {
                    PropertyInfo versionProp = applicationType.GetProperty("version");
                    if (versionProp != null)
                        Application_get_version = versionProp.GetGetMethod();

                    PropertyInfo buildGUIDProp = applicationType.GetProperty("buildGUID");
                    if (buildGUIDProp != null)
                        Application_get_buildGUID = buildGUIDProp.GetGetMethod();
                }
            }

            string game_version = "0";
            MethodInfoCheck(Application_get_version, ref game_version);
            MethodInfoCheck(Application_get_buildGUID, ref game_version);
            Version = game_version;

            MelonLogger.Msg($"Game Version: {game_version}");
            SetDefaultConsoleTitleWithGameName(game_version);
        }

        internal static string Version { get; private set; } = "0";

        private static void MethodInfoCheck(MethodInfo method, ref string game_version)
        {
            if (!string.IsNullOrEmpty(game_version) && !game_version.Equals("0"))
                return;
            game_version = (method != null)
                ? (string)method.Invoke(null, new object[0])
                : null;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameVersion = null);
    }
}
