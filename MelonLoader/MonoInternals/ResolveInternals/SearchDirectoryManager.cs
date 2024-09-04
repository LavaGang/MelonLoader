using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader.MonoInternals.ResolveInternals
{
    internal static class SearchDirectoryManager
    {
        private static List<SearchDirectoryInfo> SearchDirectoryList = new List<SearchDirectoryInfo>();

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

            SearchDirectoryInfo searchDirectory = SearchDirectoryList.FirstOrDefault(x => x.Path.Equals(path));
            if (searchDirectory != null)
                return;

            searchDirectory = new SearchDirectoryInfo();
            searchDirectory.Path = path;
            searchDirectory.Priority = priority;
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

            SearchDirectoryInfo searchDirectory = SearchDirectoryList.FirstOrDefault(x => x.Path.Equals(path));
            if (searchDirectory == null)
                return;

            SearchDirectoryList.Remove(searchDirectory);

            Sort();
        }

        internal static Assembly Scan(string requested_name)
        {
            LemonEnumerator<SearchDirectoryInfo> enumerator = new LemonEnumerator<SearchDirectoryInfo>(SearchDirectoryList);
            while (enumerator.MoveNext())
            {
                string folderpath = enumerator.Current.Path;

                string filepath = Directory.GetFiles(folderpath).Where(x =>
                    !string.IsNullOrEmpty(x)
                    && ((Path.GetExtension(x).ToLowerInvariant().Equals(".dll")
                        && Path.GetFileName(x).Equals($"{requested_name}.dll"))
                    || (Path.GetExtension(x).ToLowerInvariant().Equals(".exe")
                        && Path.GetFileName(x).Equals($"{requested_name}.exe")))
                ).FirstOrDefault();

                if (string.IsNullOrEmpty(filepath))
                    continue;

                IntPtr filePathPtr = Marshal.StringToHGlobalAnsi(filepath);
                if (filePathPtr == IntPtr.Zero)
                    continue;

                IntPtr rootPtr = MonoLibrary.GetRootDomainPtr();
                if (rootPtr == IntPtr.Zero)
                    continue;

                IntPtr assemblyPtr = MonoLibrary.Instance.mono_assembly_open_full(filePathPtr, IntPtr.Zero, false);
                if (assemblyPtr == IntPtr.Zero)
                    continue;

                IntPtr assemblyReflectionPtr = MonoLibrary.Instance.mono_assembly_get_object(rootPtr, assemblyPtr);
                if (assemblyReflectionPtr == IntPtr.Zero)
                    continue;

                return MonoLibrary.CastManagedAssemblyPtr(assemblyReflectionPtr);
            }

            return null;
        }

        private class SearchDirectoryInfo
        {
            internal string Path = null;
            internal int Priority = 0;
        }
    }
}
