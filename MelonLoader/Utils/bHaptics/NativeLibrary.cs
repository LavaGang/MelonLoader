using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MelonLoader
{
    internal static class bHaptics_NativeLibrary
    {
        private static IntPtr NativeLib = IntPtr.Zero;

        internal static void Load()
        {
            string filename = "bHaptics.x" + (MelonUtils.IsGame32Bit() ? "86" : "64") + ".dll";
            string filepath = Path.Combine(Path.Combine(Path.Combine(MelonUtils.GameDirectory, "MelonLoader"), "Dependencies"), filename);
            if (!File.Exists(filepath))
                throw new Exception("Failed to find " + filename + "!");
            NativeLib = LoadLibrary(filepath);
            if (NativeLib == IntPtr.Zero)
                throw new Exception("Unable to Load bHaptics Native Library!");
            GetDelegateFromProcAddress("Initialise", out Initialise);
            GetDelegateFromProcAddress("TurnOff", out TurnOff);
            GetDelegateFromProcAddress("Destroy", out Destroy);
            GetDelegateFromProcAddress("RegisterFeedbackFromTactFile", out RegisterFeedbackFromTactFile);
            GetDelegateFromProcAddress("RegisterFeedbackFromTactFileReflected", out RegisterFeedbackFromTactFileReflected);
            GetDelegateFromProcAddress("SubmitRegistered", out SubmitRegistered);
            GetDelegateFromProcAddress("SubmitRegisteredStartMillis", out SubmitRegisteredStartMillis);
            GetDelegateFromProcAddress("SubmitRegisteredWithOption", out SubmitRegisteredWithOption);
            GetDelegateFromProcAddress("SubmitByteArray", out SubmitByteArray);
            GetDelegateFromProcAddress("SubmitPathArray", out SubmitPathArray);
            GetDelegateFromProcAddress("IsFeedbackRegistered", out IsFeedbackRegistered);
            GetDelegateFromProcAddress("IsPlaying", out IsPlaying);
            GetDelegateFromProcAddress("IsPlayingKey", out IsPlayingKey);
            GetDelegateFromProcAddress("TurnOffKey", out TurnOffKey);
            GetDelegateFromProcAddress("IsDevicePlaying", out IsDevicePlaying);
            GetDelegateFromProcAddress("TryGetResponseForPosition", out TryGetResponseForPosition);
            GetDelegateFromProcAddress("TryGetExePath", out TryGetExePath);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dInitialise(string appId, string appName);
        internal static dInitialise Initialise;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dTurnOff();
        internal static dTurnOff TurnOff;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dDestroy();
        internal static dDestroy Destroy;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dRegisterFeedbackFromTactFile(string str, string tactFileStr);
        internal static dRegisterFeedbackFromTactFile RegisterFeedbackFromTactFile;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dRegisterFeedbackFromTactFileReflected(string str, string tactFileStr);
        internal static dRegisterFeedbackFromTactFileReflected RegisterFeedbackFromTactFileReflected;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dSubmitRegistered(string key);
        internal static dSubmitRegistered SubmitRegistered;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dSubmitRegisteredStartMillis(string key, int startTimeMillis);
        internal static dSubmitRegisteredStartMillis SubmitRegisteredStartMillis;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dSubmitRegisteredWithOption(string key, string altKey, float intensity, float duration, float offsetX, float offsetY);
        internal static dSubmitRegisteredWithOption SubmitRegisteredWithOption;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dSubmitByteArray(string key, bHaptics.PositionType pos, byte[] bytes, int length, int durationMillis);
        internal static dSubmitByteArray SubmitByteArray;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dSubmitPathArray(string key, bHaptics.PositionType pos, bHaptics.PathPoint[] points, int length, int durationMillis);
        internal static dSubmitPathArray SubmitPathArray;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool dIsFeedbackRegistered(string key);
        internal static dIsFeedbackRegistered IsFeedbackRegistered;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool dIsPlaying();
        internal static dIsPlaying IsPlaying;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool dIsPlayingKey(string key);
        internal static dIsPlayingKey IsPlayingKey;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dTurnOffKey(string key);
        internal static dTurnOffKey TurnOffKey;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool dIsDevicePlaying(bHaptics.PositionType pos);
        internal static dIsDevicePlaying IsDevicePlaying;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool dTryGetResponseForPosition(bHaptics.PositionType pos, out bHaptics.FeedbackStatus status);
        internal static dTryGetResponseForPosition TryGetResponseForPosition;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool dTryGetExePath(byte[] buf, ref int size);
        internal static dTryGetExePath TryGetExePath;

        private static void GetDelegateFromProcAddress<T>(string name, out T output) where T : Delegate
        {
            IntPtr ptr = GetProcAddress(NativeLib, name);
            if (ptr == IntPtr.Zero)
                throw new Exception("Unable to Find " + name + " Export!");
            output = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
        }
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpLibFileName);
        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
    }
}
