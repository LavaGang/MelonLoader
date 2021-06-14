using System;
using System.IO;
using System.Reflection;
using HarmonyLib;

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
                MethodInfo method = AccessTools.PropertyGetter(typeof(FileSystemWatcher), "Path");
                if (method == null)
                    throw new NullReferenceException("No Path Property Get Method Found!");
                if (method.IsNotImplemented())
                {
                    MelonLogger.Warning("FileSystemWatcher NotImplementedException Detected! Disabling MelonPreferences FileWatcher Functionality...");
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
