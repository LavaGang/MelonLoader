using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using MelonLoader.Utils;

namespace MelonLoader
{
    public static class MelonDebug
    {
        public static void Msg(object obj)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(Color.CornflowerBlue, MelonLogger.DefaultTextColor, "DEBUG", obj.ToString());
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), obj.ToString());
        }

        public static void Msg(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(Color.CornflowerBlue, MelonLogger.DefaultTextColor, "DEBUG", txt);
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), txt);
        }

        public static void Msg(string txt, params object[] args)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(Color.CornflowerBlue, MelonLogger.DefaultTextColor, "DEBUG", string.Format(txt, args));
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), string.Format(txt, args));
        }

        public static void Error(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Error("DEBUG", txt);
            ErrorCallbackHandler?.Invoke(txt);
        }

        public static event Action<ConsoleColor, string> MsgCallbackHandler;

        public static event Action<string> ErrorCallbackHandler;
        //public static bool IsEnabled() => MelonLaunchOptions.Core.DebugMode;

        public static bool IsEnabled()
        {
#if DEBUG
            return true;
#else
            return MelonLaunchOptions.Core.IsDebug;
#endif
        }
    }
}