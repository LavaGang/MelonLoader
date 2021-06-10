using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using MelonLoader.TinyJSON;
#pragma warning disable 0618

namespace MelonLoader
{
    public static class MelonUtils
    {
        internal static void Setup()
        {
            GameDeveloper = string.Copy(Internal_GetGameDeveloper());
            GameName = string.Copy(Internal_GetGameName());
            HashCode = string.Copy(Internal_GetHashCode());
            CurrentGameAttribute = new MelonGameAttribute(GameDeveloper, GameName);
            GameDirectory = string.Copy(Internal_GetGameDirectory());
            UserDataDirectory = Path.Combine(GameDirectory, "UserData");
            if (!Directory.Exists(UserDataDirectory))
                Directory.CreateDirectory(UserDataDirectory);
            Main.IsBoneworks = IsBONEWORKS;
        }

        public static string GameDirectory { get; private set; }
        public static string UserDataDirectory { get; private set; }
        public static MelonGameAttribute CurrentGameAttribute { get; private set; }
        public static string GameDeveloper { get; private set; }
        public static string GameName { get; private set; }
        public static bool IsBONEWORKS { get => (!string.IsNullOrEmpty(GameDeveloper) && GameDeveloper.Equals("Stress Level Zero") && !string.IsNullOrEmpty(GameName) && GameName.Equals("BONEWORKS")); }
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> { if (value.CompareTo(min) < 0) return min; if (value.CompareTo(max) > 0) return max; return value; }
        public static string HashCode { get; private set; }

        public static string RandomString(int length)
        {
            StringBuilder builder = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < length; i++)
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(25 * rand.NextDouble())) + 65));
            return builder.ToString();
        }

        public static void SetCurrentDomainBaseDirectory(string dirpath, AppDomain domain = null)
        {
            if (domain == null)
                domain = AppDomain.CurrentDomain;
            try
            {
                ((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(domain, new object[0]))
                    .ApplicationBase = dirpath;
            }
            catch (Exception ex) { MelonLogger.Warning($"AppDomainSetup.ApplicationBase Exception: {ex}"); }
            Directory.SetCurrentDirectory(dirpath);
        }

        public static MelonBase GetMelonFromStackTrace()
        {
            StackTrace st = new StackTrace(3, true);
            if (st.FrameCount <= 0)
                return null;
            MelonBase output = CheckForMelonInFrame(st);
            if (output == null)
                output = CheckForMelonInFrame(st, 1);
            if (output == null)
                output = CheckForMelonInFrame(st, 2);
            return output;
        }
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

        public static string ColorToANSI(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => "\x1b[30m",
                ConsoleColor.DarkBlue => "\x1b[34m",
                ConsoleColor.DarkGreen => "\x1b[32m",
                ConsoleColor.DarkCyan => "\x1b[36m",
                ConsoleColor.DarkRed => "\x1b[31m",
                ConsoleColor.DarkMagenta => "\x1b[35m",
                ConsoleColor.DarkYellow => "\x1b[33m",
                ConsoleColor.Gray => "\x1b[37m",
                ConsoleColor.DarkGray => "\x1b[90m",
                ConsoleColor.Blue => "\x1b[94m",
                ConsoleColor.Green => "\x1b[92m",
                ConsoleColor.Cyan => "\x1b[96m",
                ConsoleColor.Red => "\x1b[91m",
                ConsoleColor.Magenta => "\x1b[95m",
                ConsoleColor.Yellow => "\x1b[93m",
                _ => "\x1b[97m",
            };
        }

        public static T ParseJSONStringtoStruct<T>(string jsonstr)
        {
            if (string.IsNullOrEmpty(jsonstr))
                return default;
            Variant jsonarr = null;
            try { jsonarr = JSON.Load(jsonstr); }
            catch (Exception ex)
            {
                MelonLogger.Error($"Exception while Decoding JSON String to JSON Variant: {ex}");
                return default;
            }
            if (jsonarr == null)
                return default;
            T returnobj = default;
            try { returnobj = jsonarr.Make<T>(); }
            catch (Exception ex) { MelonLogger.Error($"Exception while Converting JSON Variant to {typeof(T).Name}: {ex}"); }
            return returnobj;
        }

        public static T PullAttributeFromAssembly<T>(Assembly asm, bool inherit = false) where T : Attribute
        {
            T[] attributetbl = PullAttributesFromAssembly<T>(asm, inherit);
            if ((attributetbl == null) || (attributetbl.Length <= 0))
                return null;
            return attributetbl[0];
        }

        public static T[] PullAttributesFromAssembly<T>(Assembly asm, bool inherit = false) where T : Attribute
        {
            Attribute[] att_tbl = Attribute.GetCustomAttributes(asm, inherit);
            if ((att_tbl == null) || (att_tbl.Length <= 0))
                return null;
            Type requestedType = typeof(T);
            List<T> output = new List<T>();
            foreach (Attribute att in att_tbl)
            {
                Type attType = att.GetType();
                string attAssemblyName = attType.Assembly.GetName().Name;
                string requestedAssemblyName = requestedType.Assembly.GetName().Name;
                if ((attType == requestedType)
                    || attType.FullName.Equals(requestedType.FullName)
                    || ((attAssemblyName.Equals("MelonLoader")
                        || attAssemblyName.Equals("MelonLoader.ModHandler"))
                        && (requestedAssemblyName.Equals("MelonLoader")
                        || requestedAssemblyName.Equals("MelonLoader.ModHandler"))
                        && attType.Name.Equals(requestedType.Name)))
                    output.Add(att as T);
            }
            return output.ToArray();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsGame32Bit();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsGameIl2Cpp();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsOldMono();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetApplicationPath();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetGameDataDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetUnityVersion();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetManagedDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void SetConsoleTitle([MarshalAs(UnmanagedType.LPStr)] string title);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetFileProductName([MarshalAs(UnmanagedType.LPStr)] string filepath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void NativeHookAttach(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void NativeHookDetach(IntPtr target, IntPtr detour);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetGameName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetGameDeveloper();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetGameDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetHashCode();
    }
}