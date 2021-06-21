using System;
using System.Diagnostics;
using MelonLoader;

namespace ModHelper
{
    public static class ModLogger
    {
        [Obsolete("ModHelper.ModLogger.Debug is Only Here for Compatibility Reasons. Please use MelonLogger.Msg instead.")]
        public static void Debug(object obj)
        {
            var frame = new StackTrace().GetFrame(1);
            var className = frame.GetMethod().ReflectedType.Name;
            var methodName = frame.GetMethod().Name;
            AddLog(className, methodName, obj);
        }

        [Obsolete("ModHelper.ModLogger.AddLog is Only Here for Compatibility Reasons. Please use MelonLogger.Msg instead.")]
        public static void AddLog(string className, string methodName, object obj)
        {
            MelonLogger.Msg($"[{className}:{methodName}]: {obj}");
        }
    }
}