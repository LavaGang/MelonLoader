using System;
using System.Drawing;
using MelonLoader.Utils;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonColorAttribute : Attribute
    {
        /// <summary>
        /// Color of the Melon.
        /// </summary>
        [Obsolete("Color is obsolete. Use DrawingColor for full Color support.")]
        public ConsoleColor Color
        {
            get => LoggerUtils.DrawingColorToConsoleColor(DrawingColor);
            set => DrawingColor = LoggerUtils.ConsoleColorToDrawingColor(value);
        }

        /// <summary>
        /// Color of the Author Log.
        /// </summary>
        public Color DrawingColor { get; internal set; }

        public MelonColorAttribute() 
            => DrawingColor = MelonLogger.DefaultTextColor;

        public MelonColorAttribute(Color drawingColor) 
            => DrawingColor = drawingColor;

        [Obsolete("ConsoleColor is obsolete, use the (int, int, int, int) or (Color) constructor instead.")]
        public MelonColorAttribute(ConsoleColor color) 
            => Color = ((color == ConsoleColor.Black) ? LoggerUtils.DrawingColorToConsoleColor(MelonLogger.DefaultMelonColor) : color);

        public MelonColorAttribute(int alpha, int red, int green, int blue) 
            => DrawingColor =  System.Drawing.Color.FromArgb(alpha, red, green, blue);
    }
}