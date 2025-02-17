using System;
using MelonLoader.Logging;
using MelonLoader.Utils;

namespace MelonLoader
{
    public static class MelonDebug
    {
        public static void Msg(object obj)
        {
            if (!IsEnabled())
                return;
            MelonLogger.PassLogMsg(MelonLogger.DefaultTextColor, obj.ToString(), ColorARGB.CornflowerBlue, "DEBUG");
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), obj.ToString());
        }

        public static void Msg(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.PassLogMsg(MelonLogger.DefaultTextColor, txt, ColorARGB.CornflowerBlue, "DEBUG");
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), txt);
        }

        public static void Msg(string txt, params object[] args)
        {
            if (!IsEnabled())
                return;
            MelonLogger.PassLogMsg(MelonLogger.DefaultTextColor, string.Format(txt, args), ColorARGB.CornflowerBlue, "DEBUG");
            MsgCallbackHandler?.Invoke(LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultTextColor), string.Format(txt, args));
        }

        public static void Error(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.PassLogError(txt, "DEBUG", false);
            ErrorCallbackHandler?.Invoke(txt);
        }

        public static event Action<ConsoleColor, string> MsgCallbackHandler;

        public static event Action<string> ErrorCallbackHandler;

        public static bool IsEnabled()
            => LoaderConfig.Current.Loader.DebugMode;
    }
}