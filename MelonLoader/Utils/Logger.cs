using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public class MelonLogger
    {
        public static void Msg(string txt)
        {
            string namesection = GetNameSection();
            Internal_Msg(namesection, txt);
            RunMsgCallbacks(namesection, txt);
        }
        public static void Msg(string txt, params object[] args)
        {
            string namesection = GetNameSection();
            string fmt = string.Format(txt, args);
            Internal_Msg(namesection, fmt);
            RunMsgCallbacks(namesection, fmt);
        }
        public static void Msg(object obj)
        {
            string namesection = GetNameSection();
            string objstr = obj.ToString();
            Internal_Msg(namesection, objstr);
            RunMsgCallbacks(namesection, objstr);
        }

        public static void Warning(string txt)
        {
            string namesection = GetNameSection();
            Internal_Warning(namesection, txt);
            RunWarningCallbacks(namesection, txt);
        }
        public static void Warning(string txt, params object[] args)
        {
            string namesection = GetNameSection();
            string fmt = string.Format(txt, args);
            Internal_Warning(namesection, fmt);
            RunWarningCallbacks(namesection, fmt);
        }
        public static void Warning(object obj)
        {
            string namesection = GetNameSection();
            string objstr = obj.ToString();
            Internal_Warning(namesection, objstr);
            RunWarningCallbacks(namesection, objstr);
        }

        public static void Error(string txt)
        {
            string namesection = GetNameSection();
            Internal_Error(namesection, txt);
            RunErrorCallbacks(namesection, txt);
        }
        public static void Error(string txt, params object[] args)
        {
            string namesection = GetNameSection();
            string fmt = string.Format(txt, args);
            Internal_Error(namesection, fmt);
            RunErrorCallbacks(namesection, fmt);
        }
        public static void Error(object obj)
        {
            string namesection = GetNameSection();
            string objstr = obj.ToString();
            Internal_Error(namesection, objstr);
            RunErrorCallbacks(namesection, objstr);
        }

        internal static string GetNameSection()
        {
            StackTrace st = new StackTrace(2, true);
            StackFrame sf = st.GetFrame(0);
            if (sf == null)
                return null;
            MethodBase method = sf.GetMethod();
            if (method == null)
                return null;
            Type methodClassType = method.DeclaringType;
            if (methodClassType == null)
                return null;
            Assembly asm = methodClassType.Assembly;
            if (asm == null)
                return null;
            MelonBase melon = MelonHandler.Plugins.Find(x => (x.Assembly == asm));
            if (melon == null)
                melon = MelonHandler.Mods.Find(x => (x.Assembly == asm));
            if (melon == null)
                return null;
            return melon.Info.Name.Replace(" ", "_");
        }

        internal static void RunMsgCallbacks(string namesection, string msg) => MsgCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> MsgCallbackHandler;
        internal static void RunWarningCallbacks(string namesection, string msg) => WarningCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> WarningCallbackHandler;
        internal static void RunErrorCallbacks(string namesection, string msg) => ErrorCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> ErrorCallbackHandler;

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Msg(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Warning(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Error(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void ThrowInternalFailure(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void WriteSpacer();
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(string txt) => Msg(txt);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(string txt, params object[] args) => Msg(txt, args);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(object obj) => Msg(obj);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, string txt) => Msg(txt);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, string txt, params object[] args) => Msg(txt, args);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, object obj) => Msg(obj);

        [Obsolete("LogWarning is obsolete. Please use Warning instead.")]
        public static void LogWarning(string txt) => Warning(txt);
        [Obsolete("LogWarning is obsolete. Please use Warning instead.")]
        public static void LogWarning(string txt, params object[] args) => Warning(txt, args);
        [Obsolete("LogError is obsolete. Please use Error instead.")]
        public static void LogError(string txt) => Error(txt);
        [Obsolete("LogError is obsolete. Please use Error instead.")]
        public static void LogError(string txt, params object[] args) => Error(txt, args);
    }
}