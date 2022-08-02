using System;
using System.Collections.Generic;

namespace MelonLoader
{
    public static class bHaptics
    {
        [Obsolete("MelonLoader.bHaptics.WasError is Only Here for Compatibility Reasons.")]
        public static bool WasError { get => false; }

        [Obsolete("MelonLoader.bHaptics.PositionType is Only Here for Compatibility Reasons. Please use bHapticsLib.PositionID instead.")]
        public enum PositionType
        {
            All = bHapticsLib.PositionID.All,
            Left = bHapticsLib.PositionID.Left,
            Right = bHapticsLib.PositionID.Right,
            Vest = bHapticsLib.PositionID.Vest,
            Head = bHapticsLib.PositionID.Head,
            Racket = bHapticsLib.PositionID.Racket,
            HandL = bHapticsLib.PositionID.HandLeft,
            HandR = bHapticsLib.PositionID.HandRight,
            FootL = bHapticsLib.PositionID.FootLeft,
            FootR = bHapticsLib.PositionID.FootRight,
            ForearmL = bHapticsLib.PositionID.ArmLeft,
            ForearmR = bHapticsLib.PositionID.ArmRight,
            VestFront = bHapticsLib.PositionID.VestFront,
            VestBack = bHapticsLib.PositionID.VestBack,
            GloveLeft = bHapticsLib.PositionID.GloveLeft,
            GloveRight = bHapticsLib.PositionID.GloveRight,
            Custom1 = bHapticsLib.PositionID.Custom1,
            Custom2 = bHapticsLib.PositionID.Custom2,
            Custom3 = bHapticsLib.PositionID.Custom3,
            Custom4 = bHapticsLib.PositionID.Custom4
        }

        [Obsolete("MelonLoader.bHaptics.RotationOption is Only Here for Compatibility Reasons. Please use bHapticsLib.RotationOption instead.")]
        public class RotationOption : bHapticsLib.RotationOption { }
        [Obsolete("MelonLoader.bHaptics.ScaleOption is Only Here for Compatibility Reasons. Please use bHapticsLib.ScaleOption instead.")]
        public class ScaleOption : bHapticsLib.ScaleOption { }

        [Obsolete("MelonLoader.bHaptics.DotPoint is Only Here for Compatibility Reasons. Please use bHapticsLib.DotPoint instead.")]
        public class DotPoint : bHapticsLib.DotPoint { }
        [Obsolete]
        private static Converter<DotPoint, bHapticsLib.DotPoint> DotPointConverter = new Converter<DotPoint, bHapticsLib.DotPoint>((x) => x);

        [Obsolete("MelonLoader.bHaptics.PathPoint is Only Here for Compatibility Reasons. Please use bHapticsLib.PathPoint instead.")]
        public class PathPoint : bHapticsLib.PathPoint { }
        [Obsolete]
        private static Converter<PathPoint, bHapticsLib.PathPoint> PathPointConverter = new Converter<PathPoint, bHapticsLib.PathPoint>((x) => x);

        [Obsolete("MelonLoader.bHaptics.IsPlaying is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsPlayingAny instead.")]
        public static bool IsPlaying()
            => bHapticsLib.bHapticsManager.IsPlayingAny();
        [Obsolete("MelonLoader.bHaptics.IsPlaying(string) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsPlaying instead.")]
        public static bool IsPlaying(string key)
            => bHapticsLib.bHapticsManager.IsPlaying(key);

        [Obsolete("MelonLoader.bHaptics.IsDeviceConnected is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsDeviceConnected instead.")]
        public static bool IsDeviceConnected(PositionType type)
            => bHapticsLib.bHapticsManager.IsDeviceConnected((bHapticsLib.PositionID)(int)type);

        [Obsolete("MelonLoader.bHaptics.IsFeedbackRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsPatternRegistered instead.")]
        public static bool IsFeedbackRegistered(string key)
            => bHapticsLib.bHapticsManager.IsPatternRegistered(key);

