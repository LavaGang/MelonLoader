using System;
using MelonLoader.Lemons;

namespace MelonLoader.Utils
{
    public static class MelonDebug
    {
        public static void Msg(object obj)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(ConsoleColor.Blue, MelonLogger.DefaultTextColor, "DEBUG", obj.ToString());
            MsgCallbackHandler?.Invoke(MelonLogger.DefaultTextColor, obj.ToString());
        }
        public static void Msg(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(ConsoleColor.Blue, MelonLogger.DefaultTextColor, "DEBUG", txt);
            MsgCallbackHandler?.Invoke(MelonLogger.DefaultTextColor, txt);
        }
        public static void Msg(string txt, params object[] args)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(ConsoleColor.Blue, MelonLogger.DefaultTextColor, "DEBUG", string.Format(txt, args));
            MsgCallbackHandler?.Invoke(MelonLogger.DefaultTextColor, string.Format(txt, args));
        }

        public static void Error(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Error("DEBUG", txt);
            ErrorCallbackHandler?.Invoke(txt);
        }

        public static event LemonAction<ConsoleColor, string> MsgCallbackHandler;
        public static event LemonAction<string> ErrorCallbackHandler;
        public static bool IsEnabled() => MelonLaunchOptions.Core.DebugMode;
    }
}