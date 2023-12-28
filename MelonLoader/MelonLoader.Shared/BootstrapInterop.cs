using System;
using System.Runtime.CompilerServices;
using MelonLoader.Utils;
using System.Runtime.InteropServices;

#if !NET6_0
using System.Runtime.CompilerServices;
#endif

namespace MelonLoader
{
    public static unsafe class BootstrapInterop
    {
        public static void NativeWriteLogToFile(string logString)
        {
#if !NET6_0_OR_GREATER
            if (pWriteLogToFile == IntPtr.Zero)
                throw new NullReferenceException("pWriteLogToFile is null!");
#endif

            if (!logString.EndsWith("\n"))
                logString += "\n";

            var log = logString.ToUnicodeBytes();
            var ptr = Marshal.AllocHGlobal(log.Length);
            Marshal.Copy(log, 0, ptr, log.Length);

#if NET6_0_OR_GREATER
            WriteLogToFile((byte*)ptr, log.Length);
#else
            var function = (dWriteLogToFile)Marshal.GetDelegateForFunctionPointer(pWriteLogToFile, typeof(dWriteLogToFile));
            function((byte*)ptr, log.Length);
#endif

            Marshal.FreeHGlobal(ptr);
        }

#if NET6_0_OR_GREATER
        public static delegate* unmanaged[Stdcall]<byte*, int, void> WriteLogToFile;

        public static IntPtr NativeLoadLib(string name)
            => NativeLibrary.Load(name);
        public static IntPtr NativeGetExport(IntPtr handle, string name)
            => NativeLibrary.GetExport(handle, name);
        
        public static delegate* unmanaged<void*, void*, void*> HookAttach;
        public static IntPtr NativeHookAttach(IntPtr target, IntPtr detour)
            => (IntPtr)HookAttach(target.ToPointer(), detour.ToPointer());

        public static delegate* unmanaged<void*, void> HookDetach;
        public static void NativeHookDetach(IntPtr target)
            => HookDetach(target.ToPointer());

#else
        public static void LoadInternalCalls(void* writeToLogFile)
        {
            pWriteLogToFile = (IntPtr)writeToLogFile;
        }
        
        private delegate void dWriteLogToFile(byte* log, int logLength);
        private static IntPtr pWriteLogToFile = IntPtr.Zero;
        
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr NativeLoadLib(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr NativeGetExport(IntPtr handle, string lpProcName);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern IntPtr NativeHookAttach(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void NativeHookDetach(IntPtr target);

#endif
    }
}
