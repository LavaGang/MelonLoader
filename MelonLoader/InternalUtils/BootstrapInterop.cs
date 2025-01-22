﻿using System;
using System.Runtime.InteropServices;

#if NET6_0_OR_GREATER
using MelonLoader.CoreClrUtils;
using MelonLoader.InternalUtils;
#endif

namespace MelonLoader.InternalUtils;

internal static unsafe class BootstrapInterop
{
    internal static BootstrapLibrary Library { get; private set; }

    internal static void SetDefaultConsoleTitleWithGameName(string gameName, string gameVersion = null)
    {
        var versionStr = $"{Core.GetVersionString()} - {gameName} {gameVersion ?? ""}";

        MelonUtils.SetConsoleTitle(versionStr);
    }

#if WINDOWS
    private const int MF_BYCOMMAND = 0x00000000;

    private const int MF_ENABLED = 0x00000000;
    private const int MF_GRAYED = 0x00000001;
    private const int MF_DISABLED = 0x00000002;
    public const int SC_CLOSE = 0xF060;

    [DllImport("user32.dll")]
    private static extern int EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, byte bRevert);

    public static void EnableCloseButton(IntPtr mainWindow)
    {
        _ = EnableMenuItem(GetSystemMenu(mainWindow, 0), SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
    }

    public static void DisableCloseButton(IntPtr mainWindow)
    {
        _ = EnableMenuItem(GetSystemMenu(mainWindow, 0), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
    }
#endif

    public static void NativeHookAttach(nint target, nint detour)
    {
#if NET6_0_OR_GREATER
        //SanityCheckDetour is able to wrap and fix the bad method in a delegate where possible, so we pass the detour by ref.
        if (!MelonUtils.IsUnderWineOrSteamProton() && !CoreClrDelegateFixer.SanityCheckDetour(ref detour))
            return;
#endif

        NativeHookAttachDirect(target, detour);

#if NET6_0_OR_GREATER
        NativeStackWalk.RegisterHookAddr((ulong)target, $"Mod-requested detour of 0x{target:X} -> 0x{detour:X}");
#endif
    }

    internal static unsafe void NativeHookAttachDirect(nint target, nint detour)
    {
        Library.NativeHookAttach((nint*)target, detour);
    }

    public static unsafe void NativeHookDetach(nint target, nint detour)
    {
        Library.NativeHookDetach((nint*)target, detour);

#if NET6_0_OR_GREATER
        NativeStackWalk.UnregisterHookAddr((ulong)target);
#endif
    }

    internal static void Initialize(nint bootstrapHandle)
    {
        Library = new NativeLibrary<BootstrapLibrary>(bootstrapHandle).Instance;

        try
        {
            Core.Initialize();
        }
        catch (Exception ex)
        {
            MelonLogger.Error("Failed to initialize MelonLoader");
            MelonLogger.Error(ex);

            throw new("Error at init");
        }
    }

    internal static void Start()
    {
        try
        {
            Core.Start();
        }
        catch (Exception ex)
        {
            MelonLogger.Error("Failed to start MelonLoader");
            MelonLogger.Error(ex);

            throw new("Error at start");
        }
    }
}