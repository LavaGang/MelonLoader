#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.bHapticsExtra;
using MelonLoader.Utils.bHapticsExtra;

namespace MelonLoader.ConnectionManager
{
    public class ConnectionManager : BaseConnectionManager
    {
        public static Action<BhapticsDevice[]> OnDeviceUpdate = (devices) => { };
        public static Action<bool> OnScanStatusChange = (isScanning) => { };
        public static Action OnChangeResponse = () => { };
        public static Action<string> OnConnect = (address) => { };
        public static Action<string> OnDisconnect = (address) => { };
        
        public new static bool IsConnectionManagerSupported => true;
        public new static bool IsScanning => Internal_GetIsScanning();

        public static void Pair(string address)
        {
            Internal_Pair(address);
        }
        public static void Pair(string address, bHaptics.PositionType position)
        {
            Internal_PairPositioned(address, position.ToString());
        }
        public static void ChangePosition(string address, bHaptics.PositionType position)
        {
            Internal_ChangePosition(address, position.ToString());
        }
        public static void SetMotor(string address, byte[] bytes)
        {
            Internal_SetMotor(address, bytes, bytes.Length);
        }
        
        public static BhapticsDevice[] GetDeviceList() => NativeParser.ParseDevicesArray(Internal_GetDeviceList());

        private static void Invoke_OnDeviceUpdate(IntPtr cDeviceArr)
        {
            OnDeviceUpdate.Invoke(NativeParser.ParseDevicesArray(cDeviceArr));
        }
        private static void Invoke_OnScanStatusChange(bool isScanning)
        {
            OnScanStatusChange.Invoke(isScanning);
        }
        private static void Invoke_OnChangeResponse()
        {
            OnChangeResponse.Invoke();
        }
        private static void Invoke_OnConnect([MarshalAs(UnmanagedType.LPStr)] string address)
        {
            OnConnect.Invoke(string.Copy(address));
        }
        private static void Invoke_OnDisconnect([MarshalAs(UnmanagedType.LPStr)] string address)
        {
            OnDisconnect.Invoke(string.Copy(address));
        }
        
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void Scan();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void StopScan();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void RefreshPairingInfo();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void Unpair([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void UnpairAll();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void TogglePosition([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void Ping([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static void PingAll();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        public extern static bool IsDeviceConnected([MarshalAs(UnmanagedType.LPStr)] string address);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static void Internal_Pair([MarshalAs(UnmanagedType.LPStr)] string address);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static void Internal_PairPositioned([MarshalAs(UnmanagedType.LPStr)] string address, [MarshalAs(UnmanagedType.LPStr)] string position);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static bool Internal_GetIsScanning();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static void Internal_ChangePosition([MarshalAs(UnmanagedType.LPStr)] string address, [MarshalAs(UnmanagedType.LPStr)] string position);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static void Internal_SetMotor([MarshalAs(UnmanagedType.LPStr)] string address, byte[] bytes, int size);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Native)]
        private extern static IntPtr Internal_GetDeviceList(); // expects an array of 2 intptrs, 0 = len, 1 = ptr
    }
}
#endif