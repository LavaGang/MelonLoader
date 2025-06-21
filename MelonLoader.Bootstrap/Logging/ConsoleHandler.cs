using MelonLoader.Logging;
using System.Runtime.InteropServices;
using System.Text;
using MelonLoader.Bootstrap.Utils;

namespace MelonLoader.Bootstrap;

internal static class ConsoleHandler
{
#if WINDOWS
    internal static nint OutputHandle;
    internal static nint ErrorHandle;

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int CloseHandleFn(uint hObject);
    private static readonly CloseHandleFn HookCloseHandleDelegate = HookCloseHandle;
#endif

#if LINUX
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int FCloseFn(nint stream);
    private static readonly FCloseFn HookFCloseDelegate = HookFClose;
#endif

    public static bool IsOpen { get; private set; }
    public static bool HasOwnWindow { get; private set; }

    public static void OpenConsole(bool onTop, string? title)
    {
#if WINDOWS
        // Do not create a new window if a window already exists or the output is being redirected.
        // On Wine, we always want to show the window because it's possible the handle isn't null due to Wine itself
        var consoleWindow = WindowsNative.GetConsoleWindow();
        var stdOut = WindowsNative.GetStdHandle(WindowsNative.StdOutputHandle);
        if (consoleWindow == 0 && (stdOut == 0 || WineUtils.IsWine))
        {
            WindowsNative.AllocConsole();
            consoleWindow = WindowsNative.GetConsoleWindow();
            if (consoleWindow == 0)
                return;

            HasOwnWindow = true;

            if (onTop)
                WindowsNative.SetWindowPos(consoleWindow, -1, 0, 0, 0, 0, 0x0001 | 0x0002);
        }

        if (consoleWindow != 0)
#endif
        {
            if (title != null)
                Console.Title = title;
        }

#if LINUX
        PltHook.InstallHooks(
        [
            ("fclose", Marshal.GetFunctionPointerForDelegate(HookFCloseDelegate))
        ]);
#endif

#if WINDOWS
        PltHook.InstallHooks(
        [
            ("CloseHandle", Marshal.GetFunctionPointerForDelegate(HookCloseHandleDelegate)),
        ]);
        
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
        Console.SetIn(new StreamReader(Console.OpenStandardInput()));

        Console.OutputEncoding = Encoding.UTF8;

        OutputHandle = WindowsNative.GetStdHandle(WindowsNative.StdOutputHandle);
        ErrorHandle = WindowsNative.GetStdHandle(WindowsNative.StdErrorHandle);
#endif

        IsOpen = true;
    }

    public static void NullHandles()
    {
#if WINDOWS
        WindowsNative.SetStdHandle(WindowsNative.StdOutputHandle, 0);
        WindowsNative.SetStdHandle(WindowsNative.StdErrorHandle, 0);
#endif
    }

    public static void ResetHandles()
    {
#if WINDOWS
        WindowsNative.SetStdHandle(WindowsNative.StdOutputHandle, OutputHandle);
        WindowsNative.SetStdHandle(WindowsNative.StdErrorHandle, ErrorHandle);
#endif
    }

#if LINUX
    private static int HookFClose(nint stream)
    {
        int fd = LibcNative.Fileno(stream);
        if (fd is LibcNative.Stdout or LibcNative.Stderr)
        {
            MelonDebug.Log($"Prevented the fclose on {(fd == LibcNative.Stdout ? "stdout" : "stderr")}");
            return 0;
        }
        return LibcNative.FClose(stream);
    }
#endif

#if WINDOWS
    private static int HookCloseHandle(uint hObject)
    {
        if (hObject == OutputHandle || hObject == ErrorHandle)
        {
            MelonDebug.Log($"Prevented the CloseHandle of {(hObject == OutputHandle ? "stdout" : "stderr")}");
            return 1;
        }

        return WindowsNative.CloseHandle(hObject);
    }
#endif

    public static ConsoleColor GetClosestConsoleColor(ColorARGB color)
    {
        var index = color.R > 128 | color.G > 128 | color.B > 128 ? 8 : 0; // Bright bit
        index |= color.R > 64 ? 4 : 0; // Red bit
        index |= color.G > 64 ? 2 : 0; // Green bit
        index |= color.B > 64 ? 1 : 0; // Blue bit
        return (ConsoleColor)index;
    }
}