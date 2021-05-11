#if __ANDROID__
using System;
using System.Runtime.InteropServices;
using MelonLoader.bHapticsExtra;

namespace MelonLoader.Utils.bHapticsExtra
{
    public class BhapticsDevice : BaseBhapticsDevice
    {
        private string DeviceAddress { get; }

        public BhapticsDevice(string Address)
        {
            DeviceAddress = Address;
        }

        public bool IsPing => Internal_IsPing(DeviceAddress);
        public bool IsPaired => Internal_IsPaired(DeviceAddress);
        public int ConnectFailCount => Internal_GetConnectFailCount(DeviceAddress);
        public int Rssi => Internal_GetRssi(DeviceAddress);
        public ConnectionStatus ConnectionStatus =>
            (ConnectionStatus) Enum.Parse(typeof(ConnectionStatus), Internal_GetConnectionStatus(DeviceAddress));
        public MelonLoader.bHaptics.PositionType Position =>
            (bHaptics.PositionType) Enum.Parse(typeof(bHaptics.PositionType), Internal_GetPosition(DeviceAddress));
        public string Address => Internal_GetAddress(DeviceAddress); // why, because constancy
        public string DeviceName => Internal_GetDeviceName(DeviceAddress);
        public int Battery => Internal_GetBattery(DeviceAddress);
        public MelonLoader.bHaptics.DeviceType Type =>
            (bHaptics.DeviceType) Enum.Parse(typeof(bHaptics.DeviceType), Internal_GetType(DeviceAddress));

        public byte[] LastBytes
        {
            get
            {
                var resPtr = Internal_GetLastBytes(DeviceAddress);
                Int64[] res = new Int64[2];
                Marshal.Copy(resPtr, res, 0, 2);
                
                int size = (int) res[0];
                IntPtr arrayLoc = new IntPtr(res[1]);
                
                byte[] result = new byte[size];
                Marshal.Copy(arrayLoc, result, 0, size);
                MelonUtils.ReleaseAddress(arrayLoc);
                MelonUtils.ReleaseAddress(resPtr);

                return result;
            }
        }

        public DateTime LastScannedTime => UnixTimeStampToDateTime(Internal_GetLastScannedTime(DeviceAddress));

        public string ToString()
        {
            return Internal_ToString(DeviceAddress);
        }

        private extern static bool Internal_IsPing(string address);
        private extern static bool Internal_IsPaired(string address);
        private extern static int Internal_GetConnectFailCount(string address);
        private extern static int Internal_GetRssi(string address);
        private extern static string Internal_GetConnectionStatus(string address);
        private extern static string Internal_GetPosition(string address);
        private extern static string Internal_GetAddress(string address);
        private extern static string Internal_GetDeviceName(string address);
        private extern static int Internal_GetBattery(string address);
        private extern static string Internal_GetType(string address);
        private extern static IntPtr Internal_GetLastBytes(string address); // expects an array of 2 intptrs, 0 = len, 1 = ptr 
        private extern static long Internal_GetLastScannedTime(string address);
        private extern static string Internal_ToString(string address);
        
        private static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dtDateTime;
        }
    }
}
#endif