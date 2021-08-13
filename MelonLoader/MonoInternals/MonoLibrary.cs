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
        public extern static IntPtr GetNativeAssemblyforManagedAssembly(Assembly asm);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void dmono_install_assembly_hook(IntPtr hookfunc, IntPtr user_data);
        internal dmono_install_assembly_hook mono_install_assembly_preload_hook = null;
        internal dmono_install_assembly_hook mono_install_assembly_search_hook = null;
        internal dmono_install_assembly_hook mono_install_assembly_refonly_search_hook = null;
        

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr dmono_assembly_name_get_name(IntPtr assemblyName);
        internal dmono_assembly_name_get_name mono_assembly_name_get_name = null;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate ushort dmono_assembly_name_get_version(IntPtr assemblyName, ushort* minor, ushort* build, ushort* revision);
        internal dmono_assembly_name_get_version mono_assembly_name_get_version = null;
    }
}
