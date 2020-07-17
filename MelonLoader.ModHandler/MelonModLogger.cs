using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MelonLoader
{
    public class MelonModLogger
    {
        internal static bool consoleEnabled = false;

        private static string GetNameSection()
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
                            object[] attrArray = asm.GetCustomAttributes(typeof(MelonPluginInfoAttribute), false);
                            if ((attrArray.Length > 0) && (attrArray[0] != null))
                            {
                                MelonPluginInfoAttribute attr = attrArray[0] as MelonPluginInfoAttribute;
                                if (!string.IsNullOrEmpty(attr.Name))
                                    return "[" + attr.Name.Replace(" ", "_") + "] ";
                            }
                            attrArray = asm.GetCustomAttributes(typeof(MelonModInfoAttribute), false);
                            if ((attrArray.Length > 0) && (attrArray[0] != null))
                            {
                                MelonModInfoAttribute attr = attrArray[0] as MelonModInfoAttribute;
                                if (!string.IsNullOrEmpty(attr.Name))
                                    return "[" + attr.Name.Replace(" ", "_") + "] ";
                            }
                        }
                    }
                }
            }
            return "";
        }

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