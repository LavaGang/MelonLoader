using System;

namespace MelonLoader
{
    [Obsolete("MelonLoader.Imports is Only Here for Compatibility Reasons.")]
    public static class Imports
    {
        [Obsolete("MelonLoader.Imports.GetCompanyName is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.UnityInformationHandler.GameDeveloper instead.")]
        public static string GetCompanyName() => InternalUtils.UnityInformationHandler.GameDeveloper;
        [Obsolete("MelonLoader.Imports.GetProductName is Only Here for Compatibility Reasons. Please use MelonLoader.InternalUtils.UnityInformationHandler.GameName instead.")]
        public static string GetProductName() => InternalUtils.UnityInformationHandler.GameName;
        [Obsolete("MelonLoader.Imports.GetGameDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.GetGameDirectory instead.")]
        public static string GetGameDirectory() => MelonUtils.GameDirectory;
        [Obsolete("MelonLoader.Imports.GetGameDataDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.GetGameDataDirectory instead.")]
        public static string GetGameDataDirectory() => MelonUtils.GetGameDataDirectory();
        [Obsolete("MelonLoader.Imports.GetAssemblyDirectory is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.GetManagedDirectory instead.")]
        public static string GetAssemblyDirectory() => MelonUtils.GetManagedDirectory();
        [Obsolete("MelonLoader.Imports.IsIl2CppGame is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.IsGameIl2Cpp instead.")]
        public static bool IsIl2CppGame() => MelonUtils.IsGameIl2Cpp();
        [Obsolete("MelonLoader.Imports.IsDebugMode is Only Here for Compatibility Reasons. Please use MelonLoader.MelonDebug.IsEnabled instead.")]
        public static bool IsDebugMode() => MelonDebug.IsEnabled();
        [Obsolete("MelonLoader.Imports.Hook is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.NativeHookAttach instead.")]
        public static void Hook(IntPtr target, IntPtr detour) => MelonUtils.NativeHookAttach(target, detour);
        [Obsolete("MelonLoader.Imports.Unhook is Only Here for Compatibility Reasons. Please use MelonLoader.MelonUtils.NativeHookDetach instead.")]
        public static void Unhook(IntPtr target, IntPtr detour) => MelonUtils.NativeHookDetach(target, detour);
    }
}