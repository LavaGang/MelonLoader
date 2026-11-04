#if LINUX || OSX
using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.Logging;

internal static class UnixPlayerLogsMirroring
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint Fopen64Fn(string pathnane, string mode);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private delegate int VfprintfFn(nint stream, string format, nint vList);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private delegate int VfprintfChkFn(nint stream, int flag, string format, nint vList);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int Dup2Fn(int oldFd, int newFd);

    private static readonly Fopen64Fn HookFopen64Delegate = HookFopen64;
    private static readonly VfprintfFn HookVfprintfDelegate = HookVfprintf;
    private static readonly VfprintfChkFn HookVfprintfChkDelegate = HookVfprintfChk;
    private static readonly Dup2Fn HookDup2Delegate = HookDup2;
    
    private static bool _foundPlayerLogsStream;
    private static nint _streamPlayerLogs = IntPtr.Zero;
    
    private static readonly StringBuilder LogBuffer = new(2048);

    // Unity logs from several threads; LogBuffer is shared between them.
    private static readonly object LogBufferLock = new();

    // Most lines fit here. Longer ones are formatted again into a heap buffer of the exact size.
    private const int StackFormatBufferSize = 1024;

    internal static void SetupPlayerLogMirroring()
    {
        PltHook.InstallHooks(
        [
#if OSX
            ("fopen", Marshal.GetFunctionPointerForDelegate(HookFopen64Delegate)),
#else
            ("fopen64", Marshal.GetFunctionPointerForDelegate(HookFopen64Delegate)),
#endif
            ("vfprintf", Marshal.GetFunctionPointerForDelegate(HookVfprintfDelegate)),
            ("__vfprintf_chk", Marshal.GetFunctionPointerForDelegate(HookVfprintfChkDelegate)),
            ("dup2", Marshal.GetFunctionPointerForDelegate(HookDup2Delegate))
        ]);
    }

    private static nint HookFopen64(string pathName, string mode)
    {
        if (_foundPlayerLogsStream)
            return LibcNative.Fopen64(pathName, mode);

        if (mode.Contains('w') && (pathName.EndsWith("Player.log") || pathName.EndsWith("output_log.txt")))
        {
            _streamPlayerLogs = LibcNative.Fopen64(pathName, mode);
            MelonDebug.Log($"Found player logs file with fd {LibcNative.Fileno(_streamPlayerLogs)} at: {pathName}");

            _foundPlayerLogsStream = true;
            return _streamPlayerLogs;
        }
        return LibcNative.Fopen64(pathName, mode);
    }

    private static int HookDup2(int oldFd, int newFd)
    {
        if (newFd is LibcNative.Stdout or LibcNative.Stderr)
        {
            MelonDebug.Log($"Prevented the dup2 on {(newFd == LibcNative.Stdout ? "stdout" : "stderr")}");
            return newFd;
        }

        return LibcNative.Dup2(oldFd, newFd);
    }

    private static int HookVfprintfChk(nint stream, int flag, string format, nint vList)
    {
        return HookVfprintf(stream, format, vList);
    }

    private static unsafe int HookVfprintf(nint stream, string format, nint vList)
    {
        int fd = LibcNative.Fileno(stream);
        bool fdIsStd = fd is LibcNative.Stdout or LibcNative.Stderr;
        if (!fdIsStd && !(_foundPlayerLogsStream && stream == _streamPlayerLogs))
            return LibcNative.Vfprintf(stream, format, vList);

        byte* stackBuffer = stackalloc byte[StackFormatBufferSize];
        int length = Format(stackBuffer, StackFormatBufferSize, format, vList);
        if (length < 0)
            return length;

        if (length < StackFormatBufferSize || !CanFormatTwice)
            return Mirror(stream, fdIsStd, stackBuffer, Math.Min(length, StackFormatBufferSize - 1));

        byte[] heapBuffer = GC.AllocateUninitializedArray<byte>(length + 1);
        fixed (byte* heap = heapBuffer)
        {
            length = Format(heap, heapBuffer.Length, format, vList);
            return length < 0 ? length : Mirror(stream, fdIsStd, heap, length);
        }
    }

    private static unsafe int Mirror(nint stream, bool fdIsStd, byte* text, int length)
    {
        string? line = null;
        lock (LogBufferLock)
        {
            LogBuffer.Append(Encoding.UTF8.GetString(text, length));
            if (LogBuffer.Length > 0 && LogBuffer[^1] == '\n')
            {
                line = LogBuffer.ToString(0, LogBuffer.Length - 1);
                LogBuffer.Clear();
            }
        }
        if (line != null)
            Core.PlayerLogger.Msg(line);

        if (fdIsStd)
            return length;

        LibcNative.Fseek(stream, 0, LibcNative.SeekEnd);
        return LibcNative.Fwrite(text, 1, length, stream);
    }

    // x86-64: a va_list is a 24-byte state block passed by pointer, and vsnprintf advances it, so
    // Format works on a copy each time (what va_copy does there). x86 and arm64 macOS: it is a
    // plain pointer passed by value, so the callee only ever advances its own copy.
    private static bool CanFormatTwice => RuntimeInformation.ProcessArchitecture switch
    {
        Architecture.X64 or Architecture.X86 => true,
        Architecture.Arm64 => OperatingSystem.IsMacOS(),
        _ => false,
    };

    private static unsafe int Format(byte* buffer, int size, string format, nint vList)
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
            return LibcNative.Vsnprintf(buffer, size, format, vList);

        byte* copy = stackalloc byte[24];
        Buffer.MemoryCopy((void*)vList, copy, 24, 24);
        return LibcNative.Vsnprintf(buffer, size, format, (nint)copy);
    }
}
#endif