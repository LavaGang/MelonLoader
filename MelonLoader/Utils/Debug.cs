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
            SendMsg(MelonLogger.DefaultTextColor, txt);
        }
        public static void Msg(string txt, params object[] args)
        {
            if (!IsEnabled())
                return;
            SendMsg(MelonLogger.DefaultTextColor, string.Format(txt, args));
        }
        public static void Msg(object obj)
        {
            if (!IsEnabled())
                return;
            SendMsg(MelonLogger.DefaultTextColor, obj.ToString());
        }

        private static void SendMsg(ConsoleColor msgcolor, string msg)
        {
            ConsoleColor meloncolor = MelonLogger.DefaultMelonColor;
            string namesection = null;
            MelonBase melon = MelonLogger.GetMelonFromStackTrace();
            if (melon != null)
            {
                namesection = melon.Info.Name.Replace(" ", "_");
                msgcolor = melon.ConsoleColor;
            }
            Internal_Msg(meloncolor, msgcolor, namesection, msg);
            MsgCallbackHandler?.Invoke(meloncolor, msgcolor, namesection, msg);
        }

        public static event Action<ConsoleColor, ConsoleColor, string, string> MsgCallbackHandler;
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsEnabled();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Msg(ConsoleColor meloncolor, ConsoleColor msgcolor, string namesection, string txt);
    }
}