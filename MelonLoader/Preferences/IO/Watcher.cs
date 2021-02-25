using System.IO;

namespace MelonLoader.Preferences.IO
{
    internal class Watcher
    {
        private FileSystemWatcher FileWatcher = null;
        private File PrefFile = null;

        internal Watcher(File preffile)
        {
            PrefFile = preffile;
            FileWatcher = new FileSystemWatcher();
            FileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            FileWatcher.Path = Path.GetDirectoryName(preffile.FilePath);
            FileWatcher.Filter = Path.GetFileName(preffile.FilePath);
            FileWatcher.Created += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.Changed += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.EnableRaisingEvents = true;
            FileWatcher.BeginInit();
        }

        internal void Destroy()
        {
            if (FileWatcher == null)
                return;
            FileWatcher.EndInit();
            FileWatcher.Dispose();
            FileWatcher = null;
        }

        private void OnFileWatcherTriggered(object source, FileSystemEventArgs e)
        {
            if (PrefFile.IsSaving)
            {
                PrefFile.IsSaving = false;
                return;
            }
            MelonPreferences.LoadFileAndRefreshCategories(PrefFile);
            MelonLogger.Msg($"MelonPreferences Reloaded from {PrefFile.FilePath}");
            MelonHandler.OnPreferencesLoaded();
        }
    }
}
