using MelonLoader.Resolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader.Runtime.Mono
{
    internal static class MonoAssemblyResolver
    {
        private static readonly List<Delegate> passedDelegates = new();

        private static IntPtr mlResolve;
        private static IntPtr mlLoadInfoFromAssembly;

        internal static unsafe void Initialize(string managedPath)
        {
            Type resolverType = typeof(MelonAssemblyResolver);
            string resolverAsmLocation = resolverType.Assembly.Location;

            var asm = MonoLibrary.Instance.mono_assembly_open_full(resolverAsmLocation.ToAnsiPointer(), IntPtr.Zero, false);
            var image = MonoLibrary.Instance.mono_assembly_get_image(asm);
            var resolverClass = MonoLibrary.Instance.mono_class_from_name(image, resolverType.Namespace.ToAnsiPointer(), resolverType.Name.ToAnsiPointer());
            mlResolve = MonoLibrary.Instance.mono_class_get_method_from_name(resolverClass, nameof(MelonAssemblyResolver.Resolve).ToAnsiPointer(), 3);
            mlLoadInfoFromAssembly = MonoLibrary.Instance.mono_class_get_method_from_name(resolverClass, nameof(MelonAssemblyResolver.LoadInfoFromAssembly).ToAnsiPointer(), 1);

            // Apply Assembly Resolver Hooks
            MelonAssemblyResolver.AddSearchDirectory(managedPath);
            InstallAssemblyHooks(OnAssemblyPreload, OnAssemblySearch, OnAssemblyLoad);
        }

        private static void InstallAssemblyHooks(MonoLibrary.AssemblyPreloadHookFn preloadHook,
            MonoLibrary.AssemblySearchHookFn searchHook,
            MonoLibrary.AssemblyLoadHookFn loadHook)
        {
            if (preloadHook != null)
            {
                passedDelegates.Add(preloadHook);
                MonoLibrary.Instance.mono_install_assembly_preload_hook(preloadHook, IntPtr.Zero);
            }

            if (searchHook != null)
            {
                passedDelegates.Add(searchHook);
                MonoLibrary.Instance.mono_install_assembly_search_hook(searchHook, IntPtr.Zero);
            }

            if (loadHook != null)
            {
                passedDelegates.Add(loadHook);
                MonoLibrary.Instance.mono_install_assembly_load_hook(loadHook, IntPtr.Zero);
            }
        }
        private static unsafe void OnAssemblyLoad(IntPtr monoAssembly, IntPtr userData)
        {
            if (monoAssembly == IntPtr.Zero)
                return;
            
            IntPtr domain = MonoLibrary.Instance.mono_domain_get();
            if (domain == IntPtr.Zero)
                return;

            IntPtr reflectionAsm = (IntPtr)MonoLibrary.Instance.mono_assembly_get_object(domain, monoAssembly);
            if (reflectionAsm == IntPtr.Zero)
                return;

            MonoLibrary.Instance.TryInvokeMethod(mlLoadInfoFromAssembly, IntPtr.Zero, monoAssembly);
        }

        private unsafe static IntPtr OnAssemblySearch(ref MonoAssemblyName assemblyName, IntPtr userData)
            => ResolveAssembly(assemblyName, false);
        private unsafe static IntPtr OnAssemblyPreload(ref MonoAssemblyName assemblyName, IntPtr assemblyPaths, IntPtr userData)
            => ResolveAssembly(assemblyName, true);

        private static unsafe IntPtr ResolveAssembly(MonoAssemblyName assemblyName, bool is_preload)
        {
            IntPtr domain = MonoLibrary.Instance.mono_domain_get();
            if (domain == IntPtr.Zero)
                return IntPtr.Zero;

            IntPtr reflectionAsm = MonoLibrary.Instance.TryInvokeMethod(mlResolve, IntPtr.Zero, 
                (void*)MonoLibrary.Instance.mono_string_new(domain, assemblyName.Name),
                &assemblyName.Major,
                &assemblyName.Minor,
                &assemblyName.Build,
                &assemblyName.Revision,
                &is_preload);

            return (reflectionAsm == IntPtr.Zero)
                ? IntPtr.Zero 
                : ((MonoReflectionAssembly*)reflectionAsm)->Assembly;
        }
    }
}