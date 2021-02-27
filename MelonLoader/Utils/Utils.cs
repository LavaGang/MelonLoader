using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

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
        }

        public static string GameDirectory { get; private set; }
        public static string UserDataDirectory { get; private set; }
        public static MelonGameAttribute CurrentGameAttribute { get; private set; }
        public static string GameDeveloper { get; private set; }
        public static string GameName { get; private set; }
        public static bool IsVRChat { get => (!string.IsNullOrEmpty(GameDeveloper) && GameDeveloper.Equals("VRChat") && !string.IsNullOrEmpty(GameName) && GameName.Equals("VRChat")); }
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
            switch (color)
            {
                case ConsoleColor.Black:
                    return "\x1b[30m";
                case ConsoleColor.DarkBlue:
                    return "\x1b[34m";
                case ConsoleColor.DarkGreen:
                    return "\x1b[32m";
                case ConsoleColor.DarkCyan:
                    return "\x1b[36m";
                case ConsoleColor.DarkRed:
                    return "\x1b[31m";
                case ConsoleColor.DarkMagenta:
                    return "\x1b[35m";
                case ConsoleColor.DarkYellow:
                    return "\x1b[33m";
                case ConsoleColor.Gray:
                    return "\x1b[37m";
                case ConsoleColor.DarkGray:
                    return "\x1b[90m";
                case ConsoleColor.Blue:
                    return "\x1b[94m";
                case ConsoleColor.Green:
                    return "\x1b[92m";
                case ConsoleColor.Cyan:
                    return "\x1b[96m";
                case ConsoleColor.Red:
                    return "\x1b[91m";
                case ConsoleColor.Magenta:
                    return "\x1b[95m";
                case ConsoleColor.Yellow:
                    return "\x1b[93m";
                case ConsoleColor.White:
                default:
                    return "\x1b[97m";
            }
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