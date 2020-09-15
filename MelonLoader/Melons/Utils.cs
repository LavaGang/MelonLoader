using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

namespace MelonLoader
{
    public static class MelonUtils
    {
        public static string RandomString(int length)
        {
            StringBuilder builder = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < length; i++)
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(25 * rand.NextDouble())) + 65));
            return builder.ToString();
        }
        public static string GetUserDataDirectory() => Core.UserDataPath;
        public static bool IsVRChat() => (GetGameName().Equals("VRChat") && GetGameDeveloper().Equals("VRChat"));
        public static bool IsBoneworks() => (GetGameName().Equals("BONEWORKS") && GetGameDeveloper().Equals("Stress Level Zero"));

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsGameIl2Cpp();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsOldMono();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetApplicationPath();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetGameName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetGameDeveloper();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetGameDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetGameDataDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetUnityVersion();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public extern static string GetManagedDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void SetConsoleTitle([MarshalAs(UnmanagedType.LPStr)] string title);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void NativeHookAttach(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void NativeHookDetach(IntPtr target, IntPtr detour);
    }
}