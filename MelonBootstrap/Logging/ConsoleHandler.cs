using MelonLoader.Bootstrap.Logging;
using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap;

internal static partial class ConsoleHandler
{
#if WINDOWS
    private const uint StdOutputHandle = 4294967285;
    private const uint StdErrorHandle = 4294967284;

    private static nint outputHandle;
#endif

    public static bool Hidden { get; private set; } = ArgParser.IsDefined("melonloader.hideconsole");

    public static void OpenConsole(string version, bool onTop)
    {
        if (Hidden)
            return;

#if WINDOWS
        AllocConsole();

        var consoleWindow = GetConsoleWindow();
        if (consoleWindow == 0)
        {
            Hidden = true;
            return;
        }

        if (onTop)
            SetWindowPos(consoleWindow, -1, 0, 0, 0, 0, 0x0001 | 0x0002);
#endif

        if (!ArgParser.IsDefined("melonloader.consoledst"))
        {
            var title = "MelonLoader v" + version;
            if (Core.Debug)
                title = "[Debug] " + title;

            Console.Title = title;
        }

#if WINDOWS
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
        Console.SetIn(new StreamReader(Console.OpenStandardInput()));

        Console.OutputEncoding = Encoding.Unicode;

        outputHandle = GetStdHandle(StdOutputHandle);
#endif
    }

    public static void NullHandles()
    {
#if WINDOWS
        SetStdHandle(StdOutputHandle, 0);
        SetStdHandle(StdErrorHandle, 0);
#endif
    }

    public static void ResetHandles()
    {
#if WINDOWS
        SetStdHandle(StdOutputHandle, outputHandle);
        SetStdHandle(StdErrorHandle, outputHandle);
#endif
    }

    public static ConsoleColor GetClosestConsoleColor(ColorRGB color)
    {
        var index = color.R > 128 | color.G > 128 | color.B > 128 ? 8 : 0; // Bright bit
        index |= color.R > 64 ? 4 : 0; // Red bit
        index |= color.G > 64 ? 2 : 0; // Green bit
        index |= color.B > 64 ? 1 : 0; // Blue bit
        return (ConsoleColor)index;
    }

#if WINDOWS
    [LibraryImport("kernel32.dll")]
    private static partial nint GetStdHandle(uint nStdHandle);

    [LibraryImport("kernel32.dll")]
    private static partial void SetStdHandle(uint nStdHandle, nint handle);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AllocConsole();

    [LibraryImport("kernel32.dll")]
    private static partial nint GetConsoleWindow();

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
#endif
}