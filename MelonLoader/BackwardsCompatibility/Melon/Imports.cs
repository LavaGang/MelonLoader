using MelonLoader.Utils;
using System;

namespace MelonLoader;

[Obsolete("MelonLoader.Imports is Only Here for Compatibility Reasons. This will be removed in a future version.", true)]
public static class Imports
{
    [Obsolete("MelonLoader.Imports.GetCompanyName is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.UnityInformationHandler.GameDeveloper instead. This will be removed in a future version.", true)]
    public static string GetCompanyName() => InternalUtils.UnityInformationHandler.GameDeveloper;
    [Obsolete("MelonLoader.Imports.GetProductName is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.UnityInformationHandler.GameName instead. This will be removed in a future version.", true)]
    public static string GetProductName() => InternalUtils.UnityInformationHandler.GameName;
    [Obsolete("MelonLoader.Imports.GetGameDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.Utils.MelonEnvironment.GameRootDirectory instead. This will be removed in a future version.", true)]
    public static string GetGameDirectory() => MelonEnvironment.GameRootDirectory;
    [Obsolete("MelonLoader.Imports.GetGameDataDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.Utils.MelonEnvironment.UnityGameDataDirectory instead. This will be removed in a future version.", true)]
    public static string GetGameDataDirectory() => MelonEnvironment.UnityGameDataDirectory;
    [Obsolete("MelonLoader.Imports.GetAssemblyDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.Utils.MelonEnvironment.MelonManagedDirectory instead. This will be removed in a future version.", true)]
    public static string GetAssemblyDirectory() => MelonEnvironment.MelonManagedDirectory;
    [Obsolete("MelonLoader.Imports.IsIl2CppGame is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.IsGameIl2Cpp instead. This will be removed in a future version.", true)]
    public static bool IsIl2CppGame() => MelonUtils.IsGameIl2Cpp();
    [Obsolete("MelonLoader.Imports.IsDebugMode is Only Here for Compatibility Reasons. Please use MelonLoader.MelonDebug.IsEnabled instead. This will be removed in a future version.", true)]
    public static bool IsDebugMode() => MelonDebug.IsEnabled();
    [Obsolete("MelonLoader.Imports.Hook is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.NativeHookAttach instead. This will be removed in a future version.", true)]
    public static void Hook(IntPtr target, IntPtr detour) => MelonUtils.NativeHookAttach(target, detour);
    [Obsolete("MelonLoader.Imports.Unhook is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.NativeHookDetach instead. This will be removed in a future version.", true)]
    public static void Unhook(IntPtr target, IntPtr detour) => MelonUtils.NativeHookDetach(target, detour);
}