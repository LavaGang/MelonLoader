using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public class MelonLogger
    {
        public static void Log(string s)
        {
            string namesection = GetNameSection();
            Native_Log(namesection, s);
            Console.RunLogCallbacks(namesection, s);
        }

        public static void Log(ConsoleColor color, string s)
        {
            string namesection = GetNameSection();
            Native_LogColor(namesection, s, color);
            Console.RunLogCallbacks(namesection, s);
        }

        public static void Log(string s, params object[] args)
        {
            string namesection = GetNameSection();
            string fmt = string.Format(s, args);
            Native_Log(namesection, fmt);
            Console.RunLogCallbacks(namesection, fmt);
        }

        public static void Log(ConsoleColor color, string s, params object[] args)
        {
            string namesection = GetNameSection();
            string fmt = string.Format(s, args);
            Native_LogColor(namesection, fmt, color);
            Console.RunLogCallbacks(namesection, fmt);
        }

        public static void LogWarning(string s)
        {
            string namesection = GetNameSection();
            Native_LogWarning(namesection, s);
            Console.RunWarningCallbacks(namesection, s);
        }

        public static void LogWarning(string s, params object[] args)
        {
            string namesection = GetNameSection();
            string fmt = string.Format(s, args);
            Native_LogWarning(namesection, fmt);
            Console.RunWarningCallbacks(namesection, fmt);
            Native_LogWarning(GetNameSection(), fmt);
        }

        public static void LogError(string s)
        {
            string namesection = GetNameSection();
            Native_LogError(namesection, s);
            Console.RunErrorCallbacks(namesection, s);
        }
        public static void LogError(string s, params object[] args)
        {
            string namesection = GetNameSection();
            string fmt = string.Format(s, args);
            Native_LogError(namesection, fmt);
            Console.RunErrorCallbacks(namesection, fmt);
        }

        internal static void LogMelonError(string msg, string modname) => Native_LogMelonError((string.IsNullOrEmpty(modname) ? "" : ("[" + modname.Replace(" ", "_") + "] ")), msg);
        internal static void LogMelonCompatibility(MelonBase.MelonCompatibility comp) => Native_LogMelonCompatibility(comp);

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
        internal extern static void Native_Log(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogColor(string namesection, string txt, ConsoleColor color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogWarning(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogError(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogMelonError(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Native_LogMelonCompatibility(MelonBase.MelonCompatibility comp);
    }
}