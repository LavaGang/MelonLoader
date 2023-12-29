using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
            
            Core.WelcomeMessage();
            
            MelonLogger.WriteLine(Color.Magenta);
            MelonLogger.Msg($"Game Name: {ModuleManager.EngineModule.GameName}");
            MelonLogger.Msg($"Game Developer: TODO");
            MelonLogger.Msg($"Engine Name: {ModuleManager.EngineModule.EngineName}");
            MelonLogger.Msg($"Engine Version: {ModuleManager.EngineModule.EngineVersion}");
            MelonLogger.Msg($"Game Version: TODO");
            MelonLogger.WriteLine(Color.Magenta);
            MelonLogger.WriteSpacer();
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