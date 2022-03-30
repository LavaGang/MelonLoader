using MelonLoader.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET6_0
using Microsoft.Diagnostics.Runtime;
#endif

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
            return Core.WineGetVersion is not null;
        }

        public static unsafe void NativeHookAttach(IntPtr target, IntPtr detour) 
        {
            if (MelonDebug.IsEnabled() && !SanityCheckDetour(detour))
                return;

            HookAttach((void**) target, (void*) detour);
        }

        private static bool SanityCheckDetour(IntPtr detour)
        {
            using DataTarget dt = DataTarget.CreateSnapshotAndAttach(Environment.ProcessId);
            ClrRuntime runtime = dt.ClrVersions.First().CreateRuntime();

            ClrMethod method = runtime.GetMethodByInstructionPointer((ulong)detour.ToInt64());

            if (method != null)
            {
                var managedMethod = MethodBaseHelper.GetMethodBaseFromHandle((IntPtr)method.MethodDesc);

                if (managedMethod?.GetCustomAttribute<UnmanagedCallersOnlyAttribute>() == null)
                {
                    //We have provided a direct managed method as the pointer to detour to. This doesn't work under CoreCLR, so we yell at the user and stop
                    var melon = MelonUtils.GetMelonFromStackTrace();

                    PrintDirtyDelegateWarning(melon?.LoggerInstance ?? new MelonLogger.Instance("Bad Delegate"), melon?.Info.Name ?? "Unknown mod", managedMethod);
                    return false;
                }
                else
                {
                    //MelonDebug.Msg($"{managedMethod.Name} is fine, has unmanagedcallersonly");
                }
            }
            else
            {
                //MelonDebug.Msg($"0x{detour.ToInt64():X} is fine, doesn't appear to be a managed method");
            }

            return true;
        }

        public static unsafe void NativeHookDetach(IntPtr target, IntPtr detour)
        {
            HookDetach((void**)target, (void*)detour);
        }

        private static void PrintDirtyDelegateWarning(MelonLogger.Instance offendingMelonLogger, string offendingMelonName, MethodBase offendingMethod)
        {
            var logger = offendingMelonLogger;

            logger.BigError(
                   $"The mod {offendingMelonName} has attempted to detour a native method to a managed one.\n"
                 + $"The managed method target is {offendingMethod.DeclaringType.Name}::{offendingMethod.Name}\n"
                 +  "If this hadn't been stopped, the runtime would have crashed.\n"
                 +  "Modder: Either create an [UnmanagedFunctionPointer] delegate from your function, and use Marshal.GetFunctionPointerFromDelegate,\n"
                 +  "or annotate your patch function as [UnmanagedCallersOnly] (target net5.0), and then you can directly use &Method as the hook target."
            );
        }
#endif
    }
}
