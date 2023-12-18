using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !NET6_0
using MelonLoader.NativeUtils;
#endif

namespace MelonLoader.Mono
{
    public class MonoLibrary
    {
        #region Private Members

        private static MonoLibrary _instance;

        #endregion

        #region Public Members

        public static MonoLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
#if !NET6_0
                    _instance = MelonNativeLibrary.ReflectiveLoad<MonoLibrary>(GetLibPtr());
#endif
                }

                return _instance;
            }
            set
            {
                if (_instance != null)
                    return;
                _instance = value;
            }
        }

        #endregion

        #region Private Methods

#if !NET6_0
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern IntPtr GetLibPtr();
#endif

        #endregion

        #region Mono Domain

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_init_version(IntPtr name, IntPtr version);
        public d_mono_init_version mono_init_version;
        public d_mono_init_version mono_jit_init_version;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_domain_get();
        public d_mono_domain_get mono_domain_get;

        #endregion

        #region Mono Assembly

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_assembly_open(IntPtr filepath, IntPtr status);
        public d_mono_assembly_open mono_assembly_open = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_assembly_open_full(IntPtr filepath, IntPtr status, bool refonly);
        public d_mono_assembly_open_full mono_assembly_open_full = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_assembly_get_image(IntPtr assembly);
        public d_mono_assembly_get_image mono_assembly_get_image;

        #endregion

        #region Mono Class

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_class_from_name(IntPtr image, IntPtr namespaze, IntPtr name);
        public d_mono_class_from_name mono_class_from_name;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_class_get_method_from_name(IntPtr clazz, IntPtr name, int paramCount);
        public d_mono_class_get_method_from_name mono_class_get_method_from_name;

        #endregion

        #region Mono Method

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_method_get_name(IntPtr method);
        public d_mono_method_get_name mono_method_get_name;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public unsafe delegate IntPtr d_mono_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        public d_mono_runtime_invoke mono_runtime_invoke;

        #endregion
        
        #region Mono String

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate IntPtr d_mono_string_new(IntPtr domain, IntPtr str);
        public d_mono_string_new mono_string_new;

        #endregion
        
        #region Utility Methods
        
        public unsafe IntPtr InvokeMethod(IntPtr method, IntPtr obj)
        {
            return InvokeMethod(method, obj, (void**)IntPtr.Zero);
        }
        
        public unsafe IntPtr InvokeMethod(IntPtr method, IntPtr obj, params IntPtr[] parameters)
        {
            fixed (void* parameterF = &parameters[0])
                return InvokeMethod(method, obj, (void**)parameterF);
        }
        
        public unsafe IntPtr InvokeMethod(IntPtr method, IntPtr obj, void** param)
        {
            if (method == IntPtr.Zero)
                return IntPtr.Zero;

            IntPtr exc = IntPtr.Zero;
            IntPtr returnVal = mono_runtime_invoke(method, obj, param, ref exc);

            //TODO: Exception to String
            if (exc != IntPtr.Zero)
            {
                throw new InvalidOperationException("Exception occurred in Mono Method!");
            }
            
            return returnVal;
        }

        #endregion
    }
}
