using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader.Runtime.Mono
{
    public class MonoLibrary
    {
        #region Public Members

        public static MonoLibrary Instance { get; private set; }
        public static MelonNativeLibrary<MonoLibrary> Lib { get; private set; }

        #endregion

        #region Public Methods

        public static void Load(string filePath)
        {
            if (Instance != null)
                return;

            IntPtr libPtr = MelonNativeLibrary.LoadLib(filePath);
            if (libPtr == IntPtr.Zero)
                throw new Exception($"Failed to load {filePath}");

            Lib = new(libPtr, false);
            Instance = Lib.Instance;
        }

        #endregion

        #region Mono Domain

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_init_version(IntPtr name, IntPtr version);
        public d_mono_init_version mono_init_version { get; private set; }
        public d_mono_init_version mono_jit_init_version { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_domain_get();
        public d_mono_domain_get mono_domain_get { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void d_mono_debug_domain_create(IntPtr domain);
        public d_mono_debug_domain_create mono_debug_domain_create { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_domain_assembly_open(IntPtr domain, IntPtr filepath);
        public d_mono_domain_assembly_open mono_domain_assembly_open { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void d_mono_domain_set_config(IntPtr domain, string configPath, IntPtr name);
        public d_mono_domain_set_config mono_domain_set_config { get; private set; }

        #endregion

        #region Mono Assembly

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_assembly_open(IntPtr filepath, IntPtr status);
        public d_mono_assembly_open mono_assembly_open { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_assembly_open_full(IntPtr filepath, IntPtr status, bool refonly);
        public d_mono_assembly_open_full mono_assembly_open_full { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_assembly_get_image(IntPtr assembly);
        public d_mono_assembly_get_image mono_assembly_get_image { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate MonoReflectionAssembly* d_mono_assembly_get_object(IntPtr domain, IntPtr assembly);
        public d_mono_assembly_get_object mono_assembly_get_object { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_install_assembly_preload_hook(AssemblyPreloadHookFn func, IntPtr userData);
        public d_mono_install_assembly_preload_hook mono_install_assembly_preload_hook { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_install_assembly_search_hook(AssemblySearchHookFn func, IntPtr userData);
        public d_mono_install_assembly_search_hook mono_install_assembly_search_hook { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_install_assembly_load_hook(AssemblyLoadHookFn func, IntPtr userData);
        public d_mono_install_assembly_load_hook mono_install_assembly_load_hook { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [MelonNativeLibraryImport("mono_assembly_get_object")]
        public unsafe delegate Assembly d_mono_assembly_get_object_managed(IntPtr domain, IntPtr assembly);
        public d_mono_assembly_get_object_managed mono_assembly_get_object_managed { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void d_mono_domain_set(IntPtr domain, int force);
        public d_mono_domain_set mono_domain_set { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr AssemblyPreloadHookFn(ref MonoAssemblyName assemblyName, IntPtr assemblyPaths, IntPtr userData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr AssemblySearchHookFn(ref MonoAssemblyName assemblyName, IntPtr userData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AssemblyLoadHookFn(IntPtr monoAssembly, IntPtr userData);

        #endregion

        #region Mono Class

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_class_from_name(IntPtr image, IntPtr namespaze, IntPtr name);
        public d_mono_class_from_name mono_class_from_name { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_class_get_method_from_name(IntPtr clazz, IntPtr name, int paramCount);
        public d_mono_class_get_method_from_name mono_class_get_method_from_name { get; private set; }

        #endregion

        #region Mono Method

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_method_get_name(IntPtr method);
        public d_mono_method_get_name mono_method_get_name { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe delegate IntPtr d_mono_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        public d_mono_runtime_invoke mono_runtime_invoke { get; private set; }

        #endregion

        #region Mono String

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_string_new(IntPtr domain, IntPtr str);
        public d_mono_string_new mono_string_new { get; private set; }

        #endregion

        #region Mono Thread

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_thread_current();
        public d_mono_thread_current mono_thread_current { get; private set; }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_mono_thread_set_main(IntPtr thread);
        public d_mono_thread_set_main mono_thread_set_main { get; private set; }

        #endregion

        #region Utility Methods

        public unsafe IntPtr TryInvokeMethod(IntPtr method, IntPtr obj)
        {
            try { return InvokeMethod(method, obj, (void**)IntPtr.Zero); }
            catch { }
            return IntPtr.Zero;
        }

        public unsafe IntPtr InvokeMethod(IntPtr method, IntPtr obj)
            => InvokeMethod(method, obj, (void**)IntPtr.Zero);

        public unsafe IntPtr TryInvokeMethod(IntPtr method, IntPtr obj, params IntPtr[] parameters)
        {
            try { return InvokeMethod(method, obj, parameters); }
            catch { }
            return IntPtr.Zero;
        }

        public unsafe IntPtr InvokeMethod(IntPtr method, IntPtr obj, params IntPtr[] parameters)
        {
            fixed (void* parameterF = &parameters[0])
                return InvokeMethod(method, obj, (void**)parameterF);
        }

        public unsafe IntPtr TryInvokeMethod(IntPtr method, IntPtr obj, params void*[] parameters)
        {
            try { return InvokeMethod(method, obj, parameters); }
            catch { }
            return IntPtr.Zero;
        }

        public unsafe IntPtr InvokeMethod(IntPtr method, IntPtr obj, params void*[] parameters)
        {
            fixed (void** parameterF = &parameters[0])
                return InvokeMethod(method, obj, parameterF);
        }

        public unsafe IntPtr TryInvokeMethod(IntPtr method, IntPtr obj, void** param)
        {
            try { return InvokeMethod(method, obj, param); }
            catch { }
            return IntPtr.Zero;
        }

        public unsafe IntPtr InvokeMethod(IntPtr method, IntPtr obj, void** param)
        {
            if (method == IntPtr.Zero)
                return IntPtr.Zero;

            IntPtr exc = IntPtr.Zero;
            IntPtr returnVal = mono_runtime_invoke(method, obj, param, ref exc);

            //TODO: Exception to String
            if (exc != IntPtr.Zero)
                throw new Exception("Exception occurred in Mono Method!");

            return returnVal;
        }

        #endregion
    }
}