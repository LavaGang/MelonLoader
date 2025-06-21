using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace MelonLoader.Resolver;

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

    internal static Assembly Scan(string requestedName)
    {
        MelonDebug.Msg($"[MelonAssemblyResolver] Attempting to find {requestedName}");
        LemonEnumerator<SearchDirectoryInfo> enumerator = new LemonEnumerator<SearchDirectoryInfo>(SearchDirectoryList);
        while (enumerator.MoveNext())
        {
            string folderPath = enumerator.Current?.Path;
            if (folderPath.ContainsExtension() || !Directory.Exists(folderPath))
                continue;

            MelonDebug.Msg($"[MelonAssemblyResolver] Searching directory {folderPath}");
            string filepath = 
                Directory.GetFiles(folderPath)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x) &&
                                         ((Path.GetExtension(x).ToLowerInvariant().Equals(".dll")
                                           && Path.GetFileName(x).Equals($"{requestedName}.dll"))
                                          || (Path.GetExtension(x).ToLowerInvariant().Equals(".exe")
                                              && Path.GetFileName(x).Equals($"{requestedName}.exe"))));

            if (string.IsNullOrEmpty(filepath))
                continue;

            MelonDebug.Msg($"[MelonAssemblyResolver] Loading {requestedName} from {filepath}...");

#if NET6_0_OR_GREATER
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(filepath);
#else
            return Assembly.LoadFrom(filepath);
#endif
        }

        MelonDebug.Msg($"[MelonAssemblyResolver] Failed to find {requestedName} in any of the known search directories");
        return null;
    }

    private class SearchDirectoryInfo
    {
        internal string Path = null;
        internal int Priority = 0;
    }
}