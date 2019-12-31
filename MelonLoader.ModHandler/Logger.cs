using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MelonLoader
{
    public class Logger
    {
        internal static bool consoleEnabled = false;
        private static StreamWriter log;
        private static string fileprefix = "MelonLoader_";

        internal static void Initialize()
        {
            if (log == null)
            {
                string logFilePath = Path.Combine(Environment.CurrentDirectory, ("Logs/" + fileprefix + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".log"));
                FileInfo logFileInfo = new FileInfo(logFilePath);
                DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);

                if (!logDirInfo.Exists)
                    logDirInfo.Create();
                else
                    CleanOld(logDirInfo);

                FileStream fileStream = null;
                if (!logFileInfo.Exists)
                    fileStream = logFileInfo.Create();
                else
                    fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Write, FileShare.Read);

                log = new StreamWriter(fileStream);
                log.AutoFlush = true;
            }
        }

        public static void Stop() { if (log != null) log.Close(); }

        internal static void CleanOld(DirectoryInfo logDirInfo)
        {
            FileInfo[] filetbl = logDirInfo.GetFiles(fileprefix + "*");
            if (filetbl.Length > 0)
            {
                List<FileInfo> filelist = filetbl.ToList().OrderBy(x => x.LastWriteTime).ToList();
                for (int i = (filelist.Count - 10); i > -1; i--)
                {
                    FileInfo file = filelist[i];
                    file.Delete();
                }
            }
        }

        internal static string GetTimestamp() { return DateTime.Now.ToString("HH:mm:ss.fff"); }

        public static void Log(string s)
        {
            var timestamp = GetTimestamp();
            if (consoleEnabled) Console.WriteLine("[" + timestamp + "] [MelonLoader] " + s);
            if (log != null) log.WriteLine("[" + timestamp + "] " + s);
        }

        public static void Log(string s, params object[] args)
        {
            var timestamp = GetTimestamp();
            var formatted = string.Format(s, args);
            if (consoleEnabled) Console.WriteLine("[" + timestamp + "] [MelonLoader] " + formatted);
            if (log != null) log.WriteLine("[" + timestamp + "] " + formatted);
        }

        public static void LogError(string s)
        {
            var timestamp = GetTimestamp();
            if (consoleEnabled) Console.WriteLine("[" + timestamp + "] [MelonLoader] [Error] " + s);
            if (log != null) log.WriteLine("[" + timestamp + "] [Error] " + s);
        }

        public static void LogError(string s, params object[] args)
        {
            var timestamp = GetTimestamp();
            var formatted = string.Format(s, args);
            if (consoleEnabled) Console.WriteLine("[" + timestamp + "] [MelonLoader] [Error] " + formatted);
            if (log != null) log.WriteLine("[" + timestamp + "] [Error] " + formatted);
        }
    }
}