using System;
using System.IO;
using System.Reflection;

namespace MelonLoader.Preferences.IO
{
    internal class Watcher
    {
        private static bool ShouldDisableFileWatcherFunctionality = false;
        private FileSystemWatcher FileWatcher = null;
        private readonly File PrefFile = null;

        internal Watcher(File preffile)
        {
            PrefFile = preffile;
            if (ShouldDisableFileWatcherFunctionality)
                return;
            try
            {
                MethodBody bod = typeof(FileSystemWatcher).GetMethod("Dispose").GetMethodBody();
                byte[] ilarr = bod.GetILAsByteArray();
                int ilarrsize = ilarr.Length - 1;
                if ((ilarr[ilarrsize] == 42)
                    && (ilarr[ilarrsize - 1] == 10)
                    && (ilarr[ilarrsize - 2] == 0)
                    && (ilarr[ilarrsize - 3] == 0))
                {
                    MelonLogger.Warning("FileSystemWatcher Exception: NotImplementedException Detected!");
                    ShouldDisableFileWatcherFunctionality = true;
                    return;
                }

                FileWatcher = new FileSystemWatcher(Path.GetDirectoryName(preffile.FilePath), Path.GetFileName(preffile.FilePath))
                {
                    NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                    EnableRaisingEvents = true
                };
                FileWatcher.Created += new FileSystemEventHandler(OnFileWatcherTriggered);
                FileWatcher.Changed += new FileSystemEventHandler(OnFileWatcherTriggered);
                FileWatcher.BeginInit();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("FileSystemWatcher Exception: " + ex.ToString());
                ShouldDisableFileWatcherFunctionality = true;
                FileWatcher = null;
            }
        }

        internal void Destroy()
        {
            if (ShouldDisableFileWatcherFunctionality || (FileWatcher == null))
                return;
            try
            {
                FileWatcher.EndInit();
                FileWatcher.Dispose();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("FileSystemWatcher Exception: " + ex.ToString());
                ShouldDisableFileWatcherFunctionality = true;
            }
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
        }
    }
}
