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
            NativeLib = MelonUtils.GetNativeLibrary(filepath);
            if (NativeLib == IntPtr.Zero)
                throw new Exception("Unable to Load bHaptics Native Library!");

            NativeLib.GetNativeLibraryExport(nameof(Initialise)).FunctionPointerToDelegate(out Initialise);
            NativeLib.GetNativeLibraryExport(nameof(TurnOff)).FunctionPointerToDelegate(out TurnOff);
            NativeLib.GetNativeLibraryExport(nameof(Destroy)).FunctionPointerToDelegate(out Destroy);
            NativeLib.GetNativeLibraryExport(nameof(RegisterFeedback)).FunctionPointerToDelegate(out RegisterFeedback);
            NativeLib.GetNativeLibraryExport(nameof(RegisterFeedbackFromTactFile)).FunctionPointerToDelegate(out RegisterFeedbackFromTactFile);
            NativeLib.GetNativeLibraryExport(nameof(RegisterFeedbackFromTactFileReflected)).FunctionPointerToDelegate(out RegisterFeedbackFromTactFileReflected);
            NativeLib.GetNativeLibraryExport(nameof(SubmitRegistered)).FunctionPointerToDelegate(out SubmitRegistered);
            NativeLib.GetNativeLibraryExport(nameof(SubmitRegisteredStartMillis)).FunctionPointerToDelegate(out SubmitRegisteredStartMillis);
            NativeLib.GetNativeLibraryExport(nameof(SubmitRegisteredWithOption)).FunctionPointerToDelegate(out SubmitRegisteredWithOption);
            NativeLib.GetNativeLibraryExport(nameof(SubmitByteArray)).FunctionPointerToDelegate(out SubmitByteArray);
            NativeLib.GetNativeLibraryExport(nameof(SubmitPathArray)).FunctionPointerToDelegate(out SubmitPathArray);
            NativeLib.GetNativeLibraryExport(nameof(IsFeedbackRegistered)).FunctionPointerToDelegate(out IsFeedbackRegistered);
            NativeLib.GetNativeLibraryExport(nameof(IsPlaying)).FunctionPointerToDelegate(out IsPlaying);
            NativeLib.GetNativeLibraryExport(nameof(IsPlayingKey)).FunctionPointerToDelegate(out IsPlayingKey);
            NativeLib.GetNativeLibraryExport(nameof(TurnOffKey)).FunctionPointerToDelegate(out TurnOffKey);
            NativeLib.GetNativeLibraryExport(nameof(IsDevicePlaying)).FunctionPointerToDelegate(out IsDevicePlaying);
            NativeLib.GetNativeLibraryExport(nameof(TryGetResponseForPosition)).FunctionPointerToDelegate(out TryGetResponseForPosition);
            NativeLib.GetNativeLibraryExport(nameof(TryGetExePath)).FunctionPointerToDelegate(out TryGetExePath);
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
        internal delegate void dRegisterFeedback(string str, string tactFileStr);
        internal static dRegisterFeedback RegisterFeedback;
        internal static dRegisterFeedback RegisterFeedbackFromTactFile;
        internal static dRegisterFeedback RegisterFeedbackFromTactFileReflected;

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
    }
}
