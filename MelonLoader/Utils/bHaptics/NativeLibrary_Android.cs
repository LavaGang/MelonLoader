using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnmanageUtility;

#if __ANDROID__
namespace MelonLoader
{
    internal static class bHaptics_NativeLibrary
    {
        public static Action OnChange = () => { };

        public static void SubmitPath(string key, string position, float[] x, float[] y, int[] intensities, int duration)
        {
            Internal_SubmitPath(key, position, x, y, intensities, new []{ x.Length, y.Length, intensities.Length }, duration);
        }

        public static void SubmitDot(string key, bHaptics.PositionType position, int[] indexes, int[] intensities, int duration)
        {
            // var data = new byte[]
            // {
            //     100, 0, 0, 0, 0,
            //     100, 0, 0, 0, 0,
            //     100, 0, 0, 0, 0,
            //     100, 0, 0, 0, 0,
            // }; 
            // HapticApi.SubmitByteArray(key, position, data, data.Length, 1000);
            // Start();
            // var sizes = new[] { indexes.Length, intensities.Length };
            // var sizes = new int[2] { indexes.Length, intensities.Length };
            // var handle = GCHandle.Alloc(sizes, GCHandleType.Pinned);

            // MelonDebug.Msg($"Sizes {sizes[0]} {sizes[1]}");
            
            // var handle = GCHandle.Alloc(objects, GCHandleType.Pinned);
            var nativeIndexes = indexes.ToUnmanagedArray();
            var nativeIntensities = intensities.ToUnmanagedArray();

            Internal_SubmitDot(key, position.ToString(), nativeIndexes.Ptr, nativeIndexes.Length, nativeIntensities.Ptr, nativeIntensities.Length, 1);
            
            nativeIndexes.Dispose();
            nativeIntensities.Dispose();
            // handle.Free();
        }

        public static byte[] GetPositionStatus(bHaptics.PositionType position)
        {
            return NativeParser.ParseBytesArray(Internal_GetPositionStatus(position.ToString()));
        }
        
        private static void Invoke_OnChange()
        {
            OnChange.Invoke();
        }

        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void Start();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void Stop();
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void TurnOff([MarshalAs(UnmanagedType.LPStr)] string key);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void TurnOffAll();
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void RegisterProject([MarshalAs(UnmanagedType.LPStr)] string key,
            [MarshalAs(UnmanagedType.LPStr)] string contents);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void RegisterProjectReflected([MarshalAs(UnmanagedType.LPStr)] string key,
            [MarshalAs(UnmanagedType.LPStr)] string contents);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void SubmitRegistered([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string altKey, float intensity, float duration, float offsetAngleX, float offsetY);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void SubmitRegisteredWithTime([MarshalAs(UnmanagedType.LPStr)] string key, int startTime);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static bool IsRegistered([MarshalAs(UnmanagedType.LPStr)] string key);
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static bool IsPlaying([MarshalAs(UnmanagedType.LPStr)] string key);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static bool IsAnythingPlaying();
        
        // sizes is the size of each array
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static void Internal_SubmitDot([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string position, IntPtr indexes, int index_size, IntPtr intensities, int intensity_size, int duration);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static void Internal_SubmitPath([MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string position, float[] x, float[] y, int[] intensities, int[] sizes, int duration);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static IntPtr Internal_GetPositionStatus(string position);
    }
}
#endif