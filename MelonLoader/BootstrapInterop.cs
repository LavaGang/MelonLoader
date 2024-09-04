using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET6_0_OR_GREATER
using MelonLoader.CoreClrUtils;
#endif

namespace MelonLoader
{
    internal static unsafe class BootstrapInterop
    {
#if NET6_0_OR_GREATER
        internal static delegate* unmanaged<void**, void*, void> HookAttach;
        internal static delegate* unmanaged<void**, void*, void> HookDetach;
#endif

        internal static void SetDefaultConsoleTitleWithGameName([MarshalAs(UnmanagedType.LPStr)] string GameName, [MarshalAs(UnmanagedType.LPStr)] string GameVersion = null)
        {
            if (!MelonLaunchOptions.Console.ShouldSetTitle || MelonLaunchOptions.Console.ShouldHide)
                return;

            string versionStr = Core.GetVersionString() +
                                $" - {GameName} {GameVersion ?? ""}";

            // Setting the title might not work on .net 2.0. In WTTG 2 it's present in mscorlib, but the resolver can't find it for whatever reason.
            // Using reflection to avoid resolver errors
            HarmonyLib.AccessTools.Property(typeof(Console), "Title")?.SetValue(null, versionStr, null);
        }
        
        private const int MF_BYCOMMAND = 0x00000000;

        private const int MF_ENABLED = 0x00000000;
        private const int MF_GRAYED = 0x00000001;
        private const int MF_DISABLED = 0x00000002;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, byte bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        public static void EnableCloseButton(IntPtr mainWindow)
        {
            EnableMenuItem(GetSystemMenu(mainWindow, 0), SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
        }

        public static void DisableCloseButton(IntPtr mainWindow)
        {
            EnableMenuItem(GetSystemMenu(mainWindow, 0), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
        }

#if !NET6_0
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void NativeHookAttach(IntPtr target, IntPtr detour);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void NativeHookDetach(IntPtr target, IntPtr detour);
#else
        public static void NativeHookAttach(IntPtr target, IntPtr detour)
        {
            //SanityCheckDetour is able to wrap and fix the bad method in a delegate where possible, so we pass the detour by ref.
            if ( /*MelonDebug.IsEnabled() && */ !MelonUtils.IsUnderWineOrSteamProton() && !CoreClrDelegateFixer.SanityCheckDetour(ref detour))
                return;

            NativeHookAttachDirect(target, detour);
            NativeStackWalk.RegisterHookAddr((ulong)target, $"Mod-requested detour of 0x{target:X} -> 0x{detour:X}");
        }

        internal static unsafe void NativeHookAttachDirect(IntPtr target, IntPtr detour)
        {
            HookAttach((void**)target, (void*)detour);
        }

        public static unsafe void NativeHookDetach(IntPtr target, IntPtr detour)
        {
            HookDetach((void**)target, (void*)detour);
            NativeStackWalk.UnregisterHookAddr((ulong)target);
        }
#endif
    }
}