﻿using System;
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
            GameDeveloper = External.Utils.Internal_GetGameDeveloper();
            GameName = External.Utils.Internal_GetGameName();
            CurrentGameAttribute = new MelonGameAttribute(GameDeveloper, GameName);
            GameDirectory = External.Utils.Internal_GetGameDirectory();
            UserDataDirectory = Path.Combine(GameDirectory, "UserData");
            if (!Directory.Exists(UserDataDirectory))
                Directory.CreateDirectory(UserDataDirectory);
        }

        public static string GameDirectory { get; internal set; }
        public static string UserDataDirectory { get; internal set; }
        public static MelonGameAttribute CurrentGameAttribute { get; internal set; }
        public static string GameDeveloper { get; internal set; }
        public static string GameName { get; internal set; }
        public static bool IsVRChat { get => (!string.IsNullOrEmpty(GameDeveloper) && GameDeveloper.Equals("VRChat") && !string.IsNullOrEmpty(GameName) && GameName.Equals("VRChat")); }
        public static bool IsBONEWORKS { get => (!string.IsNullOrEmpty(GameDeveloper) && GameDeveloper.Equals("Stress Level Zero") && !string.IsNullOrEmpty(GameName) && GameName.Equals("BONEWORKS")); }
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> { if (value.CompareTo(min) < 0) return min; if (value.CompareTo(max) > 0) return max; return value; }

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
    }
}