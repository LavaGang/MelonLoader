using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#pragma warning disable 0649

namespace MelonLoader
{
    public static class Il2Cpp
    {
        public static string IntPtrToString(IntPtr ptr) => Marshal.PtrToStringAnsi(ptr);
        public static IntPtr StringToIntPtr(string str) => il2cpp_string_new(str);

        internal static IntPtr MethodBaseToIntPtr(MethodBase method)
        {
            if (UnhollowerSupport.IsGeneratedAssemblyType(method.DeclaringType))
            {
                FieldInfo methodptr = method.DeclaringType.GetFields(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name.StartsWith(("NativeMethodInfoPtr_" + method.Name)));
                if (methodptr != null)
                    return (IntPtr)methodptr.GetValue(null);
            }
            else
                return method.MethodHandle.GetFunctionPointer();
            return IntPtr.Zero;
        }
        internal static IntPtr MethodInfoToIntPtr(MethodInfo method)
        {
            if (UnhollowerSupport.IsGeneratedAssemblyType(method.DeclaringType))
            {
                FieldInfo methodptr = method.DeclaringType.GetFields(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name.StartsWith(("NativeMethodInfoPtr_" + method.Name)));
                if (methodptr != null)
                    return (IntPtr)methodptr.GetValue(null);
            }
            else
                return method.MethodHandle.GetFunctionPointer();
            return IntPtr.Zero;
        }

