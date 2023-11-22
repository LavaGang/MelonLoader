using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Unity.Mono
{
    internal class MonoNativeLibrary
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_mono_jit_init_version([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string version);
        internal d_mono_jit_init_version mono_jit_init_version;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_mono_runtime_invoke(IntPtr method, IntPtr obj, IntPtr prams, IntPtr exec);
        internal d_mono_runtime_invoke mono_runtime_invoke;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_mono_thread_current();
        internal d_mono_thread_current mono_thread_current;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_mono_thread_set_main(IntPtr thread);
        internal d_mono_thread_set_main mono_thread_set_main;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_mono_domain_set_config(IntPtr domain, [MarshalAs(UnmanagedType.LPStr)] string configPath, [MarshalAs(UnmanagedType.LPStr)] string name);
        internal d_mono_domain_set_config mono_domain_set_config;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_mono_domain_assembly_open(IntPtr domain, [MarshalAs(UnmanagedType.LPStr)] string name);
        internal d_mono_domain_assembly_open mono_domain_assembly_open;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_mono_assembly_get_image(IntPtr assembly);
        internal d_mono_assembly_get_image mono_assembly_get_image;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_mono_class_from_name(IntPtr image, [MarshalAs(UnmanagedType.LPStr)] string namespaze, [MarshalAs(UnmanagedType.LPStr)] string name);
        internal d_mono_class_from_name mono_class_from_name;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_mono_class_get_method_from_name(IntPtr clazz, [MarshalAs(UnmanagedType.LPStr)] string name, int paramCount);
        internal d_mono_class_get_method_from_name mono_class_get_method_from_name;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal delegate string d_mono_method_get_name(IntPtr method);
        internal d_mono_method_get_name mono_method_get_name;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_mono_debug_domain_create(IntPtr domain);
        internal d_mono_debug_domain_create mono_debug_domain_create;

        internal string Validation()
        {
            (string, Delegate)[] majorList = new (string, Delegate)[]
            {
                    (nameof(mono_jit_init_version), mono_jit_init_version),
                    (nameof(mono_runtime_invoke), mono_runtime_invoke),
                    (nameof(mono_thread_current), mono_thread_current),
                    (nameof(mono_thread_set_main), mono_thread_set_main),
                    (nameof(mono_domain_set_config), mono_domain_set_config),
                    (nameof(mono_domain_assembly_open), mono_domain_assembly_open),
                    (nameof(mono_assembly_get_image), mono_assembly_get_image),
                    (nameof(mono_class_from_name), mono_class_from_name),
                    (nameof(mono_class_get_method_from_name), mono_class_get_method_from_name),
                    (nameof(mono_method_get_name), mono_method_get_name)
            };

            foreach (var obj in majorList)
                if (obj.Item2 == null)
                    return obj.Item1;

            return null;
        }
    }
}
