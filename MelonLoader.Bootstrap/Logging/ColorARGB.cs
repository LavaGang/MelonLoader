using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Logging;

[StructLayout(LayoutKind.Sequential)]
public readonly struct ColorARGB : IEquatable<ColorARGB>
{
    private readonly uint value;

    /// <summary>Gets the red component value of this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure.</summary>
    /// <returns>The red component value of this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</returns>
    public byte R => (byte) ((ulong) (value >> 16) & byte.MaxValue);

    /// <summary>Gets the green component value of this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure.</summary>
    /// <returns>The green component value of this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</returns>
    public byte G => (byte) ((ulong) (value >> 8) & byte.MaxValue);

    /// <summary>Gets the blue component value of this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure.</summary>
    /// <returns>The blue component value of this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</returns>
    public byte B => (byte) ((ulong) value & byte.MaxValue);

    /// <summary>Gets the alpha component value of this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure.</summary>
    /// <returns>The alpha component value of this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</returns>
    public byte A => (byte) ((ulong) (value >> 24) & byte.MaxValue);

    private ColorARGB(uint value) => this.value = value;

    private static uint MakeArgb(byte alpha, byte red, byte green, byte blue)
    {
        return (uint) (red << 16 | green << 8 | blue | alpha << 24) & uint.MaxValue;
    }

    /// <summary>Creates a <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure from a 32-bit ARGB value.</summary>
    /// <param name="argb">A value specifying the 32-bit ARGB value.</param>
    /// <returns>The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure that this method creates.</returns>
    public static ColorARGB FromArgb(uint argb)
    {
        return new ColorARGB(argb);
    }

    /// <summary>Creates a <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure from the four ARGB component (alpha, red, green, and blue) values.</summary>
    /// <param name="alpha">The alpha component.</param>
    /// <param name="red">The red component.</param>
    /// <param name="green">The green component.</param>
    /// <param name="blue">The blue component.</param>
    /// <returns>The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> that this method creates.</returns>
    public static ColorARGB FromArgb(byte alpha, byte red, byte green, byte blue)
    {
        return new ColorARGB(MakeArgb(alpha, red, green, blue));
    }

    /// <summary>Creates a <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure from the specified <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure, but with the new specified alpha value.</summary>
    /// <param name="alpha">The alpha value for the new <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</param>
    /// <param name="baseColor">The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> from which to create the new <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</param>
    /// <returns>The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> that this method creates.</returns>
    public static ColorARGB FromArgb(byte alpha, ColorARGB baseColor)
    {
      return new ColorARGB(MakeArgb(alpha, baseColor.R, baseColor.G, baseColor.B));
    }

    /// <summary>Creates a <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure from the specified 8-bit color values (red, green, and blue). The alpha value is implicitly 255 (fully opaque).</summary>
    /// <param name="red">The red component value for the new <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</param>
    /// <param name="green">The green component value for the new <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</param>
    /// <param name="blue">The blue component value for the new <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</param>
    /// <returns>The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> that this method creates.</returns>
    public static ColorARGB FromArgb(byte red, byte green, byte blue)
    {
      return FromArgb(byte.MaxValue, red, green, blue);
    }
    
    /// <summary>Tests whether two specified <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structures are equivalent.</summary>
    /// <param name="left">The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> that is to the left of the equality operator.</param>
    /// <param name="right">The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> that is to the right of the equality operator.</param>
    /// <returns>
    /// <see langword="true" /> if the two <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structures are equal; otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator ==(ColorARGB left, ColorARGB right)
    {
        return left.value == right.value;
    }

    /// <summary>Tests whether two specified <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structures are different.</summary>
    /// <param name="left">The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> that is to the left of the inequality operator.</param>
    /// <param name="right">The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> that is to the right of the inequality operator.</param>
    /// <returns>
    /// <see langword="true" /> if the two <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structures are different; otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator !=(ColorARGB left, ColorARGB right) => !(left == right);

    /// <summary>Tests whether the specified object is a <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure and is equivalent to this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure.</summary>
    /// <param name="obj">The object to test.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure equivalent to this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure; otherwise, <see langword="false" />.
    /// </returns>
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override bool Equals(object obj)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    {
        return obj is ColorARGB color && value == color.value;
    }

    /// <summary>Tests whether the specified <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure is equivalent to this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure.</summary>
    /// <param name="other">The <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure to test.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="other" /> is equivalent to this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure; otherwise, <see langword="false" />.
    /// </returns>
    public bool Equals(ColorARGB other)
    {
        return value == other.value;
    }

    /// <summary>Returns a hash code for this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" /> structure.</summary>
    /// <returns>An integer value that specifies the hash code for this <see cref="T:MelonLoader.Bootstrap.Logging.ColorARGB" />.</returns>
    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    /// <summary>Gets a ColorARGB that has an ARGB value of #00FFFFFF.</summary>
    public static ColorARGB Transparent => FromArgb(0, 255, 255, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF0F8FF.</summary>
    public static ColorARGB AliceBlue => FromArgb(255, 240, 248, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFAEBD7.</summary>
    public static ColorARGB AntiqueWhite => FromArgb(255, 250, 235, 215);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF00FFFF.</summary>
    public static ColorARGB Aqua => FromArgb(255, 0, 255, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF7FFFD4.</summary>
    public static ColorARGB Aquamarine => FromArgb(255, 127, 255, 212);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF0FFFF.</summary>
    public static ColorARGB Azure => FromArgb(255, 240, 255, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF5F5DC.</summary>
    public static ColorARGB Beige => FromArgb(255, 245, 245, 220);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFE4C4.</summary>
    public static ColorARGB Bisque => FromArgb(255, 255, 228, 196);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF000000.</summary>
    public static ColorARGB Black => FromArgb(255, 0, 0, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFEBCD.</summary>
    public static ColorARGB BlanchedAlmond => FromArgb(255, 255, 235, 205);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF0000FF.</summary>
    public static ColorARGB Blue => FromArgb(255, 0, 0, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF8A2BE2.</summary>
    public static ColorARGB BlueViolet => FromArgb(255, 138, 43, 226);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFA52A2A.</summary>
    public static ColorARGB Brown => FromArgb(255, 165, 42, 42);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFDEB887.</summary>
    public static ColorARGB BurlyWood => FromArgb(255, 222, 184, 135);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF5F9EA0.</summary>
    public static ColorARGB CadetBlue => FromArgb(255, 95, 158, 160);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF7FFF00.</summary>
    public static ColorARGB Chartreuse => FromArgb(255, 127, 255, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFD2691E.</summary>
    public static ColorARGB Chocolate => FromArgb(255, 210, 105, 30);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF7F50.</summary>
    public static ColorARGB Coral => FromArgb(255, 255, 127, 80);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF6495ED.</summary>
    public static ColorARGB CornflowerBlue => FromArgb(255, 100, 149, 237);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFF8DC.</summary>
    public static ColorARGB Cornsilk => FromArgb(255, 255, 248, 220);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFDC143C.</summary>
    public static ColorARGB Crimson => FromArgb(255, 220, 20, 60);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF00FFFF.</summary>
    public static ColorARGB Cyan => FromArgb(255, 0, 255, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF00008B.</summary>
    public static ColorARGB DarkBlue => FromArgb(255, 0, 0, 139);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF008B8B.</summary>
    public static ColorARGB DarkCyan => FromArgb(255, 0, 139, 139);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFB8860B.</summary>
    public static ColorARGB DarkGoldenrod => FromArgb(255, 184, 134, 11);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFA9A9A9.</summary>
    public static ColorARGB DarkGray => FromArgb(255, 169, 169, 169);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF006400.</summary>
    public static ColorARGB DarkGreen => FromArgb(255, 0, 100, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFBDB76B.</summary>
    public static ColorARGB DarkKhaki => FromArgb(255, 189, 183, 107);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF8B008B.</summary>
    public static ColorARGB DarkMagenta => FromArgb(255, 139, 0, 139);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF556B2F.</summary>
    public static ColorARGB DarkOliveGreen => FromArgb(255, 85, 107, 47);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF8C00.</summary>
    public static ColorARGB DarkOrange => FromArgb(255, 255, 140, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF9932CC.</summary>
    public static ColorARGB DarkOrchid => FromArgb(255, 153, 50, 204);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF8B0000.</summary>
    public static ColorARGB DarkRed => FromArgb(255, 139, 0, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFE9967A.</summary>
    public static ColorARGB DarkSalmon => FromArgb(255, 233, 150, 122);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF8FBC8B.</summary>
    public static ColorARGB DarkSeaGreen => FromArgb(255, 143, 188, 139);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF483D8B.</summary>
    public static ColorARGB DarkSlateBlue => FromArgb(255, 72, 61, 139);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF2F4F4F.</summary>
    public static ColorARGB DarkSlateGray => FromArgb(255, 47, 79, 79);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF00CED1.</summary>
    public static ColorARGB DarkTurquoise => FromArgb(255, 0, 206, 209);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF9400D3.</summary>
    public static ColorARGB DarkViolet => FromArgb(255, 148, 0, 211);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF1493.</summary>
    public static ColorARGB DeepPink => FromArgb(255, 255, 20, 147);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF00BFFF.</summary>
    public static ColorARGB DeepSkyBlue => FromArgb(255, 0, 191, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF696969.</summary>
    public static ColorARGB DimGray => FromArgb(255, 105, 105, 105);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF1E90FF.</summary>
    public static ColorARGB DodgerBlue => FromArgb(255, 30, 144, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFB22222.</summary>
    public static ColorARGB Firebrick => FromArgb(255, 178, 34, 34);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFFAF0.</summary>
    public static ColorARGB FloralWhite => FromArgb(255, 255, 250, 240);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF228B22.</summary>
    public static ColorARGB ForestGreen => FromArgb(255, 34, 139, 34);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF00FF.</summary>
    public static ColorARGB Fuchsia => FromArgb(255, 255, 0, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFDCDCDC.</summary>
    public static ColorARGB Gainsboro => FromArgb(255, 220, 220, 220);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF8F8FF.</summary>
    public static ColorARGB GhostWhite => FromArgb(255, 248, 248, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFD700.</summary>
    public static ColorARGB Gold => FromArgb(255, 255, 215, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFDAA520.</summary>
    public static ColorARGB Goldenrod => FromArgb(255, 218, 165, 32);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF808080.</summary>
    public static ColorARGB Gray => FromArgb(255, 128, 128, 128);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF008000.</summary>
    public static ColorARGB Green => FromArgb(255, 0, 128, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFADFF2F.</summary>
    public static ColorARGB GreenYellow => FromArgb(255, 173, 255, 47);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF0FFF0.</summary>
    public static ColorARGB Honeydew => FromArgb(255, 240, 255, 240);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF69B4.</summary>
    public static ColorARGB HotPink => FromArgb(255, 255, 105, 180);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFCD5C5C.</summary>
    public static ColorARGB IndianRed => FromArgb(255, 205, 92, 92);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF4B0082.</summary>
    public static ColorARGB Indigo => FromArgb(255, 75, 0, 130);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFFFF0.</summary>
    public static ColorARGB Ivory => FromArgb(255, 255, 255, 240);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF0E68C.</summary>
    public static ColorARGB Khaki => FromArgb(255, 240, 230, 140);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFE6E6FA.</summary>
    public static ColorARGB Lavender => FromArgb(255, 230, 230, 250);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFF0F5.</summary>
    public static ColorARGB LavenderBlush => FromArgb(255, 255, 240, 245);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF7CFC00.</summary>
    public static ColorARGB LawnGreen => FromArgb(255, 124, 252, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFFACD.</summary>
    public static ColorARGB LemonChiffon => FromArgb(255, 255, 250, 205);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFADD8E6.</summary>
    public static ColorARGB LightBlue => FromArgb(255, 173, 216, 230);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF08080.</summary>
    public static ColorARGB LightCoral => FromArgb(255, 240, 128, 128);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFE0FFFF.</summary>
    public static ColorARGB LightCyan => FromArgb(255, 224, 255, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFAFAD2.</summary>
    public static ColorARGB LightGoldenrodYellow => FromArgb(255, 250, 250, 210);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF90EE90.</summary>
    public static ColorARGB LightGreen => FromArgb(255, 144, 238, 144);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFD3D3D3.</summary>
    public static ColorARGB LightGray => FromArgb(255, 211, 211, 211);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFB6C1.</summary>
    public static ColorARGB LightPink => FromArgb(255, 255, 182, 193);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFA07A.</summary>
    public static ColorARGB LightSalmon => FromArgb(255, 255, 160, 122);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF20B2AA.</summary>
    public static ColorARGB LightSeaGreen => FromArgb(255, 32, 178, 170);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF87CEFA.</summary>
    public static ColorARGB LightSkyBlue => FromArgb(255, 135, 206, 250);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF778899.</summary>
    public static ColorARGB LightSlateGray => FromArgb(255, 119, 136, 153);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFB0C4DE.</summary>
    public static ColorARGB LightSteelBlue => FromArgb(255, 176, 196, 222);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFFFE0.</summary>
    public static ColorARGB LightYellow => FromArgb(255, 255, 255, 224);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF00FF00.</summary>
    public static ColorARGB Lime => FromArgb(255, 0, 255, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF32CD32.</summary>
    public static ColorARGB LimeGreen => FromArgb(255, 50, 205, 50);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFAF0E6.</summary>
    public static ColorARGB Linen => FromArgb(255, 250, 240, 230);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF00FF.</summary>
    public static ColorARGB Magenta => FromArgb(255, 255, 0, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF800000.</summary>
    public static ColorARGB Maroon => FromArgb(255, 128, 0, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF66CDAA.</summary>
    public static ColorARGB MediumAquamarine => FromArgb(255, 102, 205, 170);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF0000CD.</summary>
    public static ColorARGB MediumBlue => FromArgb(255, 0, 0, 205);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFBA55D3.</summary>
    public static ColorARGB MediumOrchid => FromArgb(255, 186, 85, 211);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF9370DB.</summary>
    public static ColorARGB MediumPurple => FromArgb(255, 147, 112, 219);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF3CB371.</summary>
    public static ColorARGB MediumSeaGreen => FromArgb(255, 60, 179, 113);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF7B68EE.</summary>
    public static ColorARGB MediumSlateBlue => FromArgb(255, 123, 104, 238);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF00FA9A.</summary>
    public static ColorARGB MediumSpringGreen => FromArgb(255, 0, 250, 154);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF48D1CC.</summary>
    public static ColorARGB MediumTurquoise => FromArgb(255, 72, 209, 204);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFC71585.</summary>
    public static ColorARGB MediumVioletRed => FromArgb(255, 199, 21, 133);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF191970.</summary>
    public static ColorARGB MidnightBlue => FromArgb(255, 25, 25, 112);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF5FFFA.</summary>
    public static ColorARGB MintCream => FromArgb(255, 245, 255, 250);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFE4E1.</summary>
    public static ColorARGB MistyRose => FromArgb(255, 255, 228, 225);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFE4B5.</summary>
    public static ColorARGB Moccasin => FromArgb(255, 255, 228, 181);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFDEAD.</summary>
    public static ColorARGB NavajoWhite => FromArgb(255, 255, 222, 173);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF000080.</summary>
    public static ColorARGB Navy => FromArgb(255, 0, 0, 128);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFDF5E6.</summary>
    public static ColorARGB OldLace => FromArgb(255, 253, 245, 230);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF808000.</summary>
    public static ColorARGB Olive => FromArgb(255, 128, 128, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF6B8E23.</summary>
    public static ColorARGB OliveDrab => FromArgb(255, 107, 142, 35);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFA500.</summary>
    public static ColorARGB Orange => FromArgb(255, 255, 165, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF4500.</summary>
    public static ColorARGB OrangeRed => FromArgb(255, 255, 69, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFDA70D6.</summary>
    public static ColorARGB Orchid => FromArgb(255, 218, 112, 214);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFEEE8AA.</summary>
    public static ColorARGB PaleGoldenrod => FromArgb(255, 238, 232, 170);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF98FB98.</summary>
    public static ColorARGB PaleGreen => FromArgb(255, 152, 251, 152);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFAFEEEE.</summary>
    public static ColorARGB PaleTurquoise => FromArgb(255, 175, 238, 238);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFDB7093.</summary>
    public static ColorARGB PaleVioletRed => FromArgb(255, 219, 112, 147);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFEFD5.</summary>
    public static ColorARGB PapayaWhip => FromArgb(255, 255, 239, 213);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFDAB9.</summary>
    public static ColorARGB PeachPuff => FromArgb(255, 255, 218, 185);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFCD853F.</summary>
    public static ColorARGB Peru => FromArgb(255, 205, 133, 63);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFC0CB.</summary>
    public static ColorARGB Pink => FromArgb(255, 255, 192, 203);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFDDA0DD.</summary>
    public static ColorARGB Plum => FromArgb(255, 221, 160, 221);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFB0E0E6.</summary>
    public static ColorARGB PowderBlue => FromArgb(255, 176, 224, 230);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF800080.</summary>
    public static ColorARGB Purple => FromArgb(255, 128, 0, 128);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF0000.</summary>
    public static ColorARGB Red => FromArgb(255, 255, 0, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFBC8F8F.</summary>
    public static ColorARGB RosyBrown => FromArgb(255, 188, 143, 143);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF4169E1.</summary>
    public static ColorARGB RoyalBlue => FromArgb(255, 65, 105, 225);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF8B4513.</summary>
    public static ColorARGB SaddleBrown => FromArgb(255, 139, 69, 19);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFA8072.</summary>
    public static ColorARGB Salmon => FromArgb(255, 250, 128, 114);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF4A460.</summary>
    public static ColorARGB SandyBrown => FromArgb(255, 244, 164, 96);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF2E8B57.</summary>
    public static ColorARGB SeaGreen => FromArgb(255, 46, 139, 87);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFF5EE.</summary>
    public static ColorARGB SeaShell => FromArgb(255, 255, 245, 238);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFA0522D.</summary>
    public static ColorARGB Sienna => FromArgb(255, 160, 82, 45);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFC0C0C0.</summary>
    public static ColorARGB Silver => FromArgb(255, 192, 192, 192);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF87CEEB.</summary>
    public static ColorARGB SkyBlue => FromArgb(255, 135, 206, 235);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF6A5ACD.</summary>
    public static ColorARGB SlateBlue => FromArgb(255, 106, 90, 205);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF708090.</summary>
    public static ColorARGB SlateGray => FromArgb(255, 112, 128, 144);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFFAFA.</summary>
    public static ColorARGB Snow => FromArgb(255, 255, 250, 250);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF00FF7F.</summary>
    public static ColorARGB SpringGreen => FromArgb(255, 0, 255, 127);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF4682B4.</summary>
    public static ColorARGB SteelBlue => FromArgb(255, 70, 130, 180);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFD2B48C.</summary>
    public static ColorARGB Tan => FromArgb(255, 210, 180, 140);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF008080.</summary>
    public static ColorARGB Teal => FromArgb(255, 0, 128, 128);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFD8BFD8.</summary>
    public static ColorARGB Thistle => FromArgb(255, 216, 191, 216);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFF6347.</summary>
    public static ColorARGB Tomato => FromArgb(255, 255, 99, 71);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF40E0D0.</summary>
    public static ColorARGB Turquoise => FromArgb(255, 64, 224, 208);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFEE82EE.</summary>
    public static ColorARGB Violet => FromArgb(255, 238, 130, 238);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF5DEB3.</summary>
    public static ColorARGB Wheat => FromArgb(255, 245, 222, 179);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFFFFF.</summary>
    public static ColorARGB White => FromArgb(255, 255, 255, 255);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFF5F5F5.</summary>
    public static ColorARGB WhiteSmoke => FromArgb(255, 245, 245, 245);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FFFFFF00.</summary>
    public static ColorARGB Yellow => FromArgb(255, 255, 255, 0);

    /// <summary>Gets a ColorARGB that has an ARGB value of #FF9ACD32.</summary>
    public static ColorARGB YellowGreen => FromArgb(255, 154, 205, 50);
}