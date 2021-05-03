using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using MonoMod.Utils;
using MonoMod.Cil;

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
                DynamicMethodDefinition method = new DynamicMethodDefinition(typeof(FileSystemWatcher).GetProperty("Path").GetGetMethod());
                ILContext ilcontext = new ILContext(method.Definition);
                ILCursor ilcursor = new ILCursor(ilcontext);
                if ((ilcursor.Instrs.Count == 2) 
                    && (ilcursor.Instrs[1].OpCode.Code == Mono.Cecil.Cil.Code.Throw))
                {
                    ilcontext.Dispose();
                    method.Dispose();
                    MelonLogger.Warning("FileSystemWatcher NotImplementedException Detected! Disabling MelonPreferences FileWatcher Functionality...");
                    ShouldDisableFileWatcherFunctionality = true;
                    return;
                }
                ilcontext.Dispose();
                method.Dispose();

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
