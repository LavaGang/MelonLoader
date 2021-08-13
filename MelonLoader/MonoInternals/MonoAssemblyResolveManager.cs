using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader.MonoInternals
{
    public static class MonoAssemblyResolveManager
    {
        internal static bool Setup()
        {
            try
            {
                MonoLibrary.Instance.mono_install_assembly_preload_hook(
                    Marshal.GetFunctionPointerForDelegate(HookFunc_Preload),
                    IntPtr.Zero
                );
            }
            catch (Exception ex)
            {
                MelonLogger.ThrowInternalFailure($"[MonoAssemblyResolveManager] Failed to Install Preload Hook!\n{ex}");
                return false;
            }

            try
            {
                MonoLibrary.Instance.mono_install_assembly_search_hook(
                    Marshal.GetFunctionPointerForDelegate(HookFunc_Search),
                    IntPtr.Zero
                );
            }
            catch (Exception ex)
            {
                MelonLogger.ThrowInternalFailure($"[MonoAssemblyResolveManager] Failed to Install Search Hook!\n{ex}");
                return false;
            }

            try
            {
                MonoLibrary.Instance.mono_install_assembly_refonly_search_hook(
                    Marshal.GetFunctionPointerForDelegate(HookFunc_Search_RefOnly),
                    IntPtr.Zero
                );
            }
            catch (Exception ex)
            {
                MelonLogger.ThrowInternalFailure($"[MonoAssemblyResolveManager] Failed to Install Ref Only Search Hook!\n{ex}");
                return false;
            }

            MelonDebug.Msg("[MonoAssemblyResolveManager] Setup Successful!");
            return true;
        }

        private static Assembly Resolve(bool refOnly, string requested_name, Version requested_version)
        {
            // Sort through Listings and Return Something
            // Return null if Nothing Found
            
            return null;
        }

        private unsafe static IntPtr HookFunc(bool refOnly, IntPtr assemblyNamePtr)
        {
            // Temporarily Don't Mess with Ref Only
            if (refOnly)
                return IntPtr.Zero;

            string assemblyName_Name = null;
            Version assemblyName_Version = null;
            GetMonoAssemblyNameInfo(assemblyNamePtr, ref assemblyName_Name, ref assemblyName_Version);

            Assembly assembly = Resolve(refOnly, assemblyName_Name, assemblyName_Version);
            if (assembly == null)
                return IntPtr.Zero;
            return MonoLibrary.GetNativeAssemblyforManagedAssembly(assembly);
        }

        private unsafe static void GetMonoAssemblyNameInfo(IntPtr assemblyNamePtr, ref string Out_Name, ref Version Out_Version)
        {
            Out_Name = Marshal.PtrToStringAnsi(MonoLibrary.Instance.mono_assembly_name_get_name(assemblyNamePtr));

            ushort assemblyName_Version_Minor = 0;
            ushort assemblyName_Version_Build = 0;
            ushort assemblyName_Version_Revision = 0;
            ushort assemblyName_Version_Major = MonoLibrary.Instance.mono_assembly_name_get_version(
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

        private unsafe delegate IntPtr dHookFunc_Preload(IntPtr assemblyName, IntPtr assemblies_path, IntPtr user_data);
        private static dHookFunc_Preload HookFunc_Preload = (IntPtr assemblyName, IntPtr assemblies_path, IntPtr user_data) => HookFunc(false, assemblyName);
        private unsafe delegate IntPtr dHookFunc_Search(IntPtr assemblyName, IntPtr user_data);
        private static dHookFunc_Search HookFunc_Search = (IntPtr assemblyName, IntPtr user_data) => HookFunc(false, assemblyName);
        private static dHookFunc_Search HookFunc_Search_RefOnly = (IntPtr assemblyName, IntPtr user_data) => HookFunc(true, assemblyName);
    }
}