        [Obsolete("MelonLoader.bHaptics.RegisterFeedback is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.RegisterPatternFromFile instead.")]
        public static void RegisterFeedback(string key, string tactFileStr)
            => bHapticsLib.bHapticsManager.RegisterPatternFromFile(key, tactFileStr);
        [Obsolete("MelonLoader.bHaptics.RegisterFeedbackFromTactFile is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.RegisterPatternFromJson instead.")]
        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr)
            => bHapticsLib.bHapticsManager.RegisterPatternFromJson(key, tactFileStr);
        [Obsolete("MelonLoader.bHaptics.RegisterFeedbackFromTactFileReflected is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.RegisterPatternSwappedFromJson instead.")]
        public static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr)
            => bHapticsLib.bHapticsManager.RegisterPatternSwappedFromJson(key, tactFileStr);

        [Obsolete("MelonLoader.bHaptics.SubmitRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.PlayRegistered instead.")]
        public static void SubmitRegistered(string key)
            => bHapticsLib.bHapticsManager.PlayRegistered(key);
        [Obsolete("MelonLoader.bHaptics.SubmitRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.PlayRegistered instead.")]
        public static void SubmitRegistered(string key, int startTimeMillis)
            => bHapticsLib.bHapticsManager.PlayRegistered(key, startTimeMillis);
        [Obsolete("MelonLoader.bHaptics.SubmitRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.PlayRegistered instead.")]
        public static void SubmitRegistered(string key, string altKey, ScaleOption option)
            => bHapticsLib.bHapticsManager.PlayRegistered(key, altKey, option);
        [Obsolete("MelonLoader.bHaptics.SubmitRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.PlayRegistered instead.")]
        public static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption)
            => bHapticsLib.bHapticsManager.PlayRegistered(key, altKey, sOption, rOption);

        [Obsolete("MelonLoader.bHaptics.TurnOff is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.StopPlayingAll instead.")]
        public static void TurnOff()
            => bHapticsLib.bHapticsManager.StopPlayingAll();
        [Obsolete("MelonLoader.bHaptics.TurnOff is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.StopPlaying instead.")]
        public static void TurnOff(string key)
            => bHapticsLib.bHapticsManager.StopPlaying(key);

        [Obsolete("MelonLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, PositionType position, byte[] bytes, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, (bHapticsLib.PositionID)(int)position, bytes);
        [Obsolete("MelonLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, (bHapticsLib.PositionID)(int)position, points.ConvertAll(DotPointConverter));
        [Obsolete("MelonLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, (bHapticsLib.PositionID)(int)position, points.ConvertAll(PathPointConverter));

        [Obsolete("MelonLoader.bHaptics.GetCurrentFeedbackStatus is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.GetDeviceStatus instead.")]
        public static FeedbackStatus GetCurrentFeedbackStatus(PositionType pos)
            => new FeedbackStatus { values = bHapticsLib.bHapticsManager.GetDeviceStatus((bHapticsLib.PositionID)(int)pos) };
        [Obsolete("MelonLoader.bHaptics.Submit(DeviceType) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, DeviceType type, bool isLeft, byte[] bytes, int durationMillis)
            => Submit(key, DeviceTypeToPositionType(type, isLeft), bytes, durationMillis);
        [Obsolete("MelonLoader.bHaptics.Submit(DeviceType) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, DeviceType type, bool isLeft, List<DotPoint> points, int durationMillis)
            => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        [Obsolete("MelonLoader.bHaptics.Submit(DeviceType) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, DeviceType type, bool isLeft, List<PathPoint> points, int durationMillis)
            => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        [Obsolete("MelonLoader.bHaptics.GetCurrentFeedbackStatus(DeviceType) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.GetDeviceStatus instead.")]
        public static FeedbackStatus GetCurrentFeedbackStatus(DeviceType type, bool isLeft = true)
            => GetCurrentFeedbackStatus(DeviceTypeToPositionType(type, isLeft));
        [Obsolete("MelonLoader.bHaptics.IsDeviceConnected(DeviceType) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsDeviceConnected instead.")]
        public static bool IsDeviceConnected(DeviceType type, bool isLeft = true)
            => IsDeviceConnected(DeviceTypeToPositionType(type, isLeft));



        [Obsolete("MelonLoader.bHaptics.FeedbackStatus is Only Here for Compatibility Reasons.")]
        public class FeedbackStatus { public int[] values; };
        [Obsolete("MelonLoader.bHaptics.DeviceTypeToPositionType(DeviceType) is Only Here for Compatibility Reasons.")]
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
        [Obsolete("MelonLoader.bHaptics.DeviceType is Only Here for Compatibility Reasons.")]
        public enum DeviceType
        {
            None = 0,
            Tactal = 1,
            TactSuit = 2,
            Tactosy_arms = 3,
            Tactosy_hands = 4,
            Tactosy_feet = 5
        }
    }
}