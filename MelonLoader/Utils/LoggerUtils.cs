using System;
using System.Collections.Generic;
using System.Drawing;

namespace MelonLoader.Utils;

internal static class LoggerUtils
{
    internal static Dictionary<ConsoleColor, Color> ConsoleColorDict = new()
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

    internal static Dictionary<Color, ConsoleColor> DrawingColorDict = new()
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

    internal static Color ConsoleColorToDrawingColor(ConsoleColor color)
    {
        return !ConsoleColorDict.ContainsKey(color) ? Color.White : ConsoleColorDict[color];
    }

    internal static ConsoleColor DrawingColorToConsoleColor(Color color)
    {
        return !DrawingColorDict.ContainsKey(color) ? ConsoleColor.White : DrawingColorDict[color];
    }
}
