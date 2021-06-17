#if !__ANDROID__
using System;
using MelonLoader.bHapticsExtra;

namespace MelonLoader.Utils.bHapticsExtra
{
    public class BhapticsDevice : BaseBhapticsDevice
    {
        public bool IsPing => false;
        public bool IsPaired => false;
        public int ConnectFailCount => 0;
        public int Rssi => 0;
        public ConnectionStatus ConnectionStatus => ConnectionStatus.Disconnected;
        public MelonLoader.bHaptics.PositionType Position => MelonLoader.bHaptics.PositionType.All;
        public string Address => null;
        public string DeviceName => null;
        public int Battery => 0;
        public MelonLoader.bHaptics.DeviceType Type => bHaptics.DeviceType.None;
        public byte[] LastBytes => null;
        public DateTime LastScannedTime => new DateTime(0);

        public string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
#endif