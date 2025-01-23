using System;
using System.Collections.Generic;
using MelonLoader.Utils;

namespace MelonLoader
{
    [Obsolete("MelonLoader.Main is Only Here for Compatibility Reasons. This will be removed in a future update.", true)]
    public static class Main
    {
        [Obsolete("MelonLoader.Main.Mods is Only Here for Compatibility Reasons. Please use MelonLoader.MelonHandler.Mods instead. This will be removed in a future update.", true)]
        public static List<MelonMod> Mods = null;
        [Obsolete("MelonLoader.Main.Plugins is Only Here for Compatibility Reasons. Please use MelonLoader.MelonHandler.Plugins instead. This will be removed in a future update.", true)]
        public static List<MelonPlugin> Plugins = null;
        [Obsolete("MelonLoader.Main.IsBoneworks is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.IsBONEWORKS instead. This will be removed in a future update.", true)]
        public static bool IsBoneworks = false;
        [Obsolete("MelonLoader.Main.GetUnityVersion is Only Here for Compatibility Reasons. Please use  MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion instead. This will be removed in a future update.", true)]
        public static string GetUnityVersion() => InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType();
        [Obsolete("MelonLoader.Main.GetUserDataPath is Only Here for Compatibility Reasons. Please use MelonLoader.Utils.MelonEnvironment.UserDataDirectory instead. This will be removed in a future update.", true)]
        public static string GetUserDataPath() => MelonEnvironment.UserDataDirectory;
    }
}