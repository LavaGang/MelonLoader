using System;
using System.Collections.Generic;
using MelonLoader.Logging;

namespace MelonLoader.Utils
{
    internal static class LoggerUtils
    {
        internal static Dictionary<ConsoleColor, ColorARGB> ConsoleColorDict = new Dictionary<ConsoleColor, ColorARGB>
        {
            { ConsoleColor.Black, ColorARGB.Black },
            { ConsoleColor.DarkBlue, ColorARGB.DarkBlue },
            { ConsoleColor.DarkGreen, ColorARGB.DarkGreen },
            { ConsoleColor.DarkCyan, ColorARGB.DarkCyan },
            { ConsoleColor.DarkRed, ColorARGB.DarkRed },
            { ConsoleColor.DarkMagenta, ColorARGB.DarkMagenta },
            { ConsoleColor.DarkYellow, ColorARGB.Yellow },
            { ConsoleColor.Gray, ColorARGB.LightGray },
            { ConsoleColor.DarkGray, ColorARGB.DarkGray },
            { ConsoleColor.Blue, ColorARGB.CornflowerBlue } ,
            { ConsoleColor.Green, ColorARGB.LimeGreen },
            { ConsoleColor.Cyan, ColorARGB.Cyan },
            { ConsoleColor.Red, ColorARGB.IndianRed },
            { ConsoleColor.Magenta, ColorARGB.Magenta },
            { ConsoleColor.Yellow, ColorARGB.Yellow },
            { ConsoleColor.White, ColorARGB.White },
        };

        internal static Dictionary<ColorARGB, ConsoleColor> DrawingColorDict = new Dictionary<ColorARGB, ConsoleColor>
        {
            { ColorARGB.Black, ConsoleColor.Black },
            { ColorARGB.DarkBlue, ConsoleColor.DarkBlue },
            { ColorARGB.DarkGreen, ConsoleColor.DarkGreen },
            { ColorARGB.DarkCyan, ConsoleColor.DarkCyan },
            { ColorARGB.DarkRed, ConsoleColor.DarkRed },
            { ColorARGB.DarkMagenta, ConsoleColor.DarkMagenta },
            { ColorARGB.Yellow, ConsoleColor.Yellow },
            { ColorARGB.LightGray, ConsoleColor.Gray },
            { ColorARGB.DarkGray, ConsoleColor.DarkGray },
            { ColorARGB.CornflowerBlue, ConsoleColor.Blue } ,
            { ColorARGB.LimeGreen, ConsoleColor.Green },
            { ColorARGB.Cyan, ConsoleColor.Cyan },
            { ColorARGB.IndianRed, ConsoleColor.Red },
            { ColorARGB.Magenta, ConsoleColor.Magenta },
            { ColorARGB.White, ConsoleColor.White },
        };

        internal static ColorARGB ConsoleColorToDrawingColor(ConsoleColor color)
        {
            if (!ConsoleColorDict.ContainsKey(color))
                return ColorARGB.White;

            return ConsoleColorDict[color];
        }

        internal static ConsoleColor DrawingColorToConsoleColor(ColorARGB color)
        {
            if (!DrawingColorDict.ContainsKey(color))
                return ConsoleColor.White;

            return DrawingColorDict[color];
        }
    }
}
