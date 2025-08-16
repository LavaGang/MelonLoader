#if NET6_0_OR_GREATER
using System;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.HarmonySupport;

namespace MelonLoader.Fixes.Il2CppInterop
{
    internal class Il2CppInteropExceptionLog
    {
        private static MelonLogger.Instance _logger = new("Il2CppInterop");

        private static MethodInfo _reportException;
        private static MethodInfo _reportException_Prefix;

        private static void LogMsg(string msg)
            => _logger.Msg(msg);
        private static void LogError(Exception ex)
            => _logger.Error(ex);
        private static void LogError(string msg, Exception ex)
            => _logger.Error(msg, ex);
        private static void LogDebugMsg(string msg)
        {
            if (!MelonDebug.IsEnabled())
                return;
            _logger.Msg(msg);
        }

        internal static void Install()
        {
            try
            {
                Type thisType = typeof(Il2CppInteropExceptionLog);
                Type harmonySupportType = typeof(HarmonySupport);

                Type detourMethodPatcherType = harmonySupportType.Assembly.GetType("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher");
                if (detourMethodPatcherType == null)
                    throw new Exception("Failed to get Il2CppDetourMethodPatcher");

                _reportException = detourMethodPatcherType.GetMethod("ReportException",
                    BindingFlags.NonPublic | BindingFlags.Static);
                if (_reportException == null)
                    throw new Exception("Failed to get Il2CppDetourMethodPatcher.ReportException");

                _reportException_Prefix = thisType.GetMethod(nameof(ReportException_Prefix), BindingFlags.NonPublic | BindingFlags.Static);

                LogDebugMsg("Patching Il2CppInterop Il2CppDetourMethodPatcher.ReportException...");
                Core.HarmonyInstance.Patch(_reportException,
                    new HarmonyMethod(_reportException_Prefix));
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        private static bool ReportException_Prefix(Exception __0)
        {
            LogError("During invoking native->managed trampoline", __0);
            return false;
        }
    }
}
#endif