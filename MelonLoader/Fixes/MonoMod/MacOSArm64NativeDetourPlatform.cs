using System;
using System.Runtime.InteropServices;
using MonoMod.RuntimeDetour;

namespace MelonLoader.Fixes.MonoMod;

internal sealed class MacOSArm64NativeDetourPlatform : IDetourNativePlatform
{
    private readonly IDetourNativePlatform _inner;
    private static bool? _nativeJitCopySupported;

    public MacOSArm64NativeDetourPlatform(IDetourNativePlatform inner)
    {
        _inner = inner;
    }

    public NativeDetourData Create(IntPtr from, IntPtr to, byte? type = null)
    {
        return _inner.Create(from, to, type);
    }

    public void Free(NativeDetourData detour)
    {
        _inner.Free(detour);
    }

    public void Apply(NativeDetourData detour)
    {
        if (TryApplyWithNativeCopy(detour))
            return;

        _inner.Apply(detour);
    }

    public void Copy(IntPtr src, IntPtr dst, byte type)
    {
        if (TryCopyWithNativeCopy(src, dst, type))
            return;

        _inner.Copy(src, dst, type);
    }

    public void MakeWritable(IntPtr src, uint size)
    {
        if (NativeJitCopySupported)
            return;

        _inner.MakeWritable(src, size);
    }

    public void MakeExecutable(IntPtr src, uint size)
    {
        if (NativeJitCopySupported)
        {
            TryFlushICache(src, size);
            return;
        }

        _inner.MakeExecutable(src, size);
    }

    public void MakeReadWriteExecutable(IntPtr src, uint size)
    {
        if (NativeJitCopySupported)
        {
            TryFlushICache(src, size);
            return;
        }

        _inner.MakeReadWriteExecutable(src, size);
    }

    public void FlushICache(IntPtr src, uint size)
    {
        if (TryFlushICache(src, size))
            return;

        _inner.FlushICache(src, size);
    }

    public IntPtr MemAlloc(uint size)
    {
        return _inner.MemAlloc(size);
    }

    public void MemFree(IntPtr ptr)
    {
        _inner.MemFree(ptr);
    }

    private bool TryApplyWithNativeCopy(NativeDetourData detour)
    {
        if (detour.Size == 0 || detour.Extra != IntPtr.Zero || !NativeJitCopySupported)
            return false;

        IntPtr copy = Marshal.AllocHGlobal((int)detour.Size);
        try
        {
            NativeDetourData copyDetour = detour;
            copyDetour.Method = copy;
            _inner.Apply(copyDetour);

            return TryNativeJitCopy(detour.Method, copy, detour.Size);
        }
        finally
        {
            Marshal.FreeHGlobal(copy);
        }
    }

    private bool TryCopyWithNativeCopy(IntPtr src, IntPtr dst, byte type)
    {
        uint size = GetDetourSize(type);
        if (size == 0)
            return false;

        return TryNativeJitCopy(dst, src, size);
    }

    private static bool TryFlushICache(IntPtr src, uint size)
    {
        try
        {
            sys_icache_invalidate(src, (UIntPtr)size);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static uint GetDetourSize(byte type)
    {
        switch (type)
        {
            case 0:
                return 5;
            case 1:
                return 6;
            case 2:
                return 14;
            case 3:
                return 6;
            case 4:
                return 16;
            default:
                return 0;
        }
    }

    private static bool TryNativeJitCopy(IntPtr dst, IntPtr src, uint size)
    {
        if (!NativeJitCopySupported)
            return false;

        try
        {
            return CallNativeJitCopy(dst, src, (UIntPtr)size) != 0;
        }
        catch
        {
            _nativeJitCopySupported = false;
            return false;
        }
    }

    private static bool NativeJitCopySupported
    {
        get
        {
            if (_nativeJitCopySupported.HasValue)
                return _nativeJitCopySupported.Value;

            try
            {
                _nativeJitCopySupported = CallNativeJitCopy(IntPtr.Zero, IntPtr.Zero, UIntPtr.Zero) != 0;
            }
            catch
            {
                _nativeJitCopySupported = false;
            }

            return _nativeJitCopySupported.Value;
        }
    }

    private static int CallNativeJitCopy(IntPtr dst, IntPtr src, UIntPtr size)
    {
        try
        {
            return MLMacOSJitCopyBootstrap(dst, src, size);
        }
        catch
        {
            return MLMacOSJitCopyInternal(dst, src, size);
        }
    }

    [DllImport("MelonLoader.Bootstrap.dylib", EntryPoint = "MLMacOSJitCopy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int MLMacOSJitCopyBootstrap(IntPtr dst, IntPtr src, UIntPtr size);

    [DllImport("__Internal", EntryPoint = "MLMacOSJitCopy", CallingConvention = CallingConvention.Cdecl)]
    private static extern int MLMacOSJitCopyInternal(IntPtr dst, IntPtr src, UIntPtr size);

    [DllImport("libSystem.B.dylib", CallingConvention = CallingConvention.Cdecl)]
    private static extern void sys_icache_invalidate(IntPtr start, UIntPtr len);
}
