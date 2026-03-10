#if WINDOWS
using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.Bootstrap.Logging;

internal static class WindowsPlayerLogsMirroring
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int WriteFileFn(nint hFile, nint lpBuffer, int nNumberOfBytesToWrite,
        ref int lpNumberOfBytesWritten, nint lpOverlapped);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    private delegate nint CreateFileWFn(string lpFileName,
        uint dwDesiredAccess,
        int dwShareMode,
        nint lpSecurityAttributes,
        int dwCreationDisposition,
        int dwFlagsAndAttributes,
        nint hTemplateFile);

    private static readonly CreateFileWFn HookCreateFileWDelegate = HookCreateFileW;
    private static readonly WriteFileFn HookWriteFileDelegate = HookWriteFile;
    
    private static bool _foundPlayerLogsHandle;
    private static nint _logHandle;
    
    private static readonly StringBuilder LogBuffer = new(2048);

    internal static void SetupPlayerLogMirroring()
    {
        IntPtr HookCreateFileWDelegatePtr = Marshal.GetFunctionPointerForDelegate(HookCreateFileWDelegate);
        IntPtr HookWriteFileDelegatePtr = Marshal.GetFunctionPointerForDelegate(HookWriteFileDelegate);

        PltHook.InstallHooks(
        [
            ("CreateFileW", Marshal.GetFunctionPointerForDelegate(HookCreateFileWDelegate)),
            ("WriteFile", Marshal.GetFunctionPointerForDelegate(HookWriteFileDelegate))
        ]);
    }

    private static IntPtr HookCreateFileW(string lpfilename, uint dwdesiredaccess, int dwsharemode, IntPtr lpsecurityattributes, int dwcreationdisposition, int dwflagsandattributes, nint htemplatefile)
    {
        if (_foundPlayerLogsHandle)
        {
            return WindowsNative.CreateFileW(lpfilename, dwdesiredaccess, dwsharemode, lpsecurityattributes,
                dwcreationdisposition, dwflagsandattributes, htemplatefile);
        }

        if (lpfilename.EndsWith("Player.log") || lpfilename.EndsWith("output_log.txt"))
        {
            _logHandle = WindowsNative.CreateFileW(lpfilename, dwdesiredaccess, dwsharemode, lpsecurityattributes,
                dwcreationdisposition, dwflagsandattributes, htemplatefile);
            MelonDebug.Log($"Found player logs file with handle 0x{_logHandle:X} at: {lpfilename}");

            _foundPlayerLogsHandle = true;
            return _logHandle;
        }
        return WindowsNative.CreateFileW(lpfilename, dwdesiredaccess, dwsharemode, lpsecurityattributes,
            dwcreationdisposition, dwflagsandattributes, htemplatefile);
    }

    private static int HookWriteFile(nint hFile, nint lpBuffer, int nNumberOfBytesToWrite,
                                    ref int lpNumberOfBytesWritten, nint lpOverlapped)
    {
        bool writeToPlayerLog = _foundPlayerLogsHandle && hFile == _logHandle;
        bool writeToStandardHandles = hFile == ConsoleHandler.OutputHandle || hFile == ConsoleHandler.ErrorHandle;
        if (writeToPlayerLog || writeToStandardHandles)
        {
            string log = Marshal.PtrToStringUTF8(lpBuffer, nNumberOfBytesToWrite);
            LogBuffer.Append(log);
            if (LogBuffer[^1] == '\n')
            {
                LogBuffer.Remove(LogBuffer.Length - 1, 1);
                Core.PlayerLogger.Msg(LogBuffer.ToString());
                LogBuffer.Clear();
            }
            if (writeToStandardHandles)
                return 1;
        }
        return WindowsNative.WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, ref lpNumberOfBytesWritten, lpOverlapped);
    }
}
#endif