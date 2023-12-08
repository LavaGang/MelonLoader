using System;

#if NET6_0
using System.Runtime.InteropServices;
using MelonLoader.Utils;
#else
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.Utils;
#endif

namespace MelonLoader
{
    public static unsafe class BootstrapInterop
    {
#if NET6_0
        public static delegate* unmanaged[Stdcall]<byte*, int, void> WriteLogToFile;

        public static void NativeWriteLogToFile(string logString)
        {
            if (!logString.EndsWith("\n"))
                logString += "\n";
            
            var log = MelonUtils.StringToBytes(logString);
            var ptr = Marshal.AllocHGlobal(log.Length);
            Marshal.Copy(log, 0, ptr, log.Length);
            
            WriteLogToFile((byte*)ptr, log.Length);
            
            Marshal.FreeHGlobal(ptr);
        }

        public static IntPtr NativeLoadLib(string name)
            => NativeLibrary.Load(name);
        public static IntPtr NativeGetExport(IntPtr handle, string name)
            => NativeLibrary.GetExport(handle, name);

        public delegate void* dHookAttach(void* target, void* detour);
        public static dHookAttach HookAttach;
        public static IntPtr NativeHookAttach(IntPtr target, IntPtr detour)
        {
            var trampoline = new IntPtr(HookAttach(target.ToPointer(), detour.ToPointer()));
            //MelonLogger.Msg(trampoline.ToString("X"));
            return trampoline;
        }

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

        public static void NativeWriteLogToFile(string logString)
        {
            if (pWriteLogToFile == IntPtr.Zero)
                throw new NullReferenceException("pWriteLogToFile is null!");
            
            if (!logString.EndsWith("\n"))
                logString += "\n";
            
            var log = MelonUtils.StringToBytes(logString);
            var ptr = Marshal.AllocHGlobal(log.Length);
            Marshal.Copy(log, 0, ptr, log.Length);

            var function = (dWriteLogToFile)Marshal.GetDelegateForFunctionPointer(pWriteLogToFile, typeof(dWriteLogToFile));
            function((byte*)ptr, log.Length);
            
            Marshal.FreeHGlobal(ptr);
        }
        
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
