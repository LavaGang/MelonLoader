using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#else
using System;
using System.Runtime.InteropServices;
using MelonLoader.Utils;
#endif

namespace MelonLoader.Resolver;

internal static class SearchDirectoryManager
{
    private static List<SearchDirectoryInfo> SearchDirectoryList = [];

    private static void Sort()
        => SearchDirectoryList =
            SearchDirectoryList.OrderBy(x => x.Priority).ToList();

    internal static void Add(string path, int priority = 0)
    {
        if (string.IsNullOrEmpty(path))
            return;

        path = Path.GetFullPath(path);
        if (path.ContainsExtension())
            return;

        var searchDirectory = SearchDirectoryList.FirstOrDefault(x => x.Path.Equals(path));
        if (searchDirectory != null)
            return;

        searchDirectory = new SearchDirectoryInfo
        {
            Path = path,
            Priority = priority
        };
        SearchDirectoryList.Add(searchDirectory);

        Sort();
    }

    internal static void Remove(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        path = Path.GetFullPath(path);
        if (path.ContainsExtension())
            return;

        var searchDirectory = SearchDirectoryList.FirstOrDefault(x => x.Path.Equals(path));
        if (searchDirectory == null)
            return;

        SearchDirectoryList.Remove(searchDirectory);

        Sort();
    }

    internal static Assembly Scan(string requested_name)
    {
        var enumerator = new LemonEnumerator<SearchDirectoryInfo>(SearchDirectoryList);
        while (enumerator.MoveNext())
        {
            var folderpath = enumerator.Current.Path;
            if (folderpath.ContainsExtension()
                || !Directory.Exists(folderpath))
                continue;

            var filepath = Directory.GetFiles(folderpath).Where(x =>
                !string.IsNullOrEmpty(x)
                    && ((Path.GetExtension(x).ToLowerInvariant().Equals(".dll")
                        && Path.GetFileName(x).Equals($"{requested_name}.dll"))
                    || (Path.GetExtension(x).ToLowerInvariant().Equals(".exe")
                        && Path.GetFileName(x).Equals($"{requested_name}.exe")))
            ).FirstOrDefault();

            if (string.IsNullOrEmpty(filepath))
                continue;

            MelonDebug.Msg($"[MelonAssemblyResolver] Loading from {filepath}...");

#if NET6_0_OR_GREATER

            return AssemblyLoadContext.Default.LoadFromAssemblyPath(filepath);

#else
            var filePathPtr = Marshal.StringToHGlobalAnsi(filepath);
            if (filePathPtr == IntPtr.Zero)
                continue;

            IntPtr rootPtr = InternalUtils.BootstrapInterop.Library.MonoGetDomainPtr();
            if (rootPtr == IntPtr.Zero)
                continue;

            var assemblyPtr = MonoLibrary.Instance.mono_assembly_open_full(filePathPtr, IntPtr.Zero, false);
            if (assemblyPtr == IntPtr.Zero)
                continue;

            var assemblyReflectionPtr = MonoLibrary.Instance.mono_assembly_get_object(rootPtr, assemblyPtr);
            if (assemblyReflectionPtr == IntPtr.Zero)
                continue;

            return MonoLibrary.CastManagedAssemblyPtr(assemblyReflectionPtr);
#endif
        }

        MelonDebug.Msg($"[MelonAssemblyResolver] Failed to find {requested_name} in any of the known search directories");
        return null;
    }

    private class SearchDirectoryInfo
    {
        internal string Path = null;
        internal int Priority = 0;
    }
}
