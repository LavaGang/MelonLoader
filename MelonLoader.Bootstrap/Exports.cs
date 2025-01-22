﻿using MelonLoader.Bootstrap.Logging;
using MelonLoader.Bootstrap.RuntimeHandlers.Mono;
using MelonLoader.Bootstrap.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap;

internal static class Exports
{
#if WINDOWS
    [UnmanagedCallersOnly(EntryPoint = "DllMain")]
    [RequiresDynamicCode("Calls InitConfig")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "It's DllMain")]
    public static bool DllMain(nint hModule, uint ulReasonForCall, nint lpReserved)
    {
        if (ulReasonForCall != 1)
            return true;

        Proxy.ProxyResolver.Init(hModule);
        Core.Init(hModule);

        return true;
    }
#endif

    [UnmanagedCallersOnly(EntryPoint = "NativeHookAttach")]
    public static unsafe void NativeHookAttach(nint* target, nint detour)
    {
        *target = Dobby.HookAttach(*target, detour);
    }

    [UnmanagedCallersOnly(EntryPoint = "NativeHookDetach")]
    public static unsafe void NativeHookDetach(nint* target, nint detour)
    {
        Dobby.HookDetach(*target);
    }

    [UnmanagedCallersOnly(EntryPoint = "LogMsg")]
    public static unsafe void LogMsg(ColorRGB* msgColor, char* msg, int msgLength, ColorRGB* sectionColor, char* section, int sectionLength)
    {
        if (msgColor == null || msg == null)
        {
            MelonLogger.LogSpacer();
            return;
        }

        var mMsg = new ReadOnlySpan<char>(msg, msgLength);

        if (sectionColor == null || section == null)
        {
            MelonLogger.Log(*msgColor, mMsg);
            return;
        }

        MelonLogger.Log(*msgColor, mMsg, *sectionColor, new(section, sectionLength));
    }

    [UnmanagedCallersOnly(EntryPoint = "LogError")]
    public static unsafe void LogError(char* msg, int msgLength, char* section, int sectionLength, bool warning)
    {
        var mMsg = new ReadOnlySpan<char>(msg, msgLength);
        if (section == null)
        {
            if (warning)
                MelonLogger.LogWarning(mMsg);
            else
                MelonLogger.LogError(mMsg);

            return;
        }

        if (warning)
            MelonLogger.LogWarning(mMsg, new(section, sectionLength));
        else
            MelonLogger.LogError(mMsg, new(section, sectionLength));
    }

    [UnmanagedCallersOnly(EntryPoint = "LogMelonInfo")]
    public static unsafe void LogMelonInfo(ColorRGB* nameColor, char* name, int nameLength, char* info, int infoLength)
    {
        MelonLogger.LogMelonInfo(*nameColor, new(name, nameLength), new(info, infoLength));
    }

    [UnmanagedCallersOnly(EntryPoint = "MonoInstallHooks")]
    public static void MonoInstallHooks()
    {
        MonoHandler.InstallHooks();
    }

    [UnmanagedCallersOnly(EntryPoint = "MonoGetDomainPtr")]
    public static nint MonoGetDomainPtr()
    {
        return MonoHandler.Domain;
    }

    [UnmanagedCallersOnly(EntryPoint = "MonoGetRuntimeHandle")]
    public static nint MonoGetRuntimeHandle()
    {
        return MonoHandler.Mono.Handle;
    }

    [UnmanagedCallersOnly(EntryPoint = "IsConsoleOpen")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static bool IsConsoleOpen()
    {
        return ConsoleHandler.HasOwnWindow;
    }

    [UnmanagedCallersOnly(EntryPoint = "GetLoaderConfig")]
    public static unsafe void GetLoaderConfig(nint* pConfig)
    {
        Marshal.StructureToPtr(LoaderConfig.Current, *pConfig, false);
    }
}
