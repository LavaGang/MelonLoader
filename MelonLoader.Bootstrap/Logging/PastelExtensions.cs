using System.Drawing;
using MelonLoader.Logging;
using Pastel;

namespace MelonLoader.Bootstrap.Logging;

internal static class PastelExtensions
{
    internal static string Pastel(this string input, ColorARGB color)
    {
        return input.AsSpan().Pastel(color);
    }

    internal static string Pastel(in this ReadOnlySpan<char> input, ColorARGB color)
    {
        return input.Pastel(Color.FromArgb(color.R, color.G, color.B));
    }
}
