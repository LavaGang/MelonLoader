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
            ConsoleColor color = ConsoleColor.Magenta;
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
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
            ConsoleColor color = ConsoleColor.Magenta;
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
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
            ConsoleColor color = ConsoleColor.Magenta;
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
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

        public static void Warning(string txt)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            Internal_Warning(namesection, txt);
            RunWarningCallbacks(namesection, txt);
        }
        public static void Warning(string txt, params object[] args)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            string fmt = string.Format(txt, args);
            Internal_Warning(namesection, fmt);
            RunWarningCallbacks(namesection, fmt);
        }
        public static void Warning(object obj)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            string objstr = obj.ToString();
            Internal_Warning(namesection, objstr);
            RunWarningCallbacks(namesection, objstr);
        }

        public static void Error(string txt)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            Internal_Error(namesection, txt);
            RunErrorCallbacks(namesection, txt);
        }
        public static void Error(string txt, params object[] args)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            string fmt = string.Format(txt, args);
            Internal_Error(namesection, fmt);
            RunErrorCallbacks(namesection, fmt);
        }
        public static void Error(object obj)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            string objstr = obj.ToString();
            Internal_Error(namesection, objstr);
            RunErrorCallbacks(namesection, objstr);
        }

        internal static MelonBase GetMelonFromStackTrace()
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
            return melon;
        }

        internal static void RunMsgCallbacks(ConsoleColor color, string namesection, string msg) => MsgCallbackHandler?.Invoke(color, namesection, msg);
        public static event Action<ConsoleColor, string, string> MsgCallbackHandler;
        internal static void RunWarningCallbacks(string namesection, string msg) => WarningCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> WarningCallbackHandler;
        internal static void RunErrorCallbacks(string namesection, string msg) => ErrorCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> ErrorCallbackHandler;
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Msg(ConsoleColor color, string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Warning(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Internal_Error(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void ThrowInternalFailure(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void WriteSpacer();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_PrintModName(ConsoleColor color, string name, string version);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Flush();
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