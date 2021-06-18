#if __ANDROID__
using System;
using System.Runtime.CompilerServices;
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
            (ConnectionStatus) Enum.Parse(typeof(ConnectionStatus), string.Copy(Internal_GetConnectionStatus(DeviceAddress)));
        public bHaptics.PositionType Position =>
            (bHaptics.PositionType) Enum.Parse(typeof(bHaptics.PositionType), String.Copy(Internal_GetPosition(DeviceAddress)));
        public string Address => string.Copy(Internal_GetAddress(DeviceAddress)); // why, because constancy
        public string DeviceName => string.Copy(Internal_GetDeviceName(DeviceAddress));
        public int Battery => Internal_GetBattery(DeviceAddress);
        public bHaptics.DeviceType Type =>
            (bHaptics.DeviceType) Enum.Parse(typeof(bHaptics.DeviceType), string.Copy(Internal_GetType(DeviceAddress)));

        public byte[] LastBytes => NativeParser.ParseBytesArray(Internal_GetLastBytes(DeviceAddress));

        public DateTime LastScannedTime => NativeParser.ParseUnixTime(Internal_GetLastScannedTime(DeviceAddress));

        public override string ToString()
        {
            return string.Copy(Internal_ToString(DeviceAddress));
        }

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static bool Internal_IsPing([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static bool Internal_IsPaired([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static int Internal_GetConnectFailCount([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static int Internal_GetRssi([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetConnectionStatus([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetPosition([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetAddress([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetDeviceName([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static int Internal_GetBattery([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_GetType([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static IntPtr Internal_GetLastBytes([MarshalAs(UnmanagedType.LPStr)] string address); // expects an array of 2 intptrs, 0 = len, 1 = ptr
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static long Internal_GetLastScannedTime([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private extern static string Internal_ToString([MarshalAs(UnmanagedType.LPStr)] string address);
    }
}
#endif