        unsafe public static IntPtr InvokeMethod(IntPtr method, IntPtr obj, params IntPtr[] paramtbl)
        {
            if (method == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr[] intPtrArray;
            IntPtr returnval = IntPtr.Zero;
            intPtrArray = ((paramtbl != null) ? paramtbl : new IntPtr[0]);
            IntPtr intPtr = Marshal.AllocHGlobal(intPtrArray.Length * sizeof(void*));
            try
            {
                void** pointerArray = (void**)intPtr.ToPointer();
                for (int i = 0; i < intPtrArray.Length; i++)
                    pointerArray[i] = intPtrArray[i].ToPointer();
                IntPtr exp = IntPtr.Zero;
                returnval = il2cpp_runtime_invoke(method, obj, pointerArray, ref exp);
                if (exp.ToInt32() != 0)
                {
                    // There was an exception! Handle it somehow here
                    string message = "";
                    var ptr = Marshal.AllocHGlobal(4097);
                    try
                    {
                        // 4096 is the largest length of an exception. If not, this can be changed.
                        // TODO: Make this a constant/predefine instead of a magic number
                        il2cpp_format_exception(exp, ptr, 4096);
                        message = Marshal.PtrToStringAnsi(ptr);
                        throw new InvalidOperationException($"Invoke failed with exception: {message}");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                    throw new InvalidOperationException($"Invoke failed! Could not get exception from invoke failure!");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
            return returnval;
        }

        private struct Void { };
        unsafe private static IntPtr IntPtrAdd(IntPtr pointer, Int32 offset) => (IntPtr)((Void*)pointer + offset);
        public static T IntPtrToStructure<T>(IntPtr ptr) => (T)Marshal.PtrToStructure(ptr, typeof(T));
        public static T[] IntPtrToStructureArray<T>(IntPtr ptr, uint len)
        {
            IntPtr iter = ptr;
            T[] arr = new T[len];
            for (uint i = 0; i < len; i++)
            {
                arr[i] = IntPtrToStructure<T>(ptr);
                iter = IntPtrAdd(iter, Marshal.SizeOf(typeof(T)));
            }
            return arr;
        }
        unsafe public static IntPtr[] IntPtrToArray(IntPtr ptr)
        {
            long length = *((long*)ptr + 3);
            IntPtr[] result = new IntPtr[length];
            for (int i = 0; i < length; i++)
                result[i] = *(IntPtr*)(IntPtrAdd((IntPtr)((long*)ptr + 4), (i * IntPtr.Size)));
            return result;
        }

        public enum Il2CppStat
        {
            NEW_OBJECT_COUNT,
            INITIALIZED_CLASS_COUNT,
            //GENERIC_VTABLE_COUNT,
            //USED_CLASS_COUNT,
            METHOD_COUNT,
            //CLASS_VTABLE_SIZE,
            CLASS_STATIC_DATA_SIZE,
            GENERIC_INSTANCE_COUNT,
            GENERIC_CLASS_COUNT,
            INFLATED_METHOD_COUNT,
            INFLATED_TYPE_COUNT,
            //DELEGATE_CREATIONS,
            //MINOR_GC_COUNT,
            //MAJOR_GC_COUNT,
            //MINOR_GC_TIME_USECS,
            //MAJOR_GC_TIME_USECS
        }

        public enum Il2CppProfileFlags
        {
            NONE = 0,
            APPDOMAIN_EVENTS = 1 << 0,
            ASSEMBLY_EVENTS = 1 << 1,
            MODULE_EVENTS = 1 << 2,
            CLASS_EVENTS = 1 << 3,
            JIT_COMPILATION = 1 << 4,
            INLINING = 1 << 5,
            EXCEPTIONS = 1 << 6,
            ALLOCATIONS = 1 << 7,
            GC = 1 << 8,
            THREADS = 1 << 9,
            REMOTING = 1 << 10,
            TRANSITIONS = 1 << 11,
            ENTER_LEAVE = 1 << 12,
            COVERAGE = 1 << 13,
            INS_COVERAGE = 1 << 14,
            STATISTICAL = 1 << 15,
            METHOD_EVENTS = 1 << 16,
            MONITOR_EVENTS = 1 << 17,
            IOMAP_EVENTS = 1 << 18, /* this should likely be removed, too */
            GC_MOVES = 1 << 19,
            FILEIO = 1 << 20
        }

        public enum Il2CppRuntimeUnhandledExceptionPolicy
        {   
            IL2CPP_UNHANDLED_POLICY_LEGACY,
            IL2CPP_UNHANDLED_POLICY_CURRENT
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_init(IntPtr domain_name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_init_utf16(IntPtr domain_name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_shutdown();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_config_dir(IntPtr config_path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_data_dir(IntPtr data_path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_temp_dir(IntPtr temp_path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_commandline_arguments(int argc, IntPtr argv, IntPtr basedir);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_commandline_arguments_utf16(int argc, IntPtr argv, IntPtr basedir);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_config_utf16(IntPtr executablePath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_config(IntPtr executablePath);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_memory_callbacks(IntPtr callbacks);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_get_corlib();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_add_internal_call(IntPtr name, IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_resolve_icall(IntPtr name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_alloc(uint size);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_free(IntPtr ptr);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_array_class_get(IntPtr element_class, uint rank);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_array_length(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_array_get_byte_length(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_array_new(IntPtr elementTypeInfo, ulong length);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_array_new_specific(IntPtr arrayTypeInfo, ulong length);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_array_new_full(IntPtr array_class, ref ulong lengths, ref ulong lower_bounds);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_bounded_array_class_get(IntPtr element_class, uint rank, bool bounded);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_array_element_size(IntPtr array_class);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_assembly_get_image(IntPtr assembly);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_enum_basetype(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_generic(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_inflated(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_assignable_from(IntPtr klass, IntPtr oklass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_subclass_of(IntPtr klass, IntPtr klassc, bool check_interfaces);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_has_parent(IntPtr klass, IntPtr klassc);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_from_il2cpp_type(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_from_name(IntPtr image, IntPtr namespaze, IntPtr name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_from_system_type(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_element_class(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_events(IntPtr klass, ref IntPtr iter);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_fields(IntPtr klass, ref IntPtr iter);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_nested_types(IntPtr klass, ref IntPtr iter);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_interfaces(IntPtr klass, ref IntPtr iter);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_properties(IntPtr klass, ref IntPtr iter);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_property_from_name(IntPtr klass, IntPtr name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_field_from_name(IntPtr klass, IntPtr name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_methods(IntPtr klass, ref IntPtr iter);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_method_from_name(IntPtr klass, IntPtr name, int argsCount);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_name(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_namespace(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_parent(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_declaring_type(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_class_instance_size(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_class_num_fields(IntPtr enumKlass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_valuetype(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_class_value_size(IntPtr klass, ref uint align);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_blittable(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_class_get_flags(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_abstract(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_interface(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_class_array_element_size(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_from_type(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_type(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_class_get_type_token(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_has_attribute(IntPtr klass, IntPtr attr_class);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_has_references(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_class_is_enum(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_image(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_class_get_assemblyname(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_class_get_rank(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_class_get_bitmap_size(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_class_get_bitmap(IntPtr klass, ref uint bitmap);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_stats_dump_to_file(IntPtr path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static ulong il2cpp_stats_get_value(Il2CppStat stat);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_domain_get();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_domain_assembly_open(IntPtr domain, IntPtr name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_domain_get_assemblies(IntPtr domain, ref uint size);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_exception_from_name_msg(IntPtr image, IntPtr name_space, IntPtr name, IntPtr msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_get_exception_argument_null(IntPtr arg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_format_exception(IntPtr ex, IntPtr message, int message_size);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_format_stack_trace(IntPtr ex, IntPtr output, int output_size);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_unhandled_exception(IntPtr ex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_field_get_flags(IntPtr field);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_field_get_name(IntPtr field);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_field_get_parent(IntPtr field);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_field_get_offset(IntPtr field);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_field_get_type(IntPtr field);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_field_get_value(IntPtr obj, IntPtr field, ref IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_field_get_value_object(IntPtr field, IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_field_has_attribute(IntPtr field, IntPtr attr_class);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_field_set_value(IntPtr obj, IntPtr field, IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_field_static_get_value(IntPtr field, ref IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_field_static_set_value(IntPtr field, IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_field_set_value_object(IntPtr instance, IntPtr field, IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_gc_collect(int maxGenerations);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_gc_collect_a_little();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_gc_disable();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_gc_enable();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_gc_is_disabled();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static long il2cpp_gc_get_used_size();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static long il2cpp_gc_get_heap_size();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_gc_wbarrier_set_field(IntPtr obj, out IntPtr targetAddress, IntPtr gcObj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_gchandle_new(IntPtr obj, bool pinned);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_gchandle_new_weakref(IntPtr obj, bool track_resurrection);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_gchandle_get_target(uint gchandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_gchandle_free(uint gchandle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_unity_liveness_calculation_begin(IntPtr filter, int max_object_count, IntPtr callback, IntPtr userdata, IntPtr onWorldStarted, IntPtr onWorldStopped);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_unity_liveness_calculation_end(IntPtr state);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_unity_liveness_calculation_from_root(IntPtr root, IntPtr state);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_unity_liveness_calculation_from_statics(IntPtr state);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_method_get_return_type(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_method_get_declaring_type(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_method_get_name(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_method_get_from_reflection(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_method_get_object(IntPtr method, IntPtr refclass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_method_is_generic(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_method_is_inflated(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_method_is_instance(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_method_get_param_count(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_method_get_param(IntPtr method, uint index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_method_get_class(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_method_has_attribute(IntPtr method, IntPtr attr_class);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_method_get_flags(IntPtr method, ref uint iflags);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_method_get_token(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_method_get_param_name(IntPtr method, uint index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_profiler_install(IntPtr prof, IntPtr shutdown_callback);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_profiler_set_events(Il2CppProfileFlags events);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_profiler_install_enter_leave(IntPtr enter, IntPtr fleave);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_profiler_install_allocation(IntPtr callback);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_profiler_install_gc(IntPtr callback, IntPtr heap_resize_callback);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_profiler_install_fileio(IntPtr callback);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_profiler_install_thread(IntPtr start, IntPtr end);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_property_get_flags(IntPtr prop);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_property_get_get_method(IntPtr prop);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_property_get_set_method(IntPtr prop);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_property_get_name(IntPtr prop);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_property_get_parent(IntPtr prop);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_object_get_class(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_object_get_size(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_object_get_virtual_method(IntPtr obj, IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_object_new(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_object_unbox(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_value_box(IntPtr klass, IntPtr data);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_monitor_enter(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_monitor_try_enter(IntPtr obj, uint timeout);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_monitor_exit(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_monitor_pulse(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_monitor_pulse_all(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_monitor_wait(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_monitor_try_wait(IntPtr obj, uint timeout);
        [MethodImpl(MethodImplOptions.InternalCall)]
        unsafe public extern static IntPtr il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        [MethodImpl(MethodImplOptions.InternalCall)]
        unsafe public extern static IntPtr il2cpp_runtime_invoke_convert_args(IntPtr method, IntPtr obj, void** param, int paramCount, ref IntPtr exc);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_runtime_class_init(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_runtime_object_init(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_runtime_object_init_exception(IntPtr obj, ref IntPtr exc);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_runtime_unhandled_exception_policy_set(Il2CppRuntimeUnhandledExceptionPolicy value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_string_length(IntPtr str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        unsafe public extern static char* il2cpp_string_chars(IntPtr str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_string_new(string str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_string_new_len(string str, uint length);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_string_new_utf16(string text, int len);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_string_new_wrapper(string str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_string_intern(string str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_string_is_interned(string str);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_thread_current();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_thread_attach(IntPtr domain);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_thread_detach(IntPtr thread);
        [MethodImpl(MethodImplOptions.InternalCall)]
        unsafe public extern static void** il2cpp_thread_get_all_attached_threads(ref uint size);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_is_vm_thread(IntPtr thread);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_current_thread_walk_frame_stack(IntPtr func, IntPtr user_data);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_thread_walk_frame_stack(IntPtr thread, IntPtr func, IntPtr user_data);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_current_thread_get_top_frame(IntPtr frame);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_thread_get_top_frame(IntPtr thread, IntPtr frame);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_current_thread_get_frame_at(int offset, IntPtr frame);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_thread_get_frame_at(IntPtr thread, int offset, IntPtr frame);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_current_thread_get_stack_depth();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_thread_get_stack_depth(IntPtr thread);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_type_get_object(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static int il2cpp_type_get_type(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_type_get_class_or_element_class(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static string il2cpp_type_get_name(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_type_is_byref(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_type_get_attrs(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_type_equals(IntPtr type, IntPtr otherType);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_type_get_assembly_qualified_name(IntPtr type);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_image_get_assembly(IntPtr image);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_image_get_name(IntPtr image);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_image_get_filename(IntPtr image);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_image_get_entry_point(IntPtr image);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static uint il2cpp_image_get_class_count(IntPtr image);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_image_get_class(IntPtr image, uint index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_capture_memory_snapshot();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_free_captured_memory_snapshot(IntPtr snapshot);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_set_find_plugin_callback(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_register_log_callback(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_debugger_set_agent_options(IntPtr options);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_is_debugger_attached();
        [MethodImpl(MethodImplOptions.InternalCall)]
        unsafe public extern static void il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_custom_attrs_from_class(IntPtr klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_custom_attrs_from_method(IntPtr method);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_custom_attrs_get_attr(IntPtr ainfo, IntPtr attr_klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool il2cpp_custom_attrs_has_attr(IntPtr ainfo, IntPtr attr_klass);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr il2cpp_custom_attrs_construct(IntPtr cinfo);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void il2cpp_custom_attrs_free(IntPtr ainfo);
    }
}
