using System;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public static class MelonDebug
    {
        public static void Msg(string txt)
        {
            if (!External.Debug.IsEnabled())
                return;
            ConsoleColor color = MelonLogger.DefaultMelonColor;
            string namesection = null;
            MelonBase melon = MelonLogger.GetMelonFromStackTrace();
            if (melon != null)
            {
                namesection = melon.Info.Name.Replace(" ", "_");
                if (melon.Color != null)
                    color = melon.Color.Color;
            }
            External.Logger.Internal_Msg(color, namesection, txt);
            RunMsgCallbacks(color, namesection, txt);
        }
        public static void Msg(string txt, params object[] args)
        {
            if (!External.Debug.IsEnabled())
                return;
            ConsoleColor color = MelonLogger.DefaultMelonColor;
            string namesection = null;
            MelonBase melon = MelonLogger.GetMelonFromStackTrace();
            if (melon != null)
            {
                namesection = melon.Info.Name.Replace(" ", "_");
                if (melon.Color != null)
                    color = melon.Color.Color;
            }
            string fmt = string.Format(txt, args);
            External.Logger.Internal_Msg(color, namesection, fmt);
            RunMsgCallbacks(color, namesection, fmt);
        }
        public static void Msg(object obj)
        {
            if (!External.Debug.IsEnabled())
                return;
            ConsoleColor color = MelonLogger.DefaultMelonColor;
            string namesection = null;
            MelonBase melon = MelonLogger.GetMelonFromStackTrace();
            if (melon != null)
            {
                namesection = melon.Info.Name.Replace(" ", "_");
                if (melon.Color != null)
                    color = melon.Color.Color;
            }
            string objstr = obj.ToString();
            External.Logger.Internal_Msg(color, namesection, objstr);
            RunMsgCallbacks(color, namesection, objstr);
        }
        internal static void RunMsgCallbacks(ConsoleColor color, string namesection, string msg) => MsgCallbackHandler?.Invoke(color, namesection, msg);
        public static event Action<ConsoleColor, string, string> MsgCallbackHandler;
    }
}
