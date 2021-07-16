using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
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

            string game_version = MelonUtils.Application_Version;
            MelonLogger.Msg($"Game Version: {((game_version != null) ? game_version : "UNKNOWN")}");
            SetDefaultConsoleTitleWithGameName(game_version);

            UnityMappers.RegisterMappers();

            SM_Component.Create();
            return new SupportModule_To();
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameVersion = null);

        private static MethodInfo Application_get_version = null;
        private static string GetGameVersion()
        {
            if (Application_get_version == null)
            {
                Type app = typeof(Application);
                PropertyInfo verprop = app.GetProperty("version");
                if (verprop == null)
                    return null;
                Application_get_version = verprop.GetGetMethod();
            }
            if (Application_get_version == null)
                return null;
            return (string)Application_get_version.Invoke(null, new object[0]);
        }
    }
}