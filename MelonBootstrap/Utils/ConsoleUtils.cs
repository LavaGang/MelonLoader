using System.Runtime.InteropServices;

namespace MelonBootstrap.Utils;

internal static partial class ConsoleUtils
{
    private const uint StdOutputHandle = 4294967285;
    private const uint StdErrorHandle = 4294967284;

    private static nint outputHandle;

    public static void OpenConsole(bool debugMode, string version, bool onTop)
    {
        AllocConsole();

        var consoleWindow = GetConsoleWindow();
        if (consoleWindow == 0)
            return;

        if (onTop)
            SetWindowPos(consoleWindow, -1, 0, 0, 0, 0, 0x0001 | 0x0002);

        var title = "MelonLoader v" + version;
        if (debugMode)
            title = "[Debug] " + title;

        Console.Title = title;

        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
        Console.SetIn(new StreamReader(Console.OpenStandardInput()));

        outputHandle = GetStdHandle(StdOutputHandle);

        SetStdHandle(StdOutputHandle, 0);
        SetStdHandle(StdErrorHandle, 0);
    }

    public static void ResetHandles()
    {
        SetStdHandle(StdOutputHandle, outputHandle);
        SetStdHandle(StdErrorHandle, outputHandle);
    }

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
}