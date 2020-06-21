using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace MelonLoader
{
    public class MelonModLogger
    {
        internal static bool consoleEnabled = false;
        private static int ErrorCount = 0;
        private static int MaxErrorCount = 100;

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
                            object[] attrArray = asm.GetCustomAttributes(typeof(MelonModInfoAttribute), false);
                            if ((attrArray.Count() > 0) && (attrArray[0] != null))
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

        public static void Log(string s) => Imports.Logger_Log((GetNameSection() + s));
        public static void Log(ConsoleColor color, string s) => Imports.Logger_LogColor((GetNameSection() + s), color);
        public static void Log(string s, params object[] args) => Imports.Logger_Log((GetNameSection() + string.Format(s, args)));
        public static void Log(ConsoleColor color, string s, params object[] args) => Imports.Logger_LogColor((GetNameSection() + string.Format(s, args)), color);

        public static void LogWarning(string s) => Imports.Logger_LogWarning(GetNameSection(), s);
        public static void LogWarning(string s, params object[] args) => Imports.Logger_LogWarning(GetNameSection(), string.Format(s, args));

        public static void LogError(string s) { if (ErrorCount < MaxErrorCount) { Imports.Logger_LogError(GetNameSection(), s); ErrorCount++; } }
        public static void LogError(string s, params object[] args) { if (ErrorCount < MaxErrorCount) { Imports.Logger_LogError(GetNameSection(), string.Format(s, args)); ErrorCount++; } }

        internal static void LogModError(string msg, string modname) { if (ErrorCount < MaxErrorCount) { Imports.Logger_LogModError((string.IsNullOrEmpty(modname) ? "" : ("[" + modname.Replace(" ", "_") + "] ")), msg); ErrorCount++; } }

        internal static void LogModStatus(int type) => Imports.Logger_LogModStatus(type);
    }
}