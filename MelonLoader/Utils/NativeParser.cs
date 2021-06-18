using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.Utils.bHapticsExtra;

namespace MelonLoader
{
    public class NativeParser
    {
        private struct ArrayData
        {
            public int Size;
            public IntPtr Location;
        }

        internal static byte[] ParseBytesArray(IntPtr cArr)
        {
            var data = GetArrayInfo(cArr);
                
            byte[] result = new byte[data.Size];
            Marshal.Copy(data.Location, result, 0, data.Size);
            
            Cleanup(cArr, data);

            return result;
        }
        
        internal static BhapticsDevice[] ParseDevicesArray(IntPtr cArr)
        {
            var data = GetArrayInfo(cArr);
            
            IntPtr[] cAddresses = new IntPtr[data.Size];
            Marshal.Copy(data.Location, cAddresses, 0, data.Size);
            
            Cleanup(cArr, data);

            BhapticsDevice[] result = new BhapticsDevice[data.Size];

            for (int i = 0; i < data.Size; i++)
            {
                result[i] = new BhapticsDevice(Marshal.PtrToStringAnsi(cAddresses[i]));
                SafeRelease(cAddresses[i]);
            }
            
            return result;
        }
        
        internal static DateTime ParseUnixTime( double unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dtDateTime;
        }

        private static ArrayData GetArrayInfo(IntPtr cArr)
        {
            Int64[] res = new Int64[2];
            Marshal.Copy(cArr, res, 0, 2);
            
            int size = (int) res[0];
            IntPtr arrayLoc = new IntPtr(res[1]);

            return new ArrayData
            {
                Size = size,
                Location = arrayLoc
            };
        }

        private static void Cleanup(IntPtr cArr)
        {
            Cleanup(cArr, GetArrayInfo(cArr));
        }
        
        private static void Cleanup(IntPtr cArr, ArrayData data)
        {
            SafeRelease(data.Location);
            SafeRelease(cArr);
        }

        private static void SafeRelease(IntPtr ptr)
        {
            if (ReleaseAddress(ptr) != 0)
                throw new Exception("Memory unsuccessfully released. Likely UB.");
        }
        
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static int ReleaseAddress(IntPtr ptr);
    }
}