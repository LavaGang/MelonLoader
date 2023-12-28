using System;
using System.Drawing;

namespace MelonLoader.Utils
{
    public static class MelonDebug
    {
        public static void Msg(object obj)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(Color.CornflowerBlue, MelonUtils.DefaultTextColor, "DEBUG", obj.ToString());
            MsgCallbackHandler?.Invoke(MelonUtils.DefaultTextConsoleColor, obj.ToString());
        }

        public static void Msg(string txt)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(Color.CornflowerBlue, MelonUtils.DefaultTextColor, "DEBUG", txt);
            MsgCallbackHandler?.Invoke(MelonUtils.DefaultTextConsoleColor, txt);
        }

        public static void Msg(string txt, params object[] args)
        {
            if (!IsEnabled())
                return;
            MelonLogger.Internal_Msg(Color.CornflowerBlue, MelonUtils.DefaultTextColor, "DEBUG", string.Format(txt, args));
            MsgCallbackHandler?.Invoke(MelonUtils.DefaultTextConsoleColor, string.Format(txt, args));
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