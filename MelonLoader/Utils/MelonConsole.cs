﻿using System;
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
        if (Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX)
            return;
        
        ConsoleOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        ConsoleOutStream = new FileStream(new SafeFileHandle(ConsoleOutHandle, false), FileAccess.Write);
        ConsoleOutWriter = new StreamWriter(ConsoleOutStream);
        ConsoleOutWriter.AutoFlush = true;
    }

    internal static void WriteLine(string txt)
    {
        if (Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX)
        {
            Console.WriteLine(txt);            
        }
        ConsoleOutWriter.WriteLine(txt);
    }

    internal static void WriteLine(object txt)
    {
        if (Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX)
        {
            Console.WriteLine(txt.ToString());            
        }
        ConsoleOutWriter.WriteLine(txt.ToString());
    }

    internal static void WriteLine()
    {
        if (Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX)
        {
            Console.WriteLine("");            
        }
        ConsoleOutWriter.WriteLine("");
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

}