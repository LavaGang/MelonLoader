using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader.MonoInternals.ResolveInternals
{
    internal static class AssemblyManagerHooks
    {
        internal static bool Setup()
        {
            try
            {
                MonoLibrary.Instance.mono_install_assembly_preload_hook(
                    Marshal.GetFunctionPointerForDelegate(PreloadHook),
                    IntPtr.Zero
                );
            }
            catch (Exception ex)
            {
                MelonLogger.ThrowInternalFailure($"[ResolveInternals.AssemblyManagerHooks] Failed to Install Preload Hook!\n{ex}");
                return false;
            }

            try
            {
                MonoLibrary.Instance.mono_install_assembly_search_hook(
                    Marshal.GetFunctionPointerForDelegate(SearchHook),
                    IntPtr.Zero
                );
            }
            catch (Exception ex)
            {
                MelonLogger.ThrowInternalFailure($"[ResolveInternals.AssemblyManagerHooks] Failed to Install Search Hook!\n{ex}");
                return false;
            }

            try
            {
                MonoLibrary.Instance.mono_install_assembly_load_hook(
                    Marshal.GetFunctionPointerForDelegate(LoadHook),
                    IntPtr.Zero
                );
            }
            catch (Exception ex)
            {
                MelonLogger.ThrowInternalFailure($"[ResolveInternals.AssemblyManagerHooks] Failed to Install Load Hook!\n{ex}");
                return false;
            }

            return true;
        }

        private unsafe static IntPtr HookFunc_Resolve(IntPtr assemblyNamePtr, bool is_preload)
        {
            string assemblyName_Name = null;
            Version assemblyName_Version = null;
            GetMonoAssemblyNameInfo(assemblyNamePtr, ref assemblyName_Name, ref assemblyName_Version);

            Assembly assembly = AssemblyManager.Resolve(assemblyName_Name, assemblyName_Version, is_preload);

            // MonoReflectionAssembly*->assembly
            return (assembly != null) ? MonoLibrary.GetNativeAssemblyFromManagedAssembly(assembly) : IntPtr.Zero;
        }

        private static unsafe void HookFunc_Load(IntPtr monoAssembly)
        {
            // Get MonoDomain from MonoAppDomain
            IntPtr curDomainPtr = MonoLibrary.GetNativeDomainFromManagedAppDomain(AppDomain.CurrentDomain);
            if (curDomainPtr == IntPtr.Zero)
                return;

            // Get MonoReflectionAssembly Pointer from MonoAssembly
            IntPtr assemblyPtr = MonoLibrary.Instance.mono_assembly_get_object(curDomainPtr, monoAssembly);
            if (assemblyPtr == IntPtr.Zero)
                return;

            // Cast Pointer to MonoReflectionAssembly
            Assembly assembly = MonoLibrary.CastManagedAssemblyPtr(assemblyPtr);
            if (assembly == null)
                return;

            // Setup the Assembly
            AssemblyManager.LoadInfo(assembly);
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

        private unsafe delegate IntPtr dMonoAssemblyPreLoadFunc(IntPtr assemblyName, IntPtr assemblies_path, IntPtr user_data);
        private static dMonoAssemblyPreLoadFunc PreloadHook = (IntPtr assemblyName, IntPtr assemblies_path, IntPtr user_data) => HookFunc_Resolve(assemblyName, true);

        private unsafe delegate IntPtr dMonoAssemblySearchFunc(IntPtr assemblyName, IntPtr user_data);
        private static dMonoAssemblySearchFunc SearchHook = (IntPtr assemblyName, IntPtr user_data) => HookFunc_Resolve(assemblyName, false);

        private unsafe delegate void dMonoAssemblyLoadFunc(IntPtr monoAssembly, IntPtr user_data);
        private static dMonoAssemblyLoadFunc LoadHook = (IntPtr monoAssembly, IntPtr user_data) => HookFunc_Load(monoAssembly);
    }
}
