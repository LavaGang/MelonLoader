using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
#pragma warning disable 0649

namespace MelonLoader
{
    public static class bHaptics
    {
        private static NativeExports NativeLib = null;
        private static readonly int MaxBufferSize = 20;
        private static bool _waserror = false;
        public static bool WasError { get => _waserror; internal set { if (value == true) MelonLogger.Warning("Disabling bHaptics API..."); _waserror = value; } }

        internal static void Load()
        {
            string filename = "bHaptics.x" + (MelonUtils.IsGame32Bit() ? "86" : "64") + ".dll";
            string filepath = Path.Combine(Path.Combine(Path.Combine(MelonUtils.GameDirectory, "MelonLoader"), "Dependencies"), filename);
            if (!File.Exists(filepath))
            {
                _waserror = true;
                return;
            }

            try
            {
                NativeLib = NativeLibrary.ReflectiveLoad<NativeExports>(filepath);
                if (NativeLib == null)
                    throw new Exception("Failed to Setup Native Library!");

                byte[] buf = new byte[500];
                int size = 0;
                if (!NativeLib.TryGetExePath(buf, ref size))
                    _waserror = true;
                else
                    MelonDebug.Msg("bHaptics Native Library Loaded!");
            }
            catch (Exception ex) { MelonLogger.Warning($"bHaptics.Load Exception: {ex}"); WasError = true; }
        }

        internal static void Start()
        {
            if (_waserror)
                return;
            try { NativeLib.Initialise("MelonLoader", MelonUtils.GameName.Replace(" ", "_")); }
            catch (Exception ex) { MelonLogger.Warning($"bHaptics.Start Exception: {ex}"); WasError = true; }
            MelonLogger.Msg("bHaptics Support Initialized!");
        }

        internal static void Quit()
        {
            if (_waserror)
                return;
            try
            {
                NativeLib.TurnOff();
                NativeLib.Destroy();
            }
            catch (Exception ex) { MelonLogger.Warning($"bHaptics.Quit Exception: {ex}"); WasError = true; }
        }

        public static bool IsPlaying() => (!_waserror && NativeLib.IsPlaying());
        public static bool IsPlaying(string key) => (!_waserror && NativeLib.IsPlayingKey(key));
        public static bool IsDeviceConnected(PositionType type) => (!_waserror && NativeLib.IsDevicePlaying(type));
        public static bool IsDeviceConnected(DeviceType type, bool isLeft = true) => (!_waserror && NativeLib.IsDevicePlaying(DeviceTypeToPositionType(type, isLeft)));
        public static bool IsFeedbackRegistered(string key) => (!_waserror && NativeLib.IsFeedbackRegistered(key));

