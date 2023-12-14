using HarmonyLib;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Utils
{
    public static class MelonExtensions
    {
        #region Assembly

        public static IEnumerable<Type> GetValidTypes(this Assembly asm)
            => GetValidTypes(asm, null);

        public static IEnumerable<Type> GetValidTypes(this Assembly asm, Func<Type, bool> predicate)
        {
            IEnumerable<Type> returnval = Enumerable.Empty<Type>();
            try { returnval = asm.GetTypes().AsEnumerable(); }
            catch (ReflectionTypeLoadException ex)
            {
                //MelonLogger.Error($"Failed to load all types in assembly {asm.FullName} due to: {ex.Message}", ex);
                returnval = ex.Types;
            }
            return returnval.Where(x => (x != null) && ((predicate == null) || predicate(x)));
        }

        #endregion

        #region Delegate

        public static IntPtr GetFunctionPointer(this Delegate del)
            => Marshal.GetFunctionPointerForDelegate(del);

        #endregion

        #region IntPtr

        public static void GetDelegate<T>(this IntPtr ptr, out T output) where T : Delegate
            => output = GetDelegate<T>(ptr);
        public static T GetDelegate<T>(this IntPtr ptr) where T : Delegate
            => GetDelegate(ptr, typeof(T)) as T;
        public static Delegate GetDelegate(this IntPtr ptr, Type type)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));
            Delegate del = Marshal.GetDelegateForFunctionPointer(ptr, type);
            if (del == null)
                throw new Exception($"Unable to Get Delegate of Type {type.FullName} for Function Pointer!");
            return del;
        }

        public static string ToAnsiString(this IntPtr ptr)
            => Marshal.PtrToStringAnsi(ptr);

        #endregion

        #region String

        public static IntPtr ToAnsiPointer(this string str)
            => Marshal.StringToHGlobalAnsi(str);
        
        public static IntPtr ToAutoPointer(this string str)
            => Marshal.StringToHGlobalAuto(str);
        
        public static IntPtr ToUnicodePointer(this string str) 
            => Marshal.StringToHGlobalUni(str);
        
        public static byte[] ToUnicodeBytes(this string str) 
            => Encoding.Unicode.GetBytes(str);
        
        public static IntPtr ToNullTerminatedAnsiPointer(this string str) => Marshal.StringToHGlobalAnsi(str + "\0");

        #endregion

        #region Enum

        /// <summary>
        /// From: http://www.sambeauvois.be/blog/2011/08/enum-hasflag-method-extension-for-4-0-framework/
        /// A FX 3.5 way to mimic the FX4 "HasFlag" method.
        /// </summary>
        /// <param name="variable">The tested enum.</param>
        /// <param name="value">The value to test.</param>
        /// <returns>True if the flag is set. Otherwise false.</returns>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            // check if from the same type.
            if (variable.GetType() != value.GetType())
                throw new ArgumentException("The checked flag is not from the same type as the checked variable.");

            ulong num = Convert.ToUInt64(value);
            ulong num2 = Convert.ToUInt64(variable);
            return (num2 & num) == num;
        }

        #endregion

        #region Type

        public static bool IsTypeOf<T>(this Type type)
            => type.IsAssignableFrom(typeof(T));

        #endregion

        #region MethodBase

        public static DynamicMethodDefinition ToDynamicMethodDefinition(this MethodBase methodBase)
        {
            if (methodBase == null)
                throw new ArgumentNullException(nameof(methodBase));
            return new DynamicMethodDefinition(methodBase);
        }

        public static bool IsNotImplemented(this MethodBase methodBase)
        {
            if (methodBase == null)
                throw new ArgumentNullException(nameof(methodBase));

            DynamicMethodDefinition method = methodBase.ToDynamicMethodDefinition();
            ILContext ilcontext = new(method.Definition);
            ILCursor ilcursor = new(ilcontext);

            bool returnval = (ilcursor.Instrs.Count == 2)
                && (ilcursor.Instrs[1].OpCode.Code == Mono.Cecil.Cil.Code.Throw);

            ilcontext.Dispose();
            method.Dispose();
            return returnval;
        }

        #endregion

        #region MethodInfo

        public static HarmonyMethod ToHarmonyMethod(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            return new HarmonyMethod(methodInfo);
        }

        #endregion

        #region Color

        private static Dictionary<Color, ConsoleColor> DrawingColorDict = new Dictionary<Color, ConsoleColor>
        {
            { Color.Black, ConsoleColor.Black },
            { Color.DarkBlue, ConsoleColor.DarkBlue },
            { Color.DarkGreen, ConsoleColor.DarkGreen },
            { Color.DarkCyan, ConsoleColor.DarkCyan },
            { Color.DarkRed, ConsoleColor.DarkRed },
            { Color.DarkMagenta, ConsoleColor.DarkMagenta },
            { Color.Yellow, ConsoleColor.Yellow },
            { Color.LightGray, ConsoleColor.Gray },
            { Color.DarkGray, ConsoleColor.DarkGray },
            { Color.CornflowerBlue, ConsoleColor.Blue } ,
            { Color.LimeGreen, ConsoleColor.Green },
            { Color.Cyan, ConsoleColor.Cyan },
            { Color.IndianRed, ConsoleColor.Red },
            { Color.Magenta, ConsoleColor.Magenta },
            { Color.White, ConsoleColor.White },
        };

        public static ConsoleColor ToConsoleColor(this Color color)
        {
            if (!DrawingColorDict.ContainsKey(color))
                return MelonUtils.DefaultTextConsoleColor;
            return DrawingColorDict[color];
        }

        #endregion

        #region ConsoleColor

        private static Dictionary<ConsoleColor, Color> ConsoleColorDict = new Dictionary<ConsoleColor, Color>
        {
            { ConsoleColor.Black, Color.Black },
            { ConsoleColor.DarkBlue, Color.DarkBlue },
            { ConsoleColor.DarkGreen, Color.DarkGreen },
            { ConsoleColor.DarkCyan, Color.DarkCyan },
            { ConsoleColor.DarkRed, Color.DarkRed },
            { ConsoleColor.DarkMagenta, Color.DarkMagenta },
            { ConsoleColor.DarkYellow, Color.Yellow },
            { ConsoleColor.Gray, Color.LightGray },
            { ConsoleColor.DarkGray, Color.DarkGray },
            { ConsoleColor.Blue, Color.CornflowerBlue } ,
            { ConsoleColor.Green, Color.LimeGreen },
            { ConsoleColor.Cyan, Color.Cyan },
            { ConsoleColor.Red, Color.IndianRed },
            { ConsoleColor.Magenta, Color.Magenta },
            { ConsoleColor.Yellow, Color.Yellow },
            { ConsoleColor.White, Color.White },
        };

        public static Color ToDrawingColor(this ConsoleColor color)
        {
            if (!ConsoleColorDict.ContainsKey(color))
                return MelonUtils.DefaultTextColor;
            return ConsoleColorDict[color];
        }

        #endregion
    }
}
