using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public class MelonLogger
    {
        internal static ConsoleColor DefaultMelonColor = ConsoleColor.Cyan;

        public static void Msg(string txt)
        {
            ConsoleColor color = DefaultMelonColor;
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
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
            ConsoleColor color = DefaultMelonColor;
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
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
            ConsoleColor color = DefaultMelonColor;
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
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

        public static void Warning(string txt)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name;
            ManualWarning(namesection, txt);
        }
        public static void Warning(string txt, params object[] args)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name;
            string fmt = string.Format(txt, args);
            ManualWarning(namesection, fmt);
        }
        public static void Warning(object obj)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name;
            string objstr = obj.ToString();
            ManualWarning(namesection, objstr);
        }
        internal static void ManualWarning(string namesection, string txt) {
            namesection = namesection?.Replace(" ", "_");
            External.Logger.Internal_Warning(namesection, txt);
            RunWarningCallbacks(namesection, txt);
        }

        public static void Error(string txt)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            External.Logger.Internal_Error(namesection, txt);
            RunErrorCallbacks(namesection, txt);
        }
        public static void Error(string txt, params object[] args)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            string fmt = string.Format(txt, args);
            External.Logger.Internal_Error(namesection, fmt);
            RunErrorCallbacks(namesection, fmt);
        }
        public static void Error(object obj)
        {
            string namesection = null;
            MelonBase melon = GetMelonFromStackTrace();
            if (melon != null)
                namesection = melon.Info.Name.Replace(" ", "_");
            string objstr = obj.ToString();
            External.Logger.Internal_Error(namesection, objstr);
            RunErrorCallbacks(namesection, objstr);
        }

        internal static MelonBase GetMelonFromStackTrace()
        {
            //throw new NotImplementedException("Not Ported");
            return null;
#if PORT_DISABLE
            StackTrace st = new StackTrace(2, true);
            if (st.FrameCount <= 0)
                return null;
            MelonBase output = CheckForMelonInFrame(st);
            if (output == null)
                output = CheckForMelonInFrame(st, 1);
            return output;
#endif
        }

#if PORT_DISABLE
        private static MelonBase CheckForMelonInFrame(StackTrace st, int frame = 0)
        {
            StackFrame sf = st.GetFrame(frame);
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
#endif

        internal static void RunMsgCallbacks(ConsoleColor color, string namesection, string msg) => MsgCallbackHandler?.Invoke(color, namesection, msg);
        public static event Action<ConsoleColor, string, string> MsgCallbackHandler;
        internal static void RunWarningCallbacks(string namesection, string msg) => WarningCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> WarningCallbackHandler;
        internal static void RunErrorCallbacks(string namesection, string msg) => ErrorCallbackHandler?.Invoke(namesection, msg);
        public static event Action<string, string> ErrorCallbackHandler;

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
