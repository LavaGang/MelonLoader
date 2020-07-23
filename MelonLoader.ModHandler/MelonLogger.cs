using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public class MelonLogger
    {
        public static void Log(string s) => Native_Log((GetNameSection() + s));
        public static void Log(ConsoleColor color, string s) => Native_LogColor((GetNameSection() + s), color);
        public static void Log(string s, params object[] args) => Native_Log((GetNameSection() + string.Format(s, args)));
        public static void Log(ConsoleColor color, string s, params object[] args) => Native_LogColor((GetNameSection() + string.Format(s, args)), color);

        public static void LogWarning(string s) => Native_LogWarning(GetNameSection(), s);
        public static void LogWarning(string s, params object[] args) => Native_LogWarning(GetNameSection(), string.Format(s, args));

        public static void LogError(string s) => Native_LogError(GetNameSection(), s);
        public static void LogError(string s, params object[] args) => Native_LogError(GetNameSection(), string.Format(s, args));

        internal static void LogDLLError(string msg, string modname) => Native_LogDLLError((string.IsNullOrEmpty(modname) ? "" : ("[" + modname.Replace(" ", "_") + "] ")), msg);
        internal static void LogDLLStatus(MelonBase.MelonCompatibility type) => Native_LogDLLStatus(type);

        internal static string GetNameSection()
        {
            StackTrace st = new StackTrace(2, true);
            StackFrame sf = st.GetFrame(0);
            if (sf != null)
            {
                MethodBase method = sf.GetMethod();
                if (!method.Equals(null))
                {
                    Type methodClassType = method.DeclaringType;
                    if (!methodClassType.Equals(null))
                    {
                        Assembly asm = methodClassType.Assembly;
                        if (!asm.Equals(null))
                        {
                            MelonPlugin plugin = Main.Plugins.Find(x => (x.Assembly == asm));
                            if (plugin != null)
                            {
                                if (!string.IsNullOrEmpty(plugin.Info.Name))
                                    return "[" + plugin.Info.Name.Replace(" ", "_") + "] ";
                            }
                            else
                            {
                                MelonMod mod = Main.Mods.Find(x => (x.Assembly == asm));
                                if (mod != null)
                                {
                                    if (!string.IsNullOrEmpty(mod.Info.Name))
                                        return "[" + mod.Info.Name.Replace(" ", "_") + "] ";
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_Log(string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogColor(string txt, ConsoleColor color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogWarning(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogError(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogDLLError(string namesection, string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogDLLStatus(MelonBase.MelonCompatibility type);
    }
}