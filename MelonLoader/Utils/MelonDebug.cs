using System;

namespace MelonLoader
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

        public static event Action<ConsoleColor, string> MsgCallbackHandler;
        public static event Action<string> ErrorCallbackHandler;
        //public static bool IsEnabled() => MelonLaunchOptions.Core.DebugMode;


        public static bool IsEnabled() 
        {
#if DEBUG
            return true;
#endif
#pragma warning disable CS0162 // Unreachable code detected
            return MelonLaunchOptions.Core.IsDebug;
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}