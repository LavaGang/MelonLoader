using System;
using MelonLoader.Logging;
using MelonLoader.Utils;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonColorAttribute : Attribute
    {
        /// <summary>
        /// Color of the Melon.
        /// </summary>
        [Obsolete("Color is obsolete. Use DrawingColor for full Color support. This will be removed in a future update.", true)]
        public ConsoleColor Color
        {
            get => LoggerUtils.DrawingColorToConsoleColor(DrawingColor);
            set => DrawingColor = LoggerUtils.ConsoleColorToDrawingColor(value);
        }

        /// <summary>
        /// Color of the Author Log.
        /// </summary>
        public ColorARGB DrawingColor { get; internal set; }

        public MelonColorAttribute() 
            => DrawingColor = MelonLogger.DefaultTextColor;

        [Obsolete("ConsoleColor is obsolete, use the (int, int, int, int) constructor instead. This will be removed in a future update.", true)]
        public MelonColorAttribute(ConsoleColor color) 
            => Color = ((color == ConsoleColor.Black) ? LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultMelonColor) : color);

        public MelonColorAttribute(int alpha, int red, int green, int blue) 
            => DrawingColor =  ColorARGB.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
    }
}