using System;
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
            FileWatcher.Path = preffile.FilePath;
            FileWatcher.Filter = Path.GetFileName(preffile.FilePath);
            FileWatcher.Created += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.Changed += new FileSystemEventHandler(OnFileWatcherTriggered);
            FileWatcher.EnableRaisingEvents = true;
            FileWatcher.BeginInit();
        }

        internal void Destroy()
        {
            FileWatcher.EndInit();
            FileWatcher.Dispose();
        }

        private void OnFileWatcherTriggered(object source, FileSystemEventArgs e)
        {
            if (PrefFile.IsSaving)
            {
                PrefFile.IsSaving = false;
                return;
            }
            try
            {
                PrefFile.Load();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error while Loading Preferences from {PrefFile.FilePath}: {ex}");
                PrefFile.WasError = true;
            }
        }
    }
}
