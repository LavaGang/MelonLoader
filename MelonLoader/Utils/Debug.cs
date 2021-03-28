#if PORT_DISABLE
using System;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public static class MelonDebug
    {
        public static void Msg(string txt)
        {
            if (!IsEnabled())
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
            Internal_Msg(color, namesection, txt);
            RunMsgCallbacks(color, namesection, txt);
        }
        public static void Msg(string txt, params object[] args)
        {
            if (!IsEnabled())
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
            Internal_Msg(color, namesection, fmt);
            RunMsgCallbacks(color, namesection, fmt);
        }
        public static void Msg(object obj)
        {
            if (!IsEnabled())
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
            Internal_Msg(color, namesection, objstr);
            RunMsgCallbacks(color, namesection, objstr);
        }
        internal static void RunMsgCallbacks(ConsoleColor color, string namesection, string msg) => MsgCallbackHandler?.Invoke(color, namesection, msg);
        public static event Action<ConsoleColor, string, string> MsgCallbackHandler;
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsEnabled();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Msg(ConsoleColor color, string namesection, string txt);
    }
}
#endif