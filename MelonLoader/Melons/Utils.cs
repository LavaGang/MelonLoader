using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace MelonLoader
{
    public static class MelonUtils
    {
        internal static void Setup()
        {
            CurrentGameAttribute = new MelonGameAttribute(Internal_GetGameDeveloper(), Internal_GetGameName());
            GameDirectory = Internal_GetGameDirectory();
            UserDataDirectory = Path.Combine(GameDirectory, "UserData");
            if (!Directory.Exists(UserDataDirectory))
                Directory.CreateDirectory(UserDataDirectory);
        }

        public static string GameDirectory { get; internal set; }
        public static string UserDataDirectory { get; internal set; }
        public static MelonGameAttribute CurrentGameAttribute { get; internal set; }
        public static string GameDeveloper { get => CurrentGameAttribute.Developer; }
        public static string GameName { get => CurrentGameAttribute.Name; }
        public static bool IsVRChat { get => CurrentGameAttribute.IsCompatible("VRChat", "VRChat"); }
        public static bool IsBONEWORKS { get => CurrentGameAttribute.IsCompatible("Stress Level Zero", "BONEWORKS"); }

        public static string RandomString(int length)
        {
            StringBuilder builder = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < length; i++)
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(25 * rand.NextDouble())) + 65));
            return builder.ToString();
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
    }
}