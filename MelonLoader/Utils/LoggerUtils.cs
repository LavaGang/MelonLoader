using System;
using System.Collections.Generic;

namespace MelonLoader.Utils
{
    internal static class LoggerUtils
    {
        internal static Dictionary<ConsoleColor, string> ConsoleAnsiiDict = new Dictionary<ConsoleColor, string>
        {
            { ConsoleColor.Black, "\x1b[30m" },
            { ConsoleColor.DarkBlue, "\x1b[34m" },
            { ConsoleColor.DarkGreen, "\x1b[32m" },
            { ConsoleColor.DarkCyan, "\x1b[36m" },
            { ConsoleColor.DarkRed, "\x1b[31m" },
            { ConsoleColor.DarkMagenta, "\x1b[35m" },
            { ConsoleColor.DarkYellow, "\x1b[33m" },
            { ConsoleColor.Gray, "\x1b[37m" },
            { ConsoleColor.DarkGray, "\x1b[90m" },
            { ConsoleColor.Blue, "\x1b[94m" } ,
            { ConsoleColor.Green, "\x1b[92m" },
            { ConsoleColor.Cyan, "\x1b[96m" },
            { ConsoleColor.Red, "\x1b[91m" },
            { ConsoleColor.Magenta, "\x1b[95m" },
            { ConsoleColor.Yellow, "\x1b[93m" },
            { ConsoleColor.White, "\x1b[97m" },

        };

        internal static string ColorToAnsi(ConsoleColor color)
        {
            if (!ConsoleAnsiiDict.ContainsKey(color))
                return ConsoleAnsiiDict[ConsoleColor.White];

            return ConsoleAnsiiDict[color];
        }

        internal static string GetTimeStamp() => $"{DateTime.Now:HH:mm:ss.fff}";
    }
}
