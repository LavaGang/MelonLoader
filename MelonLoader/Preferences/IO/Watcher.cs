using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using MelonLoader.Utils;

namespace MelonLoader.Preferences.IO
{
    internal class Watcher
    {
        private static bool ShouldDisableFileWatcherFunctionality;
        private FileSystemWatcher FileWatcher;
        private readonly File PrefFile;

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
                FileWatcher.Created += OnFileWatcherTriggered;
                FileWatcher.Changed += OnFileWatcherTriggered;
                FileWatcher.BeginInit();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("FileSystemWatcher Exception: " + ex);
                ShouldDisableFileWatcherFunctionality = true;
                FileWatcher = null;
            }
        }

        internal void Destroy()
        {
            if (ShouldDisableFileWatcherFunctionality || FileWatcher == null)
                return;
            try
            {
                FileWatcher.EndInit();
                FileWatcher.Dispose();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("FileSystemWatcher Exception: " + ex);
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
