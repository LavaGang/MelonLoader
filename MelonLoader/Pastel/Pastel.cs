using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MelonLoader.Pastel
{
    /// <summary>
    /// Controls colored console output by <see langword="Pastel"/>.
    /// </summary>
    public static class ConsoleExtensions
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);


        private static bool _enabled;

        private delegate string ColorFormat(string input, Color color);
        private delegate string HexColorFormat(string input, string hexColor);

        private enum ColorPlane : byte
        {
            Foreground,
            Background
        }

        private const string _formatStringStart = "\u001b[{0};2;";
        private const string _formatStringColor = "{1};{2};{3}m";
        private const string _formatStringContent = "{4}";
        private const string _formatStringEnd = "\u001b[0m";
        private static readonly string _formatStringFull = $"{_formatStringStart}{_formatStringColor}{_formatStringContent}{_formatStringEnd}";


        private static readonly Dictionary<ColorPlane, string> _planeFormatModifiers = new Dictionary<ColorPlane, string>
        {
            [ColorPlane.Foreground] = "38",
            [ColorPlane.Background] = "48"
        };



        private static readonly Regex _closeNestedPastelStringRegex1 = new Regex($"({_formatStringEnd.Replace("[", @"\[")})+", RegexOptions.Compiled);
        private static readonly Regex _closeNestedPastelStringRegex2 = new Regex($"(?<!^)(?<!{_formatStringEnd.Replace("[", @"\[")})(?<!{string.Format($"{_formatStringStart.Replace("[", @"\[")}{_formatStringColor}", new[] { $"(?:{_planeFormatModifiers[ColorPlane.Foreground]}|{_planeFormatModifiers[ColorPlane.Background]})" }.Concat(Enumerable.Repeat(@"\d{1,3}", 3)).Cast<object>().ToArray())})(?:{string.Format(_formatStringStart.Replace("[", @"\["), $"(?:{_planeFormatModifiers[ColorPlane.Foreground]}|{_planeFormatModifiers[ColorPlane.Background]})")})", RegexOptions.Compiled);

        private static readonly Dictionary<ColorPlane, Regex> _closeNestedPastelStringRegex3 = new Dictionary<ColorPlane, Regex>
        {
            [ColorPlane.Foreground] = new Regex($"(?:{_formatStringEnd.Replace("[", @"\[")})(?!{string.Format(_formatStringStart.Replace("[", @"\["), _planeFormatModifiers[ColorPlane.Foreground])})(?!$)", RegexOptions.Compiled),
            [ColorPlane.Background] = new Regex($"(?:{_formatStringEnd.Replace("[", @"\[")})(?!{string.Format(_formatStringStart.Replace("[", @"\["), _planeFormatModifiers[ColorPlane.Background])})(?!$)", RegexOptions.Compiled)
        };



        private static readonly Func<string, int> _parseHexColor = hc => int.Parse(hc.Replace("#", ""), NumberStyles.HexNumber);

        private static readonly Func<string, Color, ColorPlane, string> _colorFormat = (i, c, p) => string.Format(_formatStringFull, _planeFormatModifiers[p], c.R, c.G, c.B, CloseNestedPastelStrings(i, c, p));
        private static readonly Func<string, string, ColorPlane, string> _colorHexFormat = (i, c, p) => _colorFormat(i, Color.FromArgb(_parseHexColor(c)), p);

        private static readonly ColorFormat _noColorOutputFormat = (i, _) => i;
        private static readonly HexColorFormat _noHexColorOutputFormat = (i, _) => i;

        private static readonly ColorFormat _foregroundColorFormat = (i, c) => _colorFormat(i, c, ColorPlane.Foreground);
        private static readonly HexColorFormat _foregroundHexColorFormat = (i, c) => _colorHexFormat(i, c, ColorPlane.Foreground);

        private static readonly ColorFormat _backgroundColorFormat = (i, c) => _colorFormat(i, c, ColorPlane.Background);
        private static readonly HexColorFormat _backgroundHexColorFormat = (i, c) => _colorHexFormat(i, c, ColorPlane.Background);



        private static readonly Dictionary<bool, Dictionary<ColorPlane, ColorFormat>> _colorFormatFuncs = new Dictionary<bool, Dictionary<ColorPlane, ColorFormat>>(new Dictionary<bool, Dictionary<ColorPlane, ColorFormat>>
        {
            [false] = new Dictionary<ColorPlane, ColorFormat>(new Dictionary<ColorPlane, ColorFormat>
            {
                [ColorPlane.Foreground] = _noColorOutputFormat,
                [ColorPlane.Background] = _noColorOutputFormat
            }),
            [true] = new Dictionary<ColorPlane, ColorFormat>(new Dictionary<ColorPlane, ColorFormat>
            {
                [ColorPlane.Foreground] = _foregroundColorFormat,
                [ColorPlane.Background] = _backgroundColorFormat
            })
        });
        private static readonly Dictionary<bool, Dictionary<ColorPlane, HexColorFormat>> _hexColorFormatFuncs = new Dictionary<bool, Dictionary<ColorPlane, HexColorFormat>>(new Dictionary<bool, Dictionary<ColorPlane, HexColorFormat>>
        {
            [false] = new Dictionary<ColorPlane, HexColorFormat>(new Dictionary<ColorPlane, HexColorFormat>
            {
                [ColorPlane.Foreground] = _noHexColorOutputFormat,
                [ColorPlane.Background] = _noHexColorOutputFormat
            }),
            [true] = new Dictionary<ColorPlane, HexColorFormat>(new Dictionary<ColorPlane, HexColorFormat>
            {
                [ColorPlane.Foreground] = _foregroundHexColorFormat,
                [ColorPlane.Background] = _backgroundHexColorFormat
            })
        });




        static ConsoleExtensions()
        {
            if (MelonUtils.IsUnix || MelonUtils.IsMac)
            {
                Enable();
                return;
            }
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

            var enable = GetConsoleMode(iStdOut, out var outConsoleMode)
                         && SetConsoleMode(iStdOut, outConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);


            if (Environment.GetEnvironmentVariable("NO_COLOR") == null)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }







        /// <summary>
        /// Enables any future console color output produced by Pastel.
        /// </summary>
        public static void Enable()
        {
            _enabled = true;
        }

        /// <summary>
        /// Disables any future console color output produced by Pastel.
        /// </summary>
        public static void Disable()
        {
            _enabled = false;
        }


        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string Pastel(this string input, Color color)
        {
            return _colorFormatFuncs[_enabled][ColorPlane.Foreground](input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string Pastel(this string input, string hexColor)
        {
            return _hexColorFormatFuncs[_enabled][ColorPlane.Foreground](input, hexColor);
        }



        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBg(this string input, Color color)
        {
            return _colorFormatFuncs[_enabled][ColorPlane.Background](input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelBg(this string input, string hexColor)
        {
            return _hexColorFormatFuncs[_enabled][ColorPlane.Background](input, hexColor);
        }



        private static string CloseNestedPastelStrings(string input, Color color, ColorPlane colorPlane)
        {
            var closedString = _closeNestedPastelStringRegex1.Replace(input, _formatStringEnd);

            closedString = _closeNestedPastelStringRegex2.Replace(closedString, $"{_formatStringEnd}$0");
            closedString = _closeNestedPastelStringRegex3[colorPlane].Replace(closedString, $"$0{string.Format($"{_formatStringStart}{_formatStringColor}", _planeFormatModifiers[colorPlane], color.R, color.G, color.B)}");

            return closedString;
        }
    }
}