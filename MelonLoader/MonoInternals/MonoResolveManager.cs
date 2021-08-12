using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader.MonoInternals
{
    public static class MonoResolveManager
    {
        private static MonoLib NativeMono = null;

        internal unsafe static bool Setup()
        {
            IntPtr NativeMonoPtr = MelonUtils.GetMonoLibraryPointer();
            if (NativeMonoPtr == IntPtr.Zero)
            {
                MelonLogger.ThrowInternalFailure("[MonoResolveManager] Failed to get Mono Library Pointer from Internal Call!");
                return false;
            }

            try { NativeMono = NativeMonoPtr.ToNewNativeLibrary<MonoLib>().Instance; }
            catch (Exception ex) { MelonLogger.ThrowInternalFailure($"[MonoResolveManager] Failed to load Mono NativeLibrary!\n{ex}"); return false; }

            if (!AssemblyHook.Setup())
                return false;

            MelonDebug.Msg("[MonoResolveManager] Setup Successful!");
            return true;
        }

        private static Assembly Resolve(string requested_name, Version requested_version)
        {
            // Sort through Listings and Return Something
            // Return null if Nothing Found
            return null;
        }

        private static class AssemblyHook
        {
            internal static bool Setup()
            {
                try
                {
                    NativeMono.mono_install_assembly_preload_hook(
                        Marshal.GetFunctionPointerForDelegate(HookFunc_Preload),
                        IntPtr.Zero
                    );

                    NativeMono.mono_install_assembly_search_hook(
                        Marshal.GetFunctionPointerForDelegate(HookFunc_Search),
                        IntPtr.Zero
                    );
                }
                catch (Exception ex) { MelonLogger.ThrowInternalFailure($"[MonoResolveManager] Failed to setup AssemblyHook!\n{ex}"); return false; }
                return true;
            }

            private unsafe static IntPtr HookFunc(IntPtr assemblyNamePtr)
            {
                string assemblyName_Name = null;
                Version assemblyName_Version = null;
                GetMonoAssemblyNameInfo(assemblyNamePtr, ref assemblyName_Name, ref assemblyName_Version);
                MelonDebug.Msg($"[MonoResolveManager] {assemblyName_Name}, Version={assemblyName_Version}");

                Assembly assembly = Resolve(assemblyName_Name, assemblyName_Version);
                if (assembly == null)
                {
                    MelonDebug.Msg("[MonoResolveManager] NULL");
                    return IntPtr.Zero;
                }
                MelonDebug.Msg($"[MonoResolveManager] {assembly.Location}");

                // Convert Assembly [MonoReflectionAssembly*] to MonoAssembly

                // Return MonoAssembly Pointer
                return IntPtr.Zero;
            }

            private unsafe static void GetMonoAssemblyNameInfo(
                IntPtr assemblyNamePtr, 
                ref string Out_Name,
                ref Version Out_Version
            ) {
                Out_Name = Marshal.PtrToStringAnsi(NativeMono.mono_assembly_name_get_name(assemblyNamePtr));

                ushort assemblyName_Version_Minor = 0;
                ushort assemblyName_Version_Build = 0;
                ushort assemblyName_Version_Revision = 0;
                ushort assemblyName_Version_Major = NativeMono.mono_assembly_name_get_version(
                    assemblyNamePtr, 
                    &assemblyName_Version_Minor,
                    &assemblyName_Version_Build, 
                    &assemblyName_Version_Revision
                );

                Out_Version = new Version(
                    assemblyName_Version_Major,
                    assemblyName_Version_Minor,
                    assemblyName_Version_Build,
                    assemblyName_Version_Revision
                );
            }

            private delegate IntPtr dHookFunc_Preload(IntPtr assemblyName, IntPtr assemblies_path, IntPtr user_data);
            private static dHookFunc_Preload HookFunc_Preload = (IntPtr assemblyName, IntPtr assemblies_path, IntPtr user_data) => HookFunc(assemblyName);
            private delegate IntPtr dHookFunc_Search(IntPtr assemblyName, IntPtr user_data);
            private static dHookFunc_Search HookFunc_Search = (IntPtr assemblyName, IntPtr user_data) => HookFunc(assemblyName);
        }

        internal unsafe class MonoLib
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate void dmono_install_assembly_preload_hook(IntPtr hookfunc, IntPtr user_data);
            internal dmono_install_assembly_preload_hook mono_install_assembly_preload_hook = null;
            internal dmono_install_assembly_preload_hook mono_install_assembly_search_hook = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate IntPtr dmono_assembly_name_get_name(IntPtr assemblyName);
            internal dmono_assembly_name_get_name mono_assembly_name_get_name = null;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate ushort dmono_assembly_name_get_version(IntPtr assemblyName, ushort* minor, ushort* build, ushort* revision);
            internal dmono_assembly_name_get_version mono_assembly_name_get_version = null;
        }
    }
}
