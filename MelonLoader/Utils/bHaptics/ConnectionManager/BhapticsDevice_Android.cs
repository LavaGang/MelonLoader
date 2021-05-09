#if __ANDROID__
using System;
using MelonLoader.bHapticsExtra;

namespace MelonLoader.Utils.bHapticsExtra
{
    public class BhapticsDevice : BaseBhapticsDevice
    {
        public bool IsPing { get; }
        public bool IsPaired { get; }
        public int ConnectFailCount { get; }
        public int Rssi { get; }
        public ConnectionStatus ConnectionStatus { get; }
        public MelonLoader.bHaptics.PositionType Position { get; }
        public string Address { get; }
        public string DeviceName { get; }
        public int Battery { get; }
        public MelonLoader.bHaptics.DeviceType Type { get; }
        public byte[] LastBytes { get; }
        public DateTime LastScannedTime { get; }

        public string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
#endif