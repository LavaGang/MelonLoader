using System;
using System.Collections.Generic;

using DeviceAddress = System.String;

namespace MelonLoader.bHapticsExtra
{
    public enum ConnectionStatus {
        Connecting, 
        Disconnecting, 
        Connected, 
        Disconnected
    }
    
    public class BaseConnectionManager
    {
        public static bool IsConnectionManagerSupported => false;

        public static bool IsScanning => false;

        public static void StartScan()
        {
            throw new NotImplementedException();
        }
        
        public static void StopScan()
        {
            throw new NotImplementedException();
        }
        
        public static void RefreshPairingInfo()
        {
            throw new NotImplementedException();

        }
        
        public static void Pair(DeviceAddress address)
        {
            throw new NotImplementedException();
        }

        public static void Pair(DeviceAddress address, bHaptics.PositionType position)
        {
            throw new NotImplementedException();
        }

        public static void Unpair(DeviceAddress address)
        {
            throw new NotImplementedException();
        }

        public static void UnpairAll()
        {
            throw new NotImplementedException();
        }

        public static void ChangePosition(DeviceAddress address, bHaptics.PositionType newPosition)
        {
            throw new NotImplementedException();
        }

        public static void TogglePosition(DeviceAddress address)
        {
            throw new NotImplementedException();
        }

        public static void SetMotor(DeviceAddress address, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public static void Ping(DeviceAddress address)
        {
            throw new NotImplementedException();
        }

        public static void PingAll()
        {
            throw new NotImplementedException();
        }

        public static bool IsDeviceConnected(bHaptics.DeviceType type)
        {
            throw new NotImplementedException();
        }

        public static List<BaseBhapticsDevice> GetDeviceList()
        {
            throw new NotImplementedException();
        }

        public static Action<List<BaseBhapticsDevice>> OnDeviceUpdate { get; set; }
        public static Action<bool> OnScanStatusChange { get; set; }
        public static Action OnChangeResponse { get; set; }
        public static Action<DeviceAddress> OnConnect { get; set; }
        public static Action<DeviceAddress> OnDisconnect { get; set; }
    }

    public interface BaseBhapticsDevice
    {
        public bool IsPing { get; }
        public bool IsPaired { get; }
        public int ConnectFailCount { get; }
        public int Rssi { get; }
        public ConnectionStatus ConnectionStatus { get; }
        public bHaptics.PositionType Position { get; }
        public DeviceAddress Address { get; }
        public string DeviceName { get; }
        public int Battery { get; }
        public bHaptics.DeviceType Type { get; }
        public byte[] LastBytes { get; }
        public DateTime LastScannedTime { get; }
        public string ToString();
    }
}