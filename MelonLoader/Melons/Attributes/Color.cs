using System;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MelonColorAttribute : Attribute
    {
        /// <summary>
        /// Color of the Melon.
        /// </summary>
        public ConsoleColor Color { get; internal set; }

        public MelonColorAttribute() { Color = ConsoleColor.Magenta; }
        public MelonColorAttribute(ConsoleColor color) { Color = color; }
    }
}