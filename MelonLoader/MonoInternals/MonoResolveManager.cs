using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader.MonoInternals
{
    public static class MonoResolveManager
    {
        private static readonly List<SearchDirectoryInfo> SearchDirectoryList = new List<SearchDirectoryInfo>();
        private static Dictionary<string, AssemblyResolveInfo> AssemblyDict = new Dictionary<string, AssemblyResolveInfo>();

        public delegate void OnAssemblyLoadHandler(Assembly assembly);
        public static event OnAssemblyLoadHandler OnAssemblyLoad;
        public delegate Assembly OnAssemblyResolveHandler(string name, Version version);
        public static event OnAssemblyResolveHandler OnAssemblyResolve;

        internal static bool Setup()
        {
            if (!Hooks.Setup())
                return false;

            // Setup Search Directories
            string[] searchdirlist =
            {
                MelonUtils.UserLibsDirectory,
                MelonHandler.PluginsDirectory,
                MelonHandler.ModsDirectory,
                MelonUtils.BaseDirectory,
                MelonUtils.GameDirectory
            };
            foreach (string path in searchdirlist)
                AddSearchDirectory(path);

            // Setup all Loaded Assemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                LoadAssembly(assembly);
            foreach (Assembly assembly in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
                LoadAssembly(assembly);

            // Setup Redirections
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
            Assembly base_assembly = typeof(MonoResolveManager).Assembly;
            foreach (string assemblyName in assembly_list)
                GetAssemblyInfo(assemblyName).Override = base_assembly;

            MelonDebug.Msg("[MonoResolveManager] Setup Successful!");

            return true;
        }

        private static Assembly ResolveAssembly(string requested_name, Version requested_version)
        {
            // Get Resolve Information Object
            AssemblyResolveInfo resolveInfo = GetAssemblyInfo(requested_name);

            // Resolve the Information Object
            Assembly assembly = resolveInfo.Resolve(requested_version);

            // Run Passthrough Events
            if (assembly == null)
                assembly = OnAssemblyResolve?.Invoke(requested_name, requested_version);

            // Search Directories

            // Load if Valid Assembly
            if (assembly != null)
                LoadAssembly(assembly);

            // Last Resort Get Any Loaded Version
            if ((assembly == null) && (resolveInfo.Versions.Count > 0))
                assembly = resolveInfo.Versions.OrderBy(x => x.Key).First().Value;

            // Return
            return assembly;
        }

        private static void LoadAssembly(Assembly assembly)
        {
            // Get AssemblyName
            AssemblyName assemblyName = assembly.GetName();

            // Get Resolve Information Object
            AssemblyResolveInfo resolveInfo = GetAssemblyInfo(assemblyName.Name);

            // Check for Existing Version
            if (resolveInfo.GetVersion(assemblyName.Version, out Assembly assembly2))
                return;

            // Set Version of Assembly
            resolveInfo.SetVersion(assemblyName.Version, assembly);

            // Run Passthrough Events
            OnAssemblyLoad?.Invoke(assembly);
        }

        public static AssemblyResolveInfo GetAssemblyInfo(string name)
        {
            if (AssemblyDict.TryGetValue(name, out AssemblyResolveInfo resolveInfo))
                return resolveInfo;
            lock (AssemblyDict)
                AssemblyDict[name] = new AssemblyResolveInfo();
            return AssemblyDict[name];
        }

        public static void AddSearchDirectory(string folder_path, int priority = 0)
        {
            if (string.IsNullOrEmpty(folder_path))
                return;

            folder_path = Path.GetFullPath(folder_path);
            if (Path.HasExtension(folder_path))
                return;

            SearchDirectoryInfo searchDirectory = SearchDirectoryList.FirstOrDefault(x => x.Path.Equals(folder_path));
            if (searchDirectory != null)
                return;

            searchDirectory = new SearchDirectoryInfo();
            searchDirectory.Path = folder_path;
            searchDirectory.Priority = priority;
            SearchDirectoryList.Add(searchDirectory);

            // Sort by Priority
        }

        private class SearchDirectoryInfo
        {
            internal string Path = null;
            internal int Priority = 0;
        }

        public class AssemblyResolveInfo
        {
            public Assembly Override = null;
            public Assembly Fallback = null;
            internal Dictionary<Version, Assembly> Versions = new Dictionary<Version, Assembly>();

            internal Assembly Resolve(Version requested_version)
            {
                // Check for Override
                if (Override != null)
                    return Override;

                // Check for Requested Version
                if ((requested_version != null)
                    && GetVersion(requested_version, out Assembly assembly))
                    return assembly;

                // Check for Fallback
                if (Fallback != null)
                    return Fallback;

                return null;
            }

            public void SetVersion(Version version, Assembly assembly = null)
            {
                lock (Versions)
                    Versions[version] = assembly;
            }
            public Assembly GetVersion(Version version)
                => GetVersion(version, out Assembly assembly)
                    ? assembly
                    : null;
            public bool GetVersion(Version version, out Assembly assembly)
                => Versions.TryGetValue(version, out assembly) && assembly != null;
        }

        private static class Hooks
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
                    MelonLogger.ThrowInternalFailure($"[MonoResolveManager] Failed to Install Preload Hook!\n{ex}");
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
                    MelonLogger.ThrowInternalFailure($"[MonoResolveManager] Failed to Install Search Hook!\n{ex}");
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
                    MelonLogger.ThrowInternalFailure($"[MonoResolveManager] Failed to Install Load Hook!\n{ex}");
                    return false;
                }

                return true;
            }

            private unsafe static IntPtr HookFunc_Resolve(IntPtr assemblyNamePtr)
            {
                string assemblyName_Name = null;
                Version assemblyName_Version = null;
                GetMonoAssemblyNameInfo(assemblyNamePtr, ref assemblyName_Name, ref assemblyName_Version);

                Assembly assembly = ResolveAssembly(assemblyName_Name, assemblyName_Version);

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
                LoadAssembly(assembly);
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
            private static dMonoAssemblyPreLoadFunc PreloadHook = (IntPtr assemblyName, IntPtr assemblies_path, IntPtr user_data) => HookFunc_Resolve(assemblyName);

            private unsafe delegate IntPtr dMonoAssemblySearchFunc(IntPtr assemblyName, IntPtr user_data);
            private static dMonoAssemblySearchFunc SearchHook = (IntPtr assemblyName, IntPtr user_data) => HookFunc_Resolve(assemblyName);

            private unsafe delegate void dMonoAssemblyLoadFunc(IntPtr monoAssembly, IntPtr user_data);
            private static dMonoAssemblyLoadFunc LoadHook = (IntPtr monoAssembly, IntPtr user_data) => HookFunc_Load(monoAssembly);
        }
    }
}
