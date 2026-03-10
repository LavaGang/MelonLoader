#if LINUX || OSX
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
        if (fdIsStd || (_foundPlayerLogsStream && stream == _streamPlayerLogs))
        {
            int bufferLength = LogBuffer.Capacity - LogBuffer.Length;
            byte* bufferSpan = stackalloc byte[bufferLength];
            int nbrBytesToWrite = LibcNative.Vsnprintf(bufferSpan, bufferLength, format, vList);
            string originalLog = Encoding.UTF8.GetString(bufferSpan, nbrBytesToWrite);
            LogBuffer.Append(originalLog);
            if (LogBuffer[^1] == '\n')
            {
                LogBuffer.Remove(LogBuffer.Length - 1, 1);
                Core.PlayerLogger.Msg(LogBuffer.ToString());
                LogBuffer.Clear();
            }
            if (fdIsStd)
                return nbrBytesToWrite;

            LibcNative.Fseek(stream, 0, LibcNative.SeekEnd);
            return LibcNative.Fwrite(bufferSpan, 1, nbrBytesToWrite, stream);
        }
        return LibcNative.Vfprintf(stream, format, vList);
    }
}
#endif