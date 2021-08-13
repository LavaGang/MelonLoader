using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader.MonoInternals
{
    public static class MonoAssemblyResolveManager
    {
        private static Dictionary<string, AssemblyResolveInfo> Listings = new Dictionary<string, AssemblyResolveInfo>();
        private static Dictionary<string, AssemblyResolveInfo> Listings_RefOnly = new Dictionary<string, AssemblyResolveInfo>();

        internal static bool Setup()
        {
            if (!Hooks.Setup())
                return false;

            string[] assembly_list =
            {
                "MelonLoader",
                "MelonLoader.ModHandler",
                "Mono.Cecil",
                "Mono.Cecil.Mdb",
                "Mono.Cecil.Pdb",
                "Mono.Cecil.Rocks",
                "MonoMod.RuntimeDetour",
                "MonoMod.Utils",
                "0Harmony",
                "Tomlet"
            };
            Assembly base_assembly = typeof(MonoAssemblyResolveManager).Assembly;
            foreach (string assemblyName in assembly_list)
                GetInfo(assemblyName).MasterOverride = base_assembly;

            return true;
        }

        private static Assembly Resolve(bool refOnly, string requested_name, Version requested_version)
        {
            // Check for Existing Info
            AssemblyResolveInfo resolveInfo = GetInfo(requested_name, refOnly, requested_version);
            if (resolveInfo != null)
            {
                Assembly resolveInfoAssembly = resolveInfo.GetAssembly();
                if (resolveInfoAssembly != null)
                    return resolveInfoAssembly;
            }

            // Scan Directories, Load and Add Listing if Found

            return null;
        }

        public static AssemblyResolveInfo GetInfo(string name, bool refOnly = false, Version version = null)
        {
            if (refOnly)
            {
                if (Listings_RefOnly.TryGetValue(name, out AssemblyResolveInfo resolveInfo))
                    return resolveInfo;
                Listings_RefOnly[name] = new AssemblyResolveInfo();
                return Listings_RefOnly[name];
            }

            if (Listings.TryGetValue(name, out AssemblyResolveInfo resolveInfo))
                return resolveInfo;
            Listings[name] = new AssemblyResolveInfo();
            return Listings[name];
        }

        public class AssemblyResolveInfo
        {
            public Assembly MasterOverride = null;
            public Assembly Default = null;
            public Dictionary<Version, Assembly> Versions = new Dictionary<Version, Assembly>();

            internal Assembly GetAssembly(Version requested_version = null)
            {
                // Check for Master Override
                if (MasterOverride != null)
                    return MasterOverride;

                // Check for Requested Version
                if ((requested_version != null) 
                    && Versions.TryGetValue(requested_version, out Assembly assembly))
                    return assembly;

                // Check for Default Version
                if (Default != null)
                    return Default;

                return null;
            }
        }

        private static class Hooks
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

            private unsafe static IntPtr HookFunc(bool refOnly, IntPtr assemblyNamePtr)
            {
                string assemblyName_Name = null;
                Version assemblyName_Version = null;
                GetMonoAssemblyNameInfo(assemblyNamePtr, ref assemblyName_Name, ref assemblyName_Version);

                Assembly assembly = Resolve(refOnly, assemblyName_Name, assemblyName_Version);
                if (assembly == null)
                    return IntPtr.Zero;
                MelonDebug.Msg($"{assembly.GetName()}, Location=\"{assembly.Location}\"");
                return MonoLibrary.GetNativeAssemblyforManagedAssembly(assembly); // MonoReflectionAssembly*->assembly
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
            private static dMonoAssemblyPreLoadFunc HookFunc_Preload = (IntPtr assemblyName, IntPtr assemblies_path, IntPtr user_data) => HookFunc(false, assemblyName);
            private unsafe delegate IntPtr dMonoAssemblySearchFunc(IntPtr assemblyName, IntPtr user_data);
            private static dMonoAssemblySearchFunc HookFunc_Search = (IntPtr assemblyName, IntPtr user_data) => HookFunc(false, assemblyName);
            private static dMonoAssemblySearchFunc HookFunc_Search_RefOnly = (IntPtr assemblyName, IntPtr user_data) => HookFunc(true, assemblyName);
        }
    }
}
