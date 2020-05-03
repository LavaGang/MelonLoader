using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace MelonLoader
{
    public class MelonModLogger
    {
        internal static bool consoleEnabled = false;
        private static ConsoleColor rainbow = ConsoleColor.DarkBlue;
        private static readonly Random rainbowrand = new Random();

        private static string GetTimestamp() { return DateTime.Now.ToString("HH:mm:ss.fff"); }

        private static string GetNameSection()
        {
            StackTrace st = new StackTrace(2, true);
            StackFrame sf = st.GetFrame(0);
            if (sf != null)
            {
                MethodBase method = sf.GetMethod();
                if (NETFrameworkFix.MethodBase_op_Inequality(method, null))
                {
                    Type methodClassType = method.DeclaringType;
                    if (NETFrameworkFix.Type_op_Equality(methodClassType, null))
                    {
                        Assembly asm = methodClassType.Assembly;
                        if (NETFrameworkFix.Assembly_op_Inequality(asm, null))
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

        public static void Log(string s)
        {
            string namesection = GetNameSection();
            Imports.Logger_Log(namesection + s);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                bool rainbow_check = RainbowCheck();
                System.Console.Write("[");
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Green);
                System.Console.Write(GetTimestamp());
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Gray);
                System.Console.Write("] [");
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Magenta);
                System.Console.Write("MelonLoader");
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Gray);
                System.Console.WriteLine("] " + namesection + s);
                if (rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        public static void Log(ConsoleColor color, string s)
        {
            string namesection = GetNameSection();
            Imports.Logger_LogColor((namesection + s), color);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                Imports.Console_SetColor(color);
                RainbowCheck();
                System.Console.WriteLine("[" + GetTimestamp() + "] [MelonLoader] " + namesection + s);
                Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        public static void Log(string s, params object[] args)
        {
            string namesection = GetNameSection();
            var formatted = string.Format(s, args);
            Imports.Logger_Log(namesection + formatted);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                bool rainbow_check = RainbowCheck();
                System.Console.WriteLine("[" + GetTimestamp() + "] [MelonLoader] " + namesection + formatted);
                if (rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        public static void Log(ConsoleColor color, string s, params object[] args)
        {
            string namesection = GetNameSection();
            var formatted = string.Format(s, args);
            Imports.Logger_LogColor((namesection + formatted), color);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                Imports.Console_SetColor(color);
                RainbowCheck();
                System.Console.WriteLine("[" + GetTimestamp() + "] [MelonLoader] " + namesection + formatted);
                Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        public static void LogWarning(string s)
        {
            string namesection = GetNameSection();
            Imports.Logger_LogWarning(namesection, s);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                Imports.Console_SetColor(ConsoleColor.Yellow);
                RainbowCheck();
                System.Console.WriteLine("[" + GetTimestamp() + "] [MelonLoader] " + namesection + "[Warning] " + s);
                Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        public static void LogWarning(string s, params object[] args)
        {
            string namesection = GetNameSection();
            var formatted = string.Format(s, args);
            Imports.Logger_LogWarning(namesection, formatted);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                Imports.Console_SetColor(ConsoleColor.Yellow);
                RainbowCheck();
                System.Console.WriteLine("[" + GetTimestamp() + "] [MelonLoader] " + namesection + "[Warning] " + formatted);
                Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        public static void LogError(string s)
        {
            string namesection = GetNameSection();
            Imports.Logger_LogError(namesection, s);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                Imports.Console_SetColor(ConsoleColor.Red);
                RainbowCheck();
                System.Console.WriteLine("[" + GetTimestamp() + "] [MelonLoader] " + namesection + "[Error] " + s);
                Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        public static void LogError(string s, params object[] args)
        {
            string namesection = GetNameSection();
            var formatted = string.Format(s, args);
            Imports.Logger_LogError(namesection, formatted);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                Imports.Console_SetColor(ConsoleColor.Red);
                RainbowCheck();
                System.Console.WriteLine("[" + GetTimestamp() + "] [MelonLoader] " + namesection + "[Error] " + formatted);
                Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        internal static void LogModError(string msg, string modname)
        {
            string namesection = (string.IsNullOrEmpty(modname) ? "" : ("[" + modname.Replace(" ", "_") + "] "));
            Imports.Logger_LogModError(namesection, msg);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                Imports.Console_SetColor(ConsoleColor.Yellow);
                RainbowCheck();
                System.Console.WriteLine("[" + GetTimestamp() + "] [MelonLoader] " + namesection + "[Error] " + msg);
                Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        internal static void LogModStatus(int type)
        {
            Imports.Logger_LogModStatus(type);
            if (!Imports.IsDebugMode() && Console.Enabled)
            {
                bool rainbow_check = RainbowCheck();
                System.Console.Write("[");
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Green);
                System.Console.Write(GetTimestamp());
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Gray);
                System.Console.Write("] [");
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Magenta);
                System.Console.Write("MelonLoader");
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Gray);
                System.Console.Write("] ");
                if (!rainbow_check)
                    Imports.Console_SetColor(ConsoleColor.Blue);
                System.Console.Write("Status: ");
                if (type == 0)
                {
                    if (!rainbow_check)
                        Imports.Console_SetColor(ConsoleColor.Cyan);
                    System.Console.WriteLine("Universal");
                }
                else if (type == 1)
                {
                    if (!rainbow_check)
                        Imports.Console_SetColor(ConsoleColor.Green);
                    System.Console.WriteLine("Compatible");
                }
                else if (type == 2)
                {
                    if (!rainbow_check)
                        Imports.Console_SetColor(ConsoleColor.Yellow);
                    System.Console.WriteLine("No MelonModGameAttribute!");
                }
                else
                {
                    if (!rainbow_check)
                        Imports.Console_SetColor(ConsoleColor.Red);
                    System.Console.WriteLine("INCOMPATIBLE!");
                }
                Imports.Console_SetColor(ConsoleColor.Gray);
            }
        }

        private static bool RainbowCheck()
        {
            if (Imports.IsRainbowMode() || Imports.IsRandomRainbowMode())
            {
                if (Imports.IsRandomRainbowMode())
                    Imports.Console_SetColor((ConsoleColor)rainbowrand.Next(1, (int)ConsoleColor.White));
                else
                {
                    Imports.Console_SetColor(rainbow);
                    rainbow++;
                    if (rainbow > ConsoleColor.White)
                        rainbow = ConsoleColor.DarkBlue;
                    else if (rainbow == ConsoleColor.Gray)
                        rainbow++;
                }
                return true;
            }
            return false;
        }
    }
}