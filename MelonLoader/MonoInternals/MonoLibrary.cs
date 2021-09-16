using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader.MonoInternals
{
    public class MonoLibrary
    {
        internal static bool Setup()
        {
            IntPtr NativeMonoPtr = GetLibPtr();
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
        private extern static IntPtr GetLibPtr();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr GetNativeAssemblyFromManagedAssembly(Assembly asm);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static IntPtr GetNativeDomainFromManagedAppDomain(AppDomain domain);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static Assembly CastManagedAssemblyPtr(IntPtr ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr dmono_assembly_open_full(IntPtr filepath, IntPtr status, bool refonly);
        public dmono_assembly_open_full mono_assembly_open_full = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr dmono_assembly_get_object(IntPtr domain, IntPtr assembly);
        public dmono_assembly_get_object mono_assembly_get_object = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void dmono_install_hook(IntPtr hookfunc, IntPtr user_data);
        public dmono_install_hook mono_install_assembly_preload_hook = null;
        public dmono_install_hook mono_install_assembly_load_hook = null;
        public dmono_install_hook mono_install_assembly_search_hook = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr dmono_assembly_name_get_name(IntPtr assemblyName);
        public dmono_assembly_name_get_name mono_assembly_name_get_name = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate ushort dmono_assembly_name_get_version(IntPtr assemblyName, ushort* minor, ushort* build, ushort* revision);
        public dmono_assembly_name_get_version mono_assembly_name_get_version = null;
    }
}
