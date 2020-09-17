using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public static class MelonDebug
    {
        public static void Msg(string txt)
        {
            string namesection = MelonLogger.GetNameSection();
            Internal_Msg(namesection, txt);
            RunMsgCallbacks(namesection, txt);
        }
        public static void Msg(string txt, params object[] args)
        {
            string namesection = MelonLogger.GetNameSection();
            string fmt = string.Format(txt, args);
            Internal_Msg(namesection, fmt);
            RunMsgCallbacks(namesection, fmt);
        }
        public static void Msg(object obj)
        {
            string namesection = MelonLogger.GetNameSection();
            string objstr = obj.ToString();
            Internal_Msg(namesection, objstr);
            RunMsgCallbacks(namesection, objstr);
        }
        internal static void RunMsgCallbacks(string namesection, string msg) => MsgCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> MsgCallbackHandler;
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsEnabled();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Msg(string namesection, string txt);
    }
}