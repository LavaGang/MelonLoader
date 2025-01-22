﻿#if !NET6_0_OR_GREATER

using MelonLoader.InternalUtils;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader.Utils;

public class MonoLibrary
{
    internal static bool Setup()
    {
        IntPtr NativeMonoPtr = BootstrapInterop.Library.MonoGetRuntimeHandle();
        if (NativeMonoPtr == IntPtr.Zero)
        {
            MelonLogger.ThrowInternalFailure("[MonoLibrary] Failed to get Mono Library Pointer from Internal Call!");
            return false;
        }

        try
        {
            Instance = NativeMonoPtr.ToNewNativeLibrary<MonoLibrary>().Instance;
        }
        catch (Exception ex)
        {
            MelonLogger.ThrowInternalFailure($"[MonoLibrary] Failed to load Mono NativeLibrary!\n{ex}");
            return false;
        }

        MelonDebug.Msg("[MonoLibrary] Setup Successful!");
        return true;
    }

    public static MonoLibrary Instance { get; private set; }

    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern Assembly CastManagedAssemblyPtr(IntPtr ptr);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr dmono_assembly_open_full(IntPtr filepath, IntPtr status, bool refonly);
    public dmono_assembly_open_full mono_assembly_open_full = null;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr dmono_assembly_get_object(IntPtr domain, IntPtr assembly);
    public dmono_assembly_get_object mono_assembly_get_object = null;
}
#endif