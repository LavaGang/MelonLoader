using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace MelonLoader.Utils;

internal static class MelonConsole
{
    private const int STD_OUTPUT_HANDLE = -11;
    
    internal static IntPtr ConsoleOutHandle = IntPtr.Zero;
    internal static FileStream ConsoleOutStream = null;
    internal static StreamWriter ConsoleOutWriter = null;
    
    internal static void Init()
    {
        if (MelonUtils.IsUnderWineOrSteamProton() || !MelonUtils.IsWindows || MelonLaunchOptions.Console.ShouldHide)
            return;
        
        ConsoleOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        ConsoleOutStream =
        // This enables support for net2.0. Even though the old constructor is deprecated in net35, it's still functional
#if NET35
#pragma warning disable CS0618 // Type or member is obsolete
        new FileStream(ConsoleOutHandle, FileAccess.Write);
#pragma warning restore CS0618 // Type or member is obsolete
#else
            new FileStream(new SafeFileHandle(ConsoleOutHandle, false), FileAccess.Write);
#endif

        ConsoleOutWriter = new StreamWriter(ConsoleOutStream)
        {
            AutoFlush = true
        };
    }

    private static bool ShouldNotUseWriter()
        => (MelonUtils.IsUnderWineOrSteamProton()
            || !MelonUtils.IsWindows
            || MelonLaunchOptions.Console.ShouldHide
            || (ConsoleOutWriter == null));

    internal static void WriteLine(string txt)
    {
        if (ShouldNotUseWriter())
        {
            Console.WriteLine(txt);
            return;
        }
        ConsoleOutWriter.WriteLine(txt);
    }

    internal static void WriteLine(object txt)
    {
        if (ShouldNotUseWriter())
        {
            Console.WriteLine(txt.ToString());
            return;
        }
        ConsoleOutWriter.WriteLine(txt.ToString());
    }

    internal static void WriteLine()
    {
        if (ShouldNotUseWriter())
        {
            Console.WriteLine();
            return;
        }
        ConsoleOutWriter.WriteLine("");
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

}