using MelonLoader.Bootstrap.Utils;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.RuntimeHandlers.Il2Cpp;

internal class Il2CppLib(Il2CppLib.MethodGetNameFn methodGetName)
{
    public required nint Handle { get; init; }

    public required InitFn Init { get; init; }
    public required RuntimeInvokeFn RuntimeInvoke { get; init; }
    public required GetAllAttachedThreadsDelegate GetAllAttachedThreads { get; init; }

    public static Il2CppLib? TryLoad(nint hRuntime)
    {
        if (!NativeFunc.GetExport<InitFn>(hRuntime, "il2cpp_init", out var init)
            || !NativeFunc.GetExport<RuntimeInvokeFn>(hRuntime, "il2cpp_runtime_invoke", out var runtimeInvoke)
            || !NativeFunc.GetExport<MethodGetNameFn>(hRuntime, "il2cpp_method_get_name", out var methodGetName)
            || !NativeFunc.GetExport<GetAllAttachedThreadsDelegate>(hRuntime, "il2cpp_thread_get_all_attached_threads", out var getAllAttached))
            return null;

        return new(methodGetName)
        {
            Handle = hRuntime,
            Init = init,
            RuntimeInvoke = runtimeInvoke,
            GetAllAttachedThreads = getAllAttached
        };
    }

    public string? GetMethodName(nint method)
    {
        return method == 0 ? null : Marshal.PtrToStringAnsi(methodGetName(method));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint InitFn(nint a);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint RuntimeInvokeFn(nint method, nint obj, nint args, nint exc);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate nint MethodGetNameFn(nint method);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr GetAllAttachedThreadsDelegate(ref UIntPtr size);
}

[StructLayout(LayoutKind.Sequential)]
public struct Il2CppObject
{
    private IntPtr data;
    private IntPtr monitor;
}

[StructLayout(LayoutKind.Sequential)]
public struct Il2CppInternalThread
{
    public Il2CppObject obj;
    public int lock_thread_id;
    public IntPtr handle;
    public IntPtr native_handle;
    public IntPtr cached_culture_info;
    public IntPtr name;
    public int name_len;
    public uint state;
    public IntPtr abort_exc;
    public int abort_state_handle;
    public ulong tid;
    public IntPtr debugger_thread;
    public IntPtr static_data;
    public IntPtr runtime_thread_info;
    public IntPtr current_appcontext;
    public IntPtr root_domain_thread;
    public IntPtr _serialized_principal;
    public int _serialized_principal_version;
    public IntPtr appdomain_refs;
    public int interruption_requested;
    public IntPtr synch_cs;
    public byte threadpool_thread;
    public byte thread_interrupt_requested;
    public int stack_size;
    public byte apartment_state;
    public int critical_region_level;
    public int managed_id;
    public uint small_id;
    public IntPtr manage_callback;
    public IntPtr interrupt_on_stop;
    public IntPtr flags;
    public IntPtr thread_pinning_ref;
    public IntPtr abort_protected_block_count;
    public int priority;
    public IntPtr owned_mutexes;
    public IntPtr suspended;
    public int self_suspended;
    public ulong thread_state;
    public ulong unused2;
    public IntPtr last;
}

[StructLayout(LayoutKind.Sequential)]
public struct Il2CppThread
{
    // Truncated just for the things needed
    public Il2CppObject obj;
    public IntPtr internal_thread;
}