        public static void RegisterFeedback(string key, string tactFileStr) { if (!_waserror) NativeLib.RegisterFeedback(key, tactFileStr); }
        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr) { if (!_waserror) NativeLib.RegisterFeedbackFromTactFile(key, tactFileStr); }
        public static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr) { if (!_waserror) NativeLib.RegisterFeedbackFromTactFileReflected(key, tactFileStr); }

        public static void SubmitRegistered(string key) { if (!_waserror) NativeLib.SubmitRegistered(key); }
        public static void SubmitRegistered(string key, int startTimeMillis) => NativeLib.SubmitRegisteredStartMillis(key, startTimeMillis);
        public static void SubmitRegistered(string key, string altKey, ScaleOption option) { if (!_waserror) NativeLib.SubmitRegisteredWithOption(key, altKey, option.Intensity, option.Duration, 1f, 1f); }
        public static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption) { if (!_waserror) NativeLib.SubmitRegisteredWithOption(key, altKey, sOption.Intensity, sOption.Duration, rOption.OffsetX, rOption.OffsetY); }

        public static void TurnOff() { if (!_waserror) NativeLib.TurnOff(); }
        public static void TurnOff(string key) { if (!_waserror) NativeLib.TurnOffKey(key); }

        public static void Submit(string key, DeviceType type, bool isLeft, byte[] bytes, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), bytes, durationMillis);
        public static void Submit(string key, PositionType position, byte[] bytes, int durationMillis)
        {
            if (_waserror)
                return;
            int bytes_size = bytes.Length;
            if (bytes_size != MaxBufferSize)
            {
                byte[] newbytes = new byte[MaxBufferSize];
                for (int i = 0; i < bytes_size; i++)
                    newbytes[i] = bytes[i];
            }
            NativeLib.SubmitByteArray(key, position, bytes, MaxBufferSize, durationMillis);
        }
        public static void Submit(string key, DeviceType type, bool isLeft, List<DotPoint> points, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        public static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            if (_waserror)
                return;
            byte[] bytes = new byte[MaxBufferSize];
            for (var i = 0; i < points.Count; i++)
            {
                DotPoint point = points[i];
                if ((point.Index < 0) || (point.Index > MaxBufferSize))
                    continue;
                bytes[point.Index] = (byte)point.Intensity;
            }
            NativeLib.SubmitByteArray(key, position, bytes, MaxBufferSize, durationMillis);
        }
        public static void Submit(string key, DeviceType type, bool isLeft, List<PathPoint> points, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        public static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            if (_waserror)
                return;
            PathPoint[] pathPoints = points.ToArray();
            NativeLib.SubmitPathArray(key, position, pathPoints, pathPoints.Length, durationMillis);
        }

        public static FeedbackStatus GetCurrentFeedbackStatus(DeviceType type, bool isLeft = true) => GetCurrentFeedbackStatus(DeviceTypeToPositionType(type, isLeft));
        public static FeedbackStatus GetCurrentFeedbackStatus(PositionType pos)
        {
            if (_waserror)
                return default;
            FeedbackStatus status;
            NativeLib.TryGetResponseForPosition(pos, out status);
            return status;
        }

        public static PositionType DeviceTypeToPositionType(DeviceType pos, bool isLeft = true)
        {
            switch (pos)
            {
                case DeviceType.Tactal:
                    return PositionType.Head;
                case DeviceType.TactSuit:
                    return PositionType.Vest;
                case DeviceType.Tactosy_arms:
                    return isLeft ? PositionType.ForearmL : PositionType.ForearmR;
                case DeviceType.Tactosy_feet:
                    return isLeft ? PositionType.FootL : PositionType.FootR;
                case DeviceType.Tactosy_hands:
                    return isLeft ? PositionType.HandL : PositionType.HandR;
                case DeviceType.None:
                    break;
            }
            return PositionType.Head;
        }

        public enum DeviceType
        {
            None = 0,
            Tactal = 1,
            TactSuit = 2,
            Tactosy_arms = 3,
            Tactosy_hands = 4,
            Tactosy_feet = 5
        }

        public enum PositionType
        {
            All = 0,
            Left = 1, Right = 2,
            Vest = 3,
            Head = 4,
            Racket = 5,
            HandL = 6, HandR = 7,
            FootL = 8, FootR = 9,
            ForearmL = 10, ForearmR = 11,
            VestFront = 201, VestBack = 202,
            GloveLeft = 203, GloveRight = 204,
            Custom1 = 251, Custom2 = 252, Custom3 = 253, Custom4 = 254
        }

        public class RotationOption
        {
            public RotationOption(float offsetX, float offsetY)
            {
                OffsetX = offsetX;
                OffsetY = offsetY;
            }
            public float OffsetX, OffsetY;
            public override string ToString() => "RotationOption { OffsetX=" + OffsetX.ToString() +
                       ", OffsetY=" + OffsetY.ToString() + " }";
        }

        public class ScaleOption
        {
            public ScaleOption(float intensity = 1f, float duration = 1f)
            {
                Intensity = intensity;
                Duration = duration;
            }
            public float Intensity, Duration;
            public override string ToString() => "ScaleOption { Intensity=" + Intensity.ToString() +
                       ", Duration=" + Duration.ToString() + " }";
        }

        public class DotPoint
        {
            public DotPoint(int index, int intensity = 50)
            {
                if ((index < 0) || (index > MaxBufferSize))
                    throw new Exception("Invalid argument index : " + index);
                Intensity = MelonUtils.Clamp(intensity, 0, 100);
                Index = index;
            }
            public int Index, Intensity;
            public override string ToString() => "DotPoint { Index=" + Index.ToString() +
                       ", Intensity=" + Intensity.ToString() + " }";
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PathPoint
        {
            public PathPoint(float x, float y, int intensity = 50, int motorCount = 3)
            {
                X = MelonUtils.Clamp(x, 0f, 1f);
                Y = MelonUtils.Clamp(y, 0f, 1f);
                Intensity = MelonUtils.Clamp(intensity, 0, 100);
                MotorCount = MelonUtils.Clamp(motorCount, 0, 3);
            }
            public float X, Y;
            public int Intensity;

            // Number of maximum motors to vibrate
            // if 0 means default motor count, now 3
            public int MotorCount;

            public override string ToString() => "PathPoint { X=" + X.ToString() +
                       ", Y=" + Y.ToString() +
                       ", MotorCount=" + MotorCount.ToString() +
                       ", Intensity=" + Intensity.ToString() + " }";
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FeedbackStatus
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public int[] values;
        };

        private class NativeExports
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dInitialise(string appId, string appName);
            internal dInitialise Initialise;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dTurnOff();
            internal dTurnOff TurnOff;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dDestroy();
            internal dDestroy Destroy;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dRegisterFeedback(string str, string tactFileStr);
            internal dRegisterFeedback RegisterFeedback;
            internal dRegisterFeedback RegisterFeedbackFromTactFile;
            internal dRegisterFeedback RegisterFeedbackFromTactFileReflected;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitRegistered(string key);
            internal dSubmitRegistered SubmitRegistered;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitRegisteredStartMillis(string key, int startTimeMillis);
            internal dSubmitRegisteredStartMillis SubmitRegisteredStartMillis;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitRegisteredWithOption(string key, string altKey, float intensity, float duration, float offsetX, float offsetY);
            internal dSubmitRegisteredWithOption SubmitRegisteredWithOption;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitByteArray(string key, PositionType pos, byte[] bytes, int length, int durationMillis);
            internal dSubmitByteArray SubmitByteArray;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dSubmitPathArray(string key, PositionType pos, PathPoint[] points, int length, int durationMillis);
            internal dSubmitPathArray SubmitPathArray;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dIsFeedbackRegistered(string key);
            internal dIsFeedbackRegistered IsFeedbackRegistered;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dIsPlaying();
            internal dIsPlaying IsPlaying;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dIsPlayingKey(string key);
            internal dIsPlayingKey IsPlayingKey;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dTurnOffKey(string key);
            internal dTurnOffKey TurnOffKey;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dIsDevicePlaying(PositionType pos);
            internal dIsDevicePlaying IsDevicePlaying;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dTryGetResponseForPosition(PositionType pos, out FeedbackStatus status);
            internal dTryGetResponseForPosition TryGetResponseForPosition;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate bool dTryGetExePath(byte[] buf, ref int size);
            internal dTryGetExePath TryGetExePath;
        }
    }
}