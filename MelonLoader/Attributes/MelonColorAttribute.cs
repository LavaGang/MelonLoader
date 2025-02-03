using System;
using System.Drawing;
using MelonLoader.Utils;

namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MelonColorAttribute : Attribute
    {
        /// <summary>
        /// Color of the Author Log.
        /// </summary>
        public Color DrawingColor { get; internal set; }

        public MelonColorAttribute() 
            => DrawingColor = MelonLogger.DefaultTextColor;

        public MelonColorAttribute(int alpha, int red, int green, int blue) 
            => DrawingColor =  System.Drawing.Color.FromArgb(alpha, red, green, blue);
    }
}