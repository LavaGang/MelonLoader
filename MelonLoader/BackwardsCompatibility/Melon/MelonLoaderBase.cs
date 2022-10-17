using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.MelonLoaderBase is Only Here for Compatibility Reasons.")]
    public static class MelonLoaderBase
    {
        [Obsolete("MelonLoader.MelonLoaderBase.UserDataPath is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.GetUserDataDirectory instead.")]
        public static string UserDataPath { get => MelonUtils.UserDataDirectory; }
        [Obsolete("MelonLoader.MelonLoaderBase.UnityVersion is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion instead.")]
        public static string UnityVersion { get => InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(); }
    }
}