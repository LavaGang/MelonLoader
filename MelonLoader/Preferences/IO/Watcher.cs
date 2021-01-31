using System.IO;

namespace MelonLoader.Preferences.IO
{
    internal static class Watcher
    {
        private static FileSystemWatcher FileWatcher = null;

        internal static void Setup(string filepath)
        {
            FileWatcher = new FileSystemWatcher();
            FileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            FileWatcher.Path = Path.GetDirectoryName(filepath);
            FileWatcher.Filter = Path.GetFileName(filepath);
            FileWatcher.Created += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.Changed += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.EnableRaisingEvents = true;
            FileWatcher.BeginInit();
        }

        internal static void Destroy()
        {
            FileWatcher.EndInit();
            FileWatcher.Dispose();
        }

        private static void OnFileWatcherTriggered(object source, FileSystemEventArgs e)
        {
            if (File.WasError)
                return;
            if (!File.IsSaving)
            {
                MelonPreferences.Load();
                return;
            }
            File.IsSaving = false;
        }
    }
}
