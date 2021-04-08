using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonColorAttribute : Attribute
    {
        /// <summary>
        /// Color of the Melon.
        /// </summary>
        public ConsoleColor Color { get; internal set; }

        public MelonColorAttribute() { Color = MelonLogger.DefaultMelonColor; }
        public MelonColorAttribute(ConsoleColor color) { Color = ((color == ConsoleColor.Black) ? MelonLogger.DefaultMelonColor : color); }
    }
}