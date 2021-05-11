#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MelonLoader.bHapticsExtra;
using MelonLoader.Utils.bHapticsExtra;

namespace MelonLoader.ConnectionManager
{
    public class ConnectionManager : BaseConnectionManager
    {
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

        public static BhapticsDevice[] GetDeviceList()
        {
            var resPtr = Internal_GetDeviceList();
            Int64[] res = new Int64[2];
            Marshal.Copy(resPtr, res, 0, 2);
            
            int size = (int) res[0];
            IntPtr arrayLoc = new IntPtr(res[1]);
            
            IntPtr[] cAddresses = new IntPtr[size];
            Marshal.Copy(arrayLoc, cAddresses, 0, size);
            MelonUtils.ReleaseAddress(arrayLoc);
            MelonUtils.ReleaseAddress(resPtr);

            BhapticsDevice[] result = new BhapticsDevice[size];

            for (int i = 0; i < size; i++)
            {
                result[i] = new BhapticsDevice(Marshal.PtrToStringAnsi(cAddresses[i]));
                MelonUtils.ReleaseAddress(cAddresses[i]);
            }
            
            return result;
        }

        public extern static void Scan();
        public extern static void StopScan();
        public extern static void RefreshPairingInfo();
        public extern static void Unpair(string address);
        public extern static void UnpairAll();
        public extern static void TogglePosition(string address);
        public extern static void Ping(string address);
        public extern static void PingAll();
        public extern static bool IsDeviceConnected();

        private extern static void Internal_Pair(string address);
        private extern static void Internal_PairPositioned(string address, string position);
        private extern static bool Internal_GetIsScanning();
        private extern static void Internal_ChangePosition(string address, string position);
        private extern static void Internal_SetMotor(string address, byte[] bytes, int size);
        private extern static IntPtr Internal_GetDeviceList(); // expects an array of 2 intptrs, 0 = len, 1 = ptr
    }
}
#endif