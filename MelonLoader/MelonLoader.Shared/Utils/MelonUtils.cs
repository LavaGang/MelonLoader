using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using MelonLoader.Interfaces;
using MelonLoader.Properties;
using MonoMod.Utils;

#if !NET35
using System.Runtime.InteropServices;
#endif

namespace MelonLoader.Utils
{
    public static class MelonUtils
    {
        public static string HashCode { get; private set; }

        internal static void Setup(AppDomain domain)
        {
            using var sha = SHA256.Create();
            HashCode = string.Join("", sha.ComputeHash(File.ReadAllBytes(Assembly.GetExecutingAssembly().Location)).Select(b => b.ToString("X")).ToArray());
            
            WelcomeMessage();
            
            EngineModuleInfo engineModuleInfo = ModuleManager.EngineModule.GameInfo;
            
            MelonLogger.WriteLine(Color.Magenta);
            MelonLogger.Msg($"Game Name: {engineModuleInfo.GameName}");
            MelonLogger.Msg($"Game Developer: {engineModuleInfo.GameDeveloper}");
            MelonLogger.Msg($"Engine Name: {engineModuleInfo.EngineName}");
            MelonLogger.Msg($"Engine Version: {engineModuleInfo.EngineVersion}");
            MelonLogger.Msg($"Game Version: {engineModuleInfo.GameVersion}");
            MelonLogger.WriteLine(Color.Magenta);
            MelonLogger.WriteSpacer();
        }
        
        private static string GetVersionString()
        {
            StringBuilder sb = new();
            sb.Append("MelonLoader ");
            sb.Append($"v{BuildInfo.Version} ");
            sb.Append(Core.IsAlpha ? "ALPHA Pre-Release" : "Open-Beta");
            
            return sb.ToString();
        }

        private static void WelcomeMessage()
        {
            EngineModuleInfo engineModuleInfo = ModuleManager.EngineModule.GameInfo;
            
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect(GetVersionString());
            MelonLogger.MsgDirect($"OS: {OsUtils.GetOSVersion()}");
            MelonLogger.MsgDirect($"Hash Code: {MelonUtils.HashCode}");
            MelonLogger.MsgDirect("------------------------------");
            MelonLogger.MsgDirect($"Game Type: {engineModuleInfo.RuntimeName}");
            var archString = MelonUtils.IsGame32Bit ? "x86" : "x64";
            MelonLogger.MsgDirect($"Game Arch: {archString}");
            MelonLogger.MsgDirect("------------------------------");

            MelonEnvironment.PrintEnvironment();
        }
        
        public static Color DefaultTextColor 
            => Color.White;
        public static ConsoleColor DefaultTextConsoleColor 
            => ConsoleColor.White;

        public static string TimeStamp
            => $"{DateTime.Now:HH:mm:ss.fff}";

        public static PlatformID Platform 
            => Environment.OSVersion.Platform;

        public static bool IsUnix
            => !IsAndroid 
                && Platform is PlatformID.Unix;

        public static bool IsWindows
            => Platform is PlatformID.Win32NT 
                or PlatformID.Win32S
                or PlatformID.Win32Windows
                or PlatformID.WinCE;

        public static bool IsMac 
            => Platform is PlatformID.MacOSX;

        public static bool IsUnderWineOrSteamProton()
            => OsUtils.WineGetVersion is not null;
        
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> { if (value.CompareTo(min) < 0) return min; if (value.CompareTo(max) > 0) return max; return value; }

#if !NET35
        public static bool IsAndroid => RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));
        public static bool IsGame32Bit => !Environment.Is64BitProcess;
#else
        public static bool IsGame32Bit => IntPtr.Size == 4;
        public static bool IsAndroid => false;
#endif
    }
}