using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader
{
    internal unsafe static class BootstrapInterop
    {
#if NET6_0
        internal static delegate* unmanaged<void**, void*, void> HookAttach;
        internal static delegate* unmanaged<void**, void*, void> HookDetach;
#endif

        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string Internal_GetHashCode();

        internal static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameName, [MarshalAs(UnmanagedType.LPStr)] string GameVersion = null)
        {
            var lemon = MelonLaunchOptions.Console.Mode == MelonLaunchOptions.Console.DisplayMode.LEMON;
            var versionStr = $"{(lemon ? "Lemon" : "Melon")}Loader " +
                $"v{BuildInfo.Version} " +
                $"{(Core.Is_ALPHA_PreRelease ? "ALPHA Pre-Release" : "Open-Beta")}" +
                $" - {GameName} {(GameVersion is null ? "" : GameVersion)}";

            Console.Title = versionStr;
        }

#if !NET6_0
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void EnableCloseButton(IntPtr mainWindow);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void DisableCloseButton(IntPtr mainWindow);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsUnderWineOrSteamProton();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void NativeHookAttach(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void NativeHookDetach(IntPtr target, IntPtr detour);
#else

        private const int MF_BYCOMMAND = 0x00000000;

        private const int MF_ENABLED = 0x00000000;
        private const int MF_GRAYED = 0x00000001;
        private const int MF_DISABLED = 0x00000002;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        public static void EnableCloseButton(IntPtr mainWindow) 
        {
            EnableMenuItem(GetSystemMenu(mainWindow, false), SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
        }

        public static void DisableCloseButton(IntPtr mainWindow)
        {
            EnableMenuItem(GetSystemMenu(mainWindow, false), SC_CLOSE, (MF_BYCOMMAND | MF_DISABLED | MF_GRAYED));
        }

        public static bool IsUnderWineOrSteamProton()
        {
            MelonLogger.Warning("TODO: IsUnderWineOrSteamProton");
            return false;
        }

        public static unsafe void NativeHookAttach(IntPtr target, IntPtr detour) 
        {
            HookAttach((void**) target, (void*) detour);
        }

        public static unsafe void NativeHookDetach(IntPtr target, IntPtr detour)
        {
            HookDetach((void**)target, (void*)detour);
        }
#endif
    }
}
