using System.Drawing;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Logging;

[StructLayout(LayoutKind.Sequential)]
internal struct ColorRGB(byte r, byte g, byte b)
{
    public byte R { get; set; } = r;
    public byte G { get; set; } = g;
    public byte B { get; set; } = b;

    public static implicit operator ColorRGB(Color color)
        => new(color.R, color.G, color.B);

    public static implicit operator Color(ColorRGB color)
        => Color.FromArgb(color.R, color.G, color.B);
}