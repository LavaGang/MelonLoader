using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public static class bHaptics
    {
        public static bool WasError { get => false; }

        public class FeedbackStatus { public int[] values; };

        public class RotationOption : bHapticsLib.RotationOption { }
        public class ScaleOption : bHapticsLib.ScaleOption { }

        public class DotPoint : bHapticsLib.DotPoint { }
        private static Converter<DotPoint, bHapticsLib.DotPoint> DotPointConverter = new Converter<DotPoint, bHapticsLib.DotPoint>((x) => x);

        public class PathPoint : bHapticsLib.PathPoint { }
        private static Converter<PathPoint, bHapticsLib.PathPoint> PathPointConverter = new Converter<PathPoint, bHapticsLib.PathPoint>((x) => x);

        public static bool IsPlaying()
            => bHapticsLib.bHapticsManager.IsPlayingAny();
        public static bool IsPlaying(string key)
            => bHapticsLib.bHapticsManager.IsPlaying(key);

        public static bool IsDeviceConnected(PositionType type)
            => bHapticsLib.bHapticsManager.IsDeviceConnected((bHapticsLib.PositionID)(int)type);
        public static bool IsDeviceConnected(DeviceType type, bool isLeft = true)
            => IsDeviceConnected(DeviceTypeToPositionType(type, isLeft));

        public static bool IsFeedbackRegistered(string key)
            => bHapticsLib.bHapticsManager.IsPatternRegistered(key);

        public static void RegisterFeedback(string key, string tactFileStr)
            => bHapticsLib.bHapticsManager.RegisterPatternFromFile(key, tactFileStr);
        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr)
            => bHapticsLib.bHapticsManager.RegisterPatternFromJson(key, tactFileStr);
        public static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr)
            => bHapticsLib.bHapticsManager.RegisterPatternSwappedFromJson(key, tactFileStr);

        public static void SubmitRegistered(string key)
            => bHapticsLib.bHapticsManager.PlayRegistered(key);

        //public static void SubmitRegistered(string key, int startTimeMillis)
        //    => bHapticsLib.bHapticsManager.PlayRegistered(key, );

        public static void SubmitRegistered(string key, string altKey, ScaleOption option)
            => bHapticsLib.bHapticsManager.PlayRegistered(key, altKey, option);

        public static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption)
            => bHapticsLib.bHapticsManager.PlayRegistered(key, altKey, sOption, rOption);

        public static void TurnOff()
            => bHapticsLib.bHapticsManager.StopPlayingAll();
        public static void TurnOff(string key)
            => bHapticsLib.bHapticsManager.StopPlaying(key);

        public static void Submit(string key, DeviceType type, bool isLeft, byte[] bytes, int durationMillis)
            => Submit(key, DeviceTypeToPositionType(type, isLeft), bytes, durationMillis);
        public static void Submit(string key, PositionType position, byte[] bytes, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, (bHapticsLib.PositionID)(int)position, bytes);
        public static void Submit(string key, DeviceType type, bool isLeft, List<DotPoint> points, int durationMillis)
            => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        public static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, (bHapticsLib.PositionID)(int)position, points.ConvertAll(DotPointConverter));
        public static void Submit(string key, DeviceType type, bool isLeft, List<PathPoint> points, int durationMillis) 
            => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        public static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, (bHapticsLib.PositionID)(int)position, points.ConvertAll(PathPointConverter));

        public static FeedbackStatus GetCurrentFeedbackStatus(DeviceType type, bool isLeft = true)
            => GetCurrentFeedbackStatus(DeviceTypeToPositionType(type, isLeft));
        public static FeedbackStatus GetCurrentFeedbackStatus(PositionType pos)
            => new FeedbackStatus { values = bHapticsLib.bHapticsManager.GetDeviceStatus((bHapticsLib.PositionID)(int)pos) };

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

            Left = 1, 
            Right = 2,

            Vest = 3,

            Head = 4,

            Racket = 5,

            HandL = 6,
            HandR = 7,

            FootL = 8, 
            FootR = 9,

            ForearmL = 10,
            ForearmR = 11,

            VestFront = 201,
            VestBack = 202,

            GloveLeft = 203,
            GloveRight = 204,

            Custom1 = 251,
            Custom2 = 252,
            Custom3 = 253,
            Custom4 = 254
        }
    }
}