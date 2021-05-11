using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if __ANDROID__
namespace MelonLoader
{
    internal static class bHaptics_NativeLibrary
    {
        public static Action OnChange = () => { };

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void RegisterProject([MarshalAs(UnmanagedType.LPStr)] string key,
            [MarshalAs(UnmanagedType.LPStr)] string contents);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void RegisterProjectReflected([MarshalAs(UnmanagedType.LPStr)] string key,
            [MarshalAs(UnmanagedType.LPStr)] string contents);

        // sizes is the size of each array
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void SubmitPath([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string position, float[] x, float[] y, int[] intensities, int[] sizes, int duration);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void SubmitDot([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string position, int[] indexes, int[] intensities, int[] sizes, int duration);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void SubmitRegistered([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string altKey, float intensity, float duration, float offsetAngleX, float offsetY);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void SubmitRegisteredWithTime([MarshalAs(UnmanagedType.LPStr)] string key, int startTime);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static byte[] GetPositionStatus(bHaptics.PositionType position);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void TurnOffAll();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void TurnOff([MarshalAs(UnmanagedType.LPStr)] string key);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsRegistered([MarshalAs(UnmanagedType.LPStr)] string key);
        
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsPlaying([MarshalAs(UnmanagedType.LPStr)] string key);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsAnythingPlaying();
    }
}
#endif