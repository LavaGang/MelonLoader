using MelonLoader.Utils;
using System;

namespace MelonLoader;

[Obsolete("MelonLoader.MelonLoaderBase is Only Here for Compatibility Reasons. This will be removed in a future version.", true)]
public static class MelonLoaderBase
{
    [Obsolete("MelonLoader.MelonLoaderBase.UserDataPath is Only Here for Compatibility Reasons. Please use MelonLoader.Utils.MelonEnvironment.UserDataDirectory instead. This will be removed in a future version.", true)]
    public static string UserDataPath { get => MelonEnvironment.UserDataDirectory; }
    [Obsolete("MelonLoader.MelonLoaderBase.UnityVersion is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion instead. This will be removed in a future version.", true)]
    public static string UnityVersion { get => InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(); }
}