using System;
using System.Drawing;
using MelonLoader.Utils;

namespace MelonLoader
{
    public static class MelonDebug
    {
        public static void Msg(object obj)
        {
            if (!IsEnabled())
                return;
            MelonLogger.PassLogMsg(MelonLogger.DefaultTextColor, obj.ToString(), Color.CornflowerBlue, "DEBUG");
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), obj.ToString());
        }

        public static void Msg(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.PassLogMsg(MelonLogger.DefaultTextColor, txt, Color.CornflowerBlue, "DEBUG");
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), txt);
        }

        public static void Msg(string txt, params object[] args)
        {
            if (!IsEnabled())
                return;
            MelonLogger.PassLogMsg(MelonLogger.DefaultTextColor, string.Format(txt, args), Color.CornflowerBlue, "DEBUG");
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), string.Format(txt, args));
        }

        public static void Error(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.PassLogError(txt, "DEBUG");
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