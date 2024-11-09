using MelonLoader.Utils;
using MelonLoader.Pastel;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using static MelonLoader.Utils.LoggerUtils;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MelonLoader
{
    public class MelonLogger
    {
        private static int MaxLogs;

        public static readonly Color DefaultMelonColor = Color.Cyan;
        public static readonly Color DefaultTextColor = Color.LightGray;

        private static FileStream LatestLogStream;
        private static StreamWriter LatestLogWriter;

        private static FileStream CachedLogStream;
        private static StreamWriter CachedLogWriter;

        internal static void Setup()
        {
            if (MelonLaunchOptions.Core.IsDebug)
                MaxLogs = 0;
            else
                MaxLogs = MelonLaunchOptions.Logger.MaxLogs;

            if (!Directory.Exists(MelonEnvironment.MelonLoaderLogsDirectory))
                Directory.CreateDirectory(MelonEnvironment.MelonLoaderLogsDirectory);
            else
            {
                if (MaxLogs > 0)
                {
                    string[] fileTbl = Directory.GetFiles(MelonEnvironment.MelonLoaderLogsDirectory, "*.log", SearchOption.TopDirectoryOnly);
                    int fileCount = fileTbl.Length;
                    if (fileCount >= MaxLogs)
                    {
                        List<(string, DateTime)> queue = new();
                        foreach (var file in fileTbl)
                            queue.Add((file, File.GetLastWriteTime(file)));
                        queue.Sort((x, y) =>
                        {
                            if (x.Item2 >= y.Item2)
                                return 0;
                            return 1;
                        });
                        foreach (var pair in queue)
                        {
                            if (fileCount < MaxLogs)
                                break;
                            else
                            {
                                // This is cursed but better than crashing
                                try
                                {
                                    File.Delete(pair.Item1);
                                }
                                catch { }
                                fileCount--;
                            }
                        }
                    }
                }
            }

#if !NET6_0
            LatestLogStream = File.Open(Path.Combine(MelonEnvironment.MelonLoaderDirectory, "Latest.log"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            CachedLogStream = File.Open(Path.Combine(MelonEnvironment.MelonLoaderLogsDirectory, $"{DateTime.Now.ToString("%y-%M-%d_%H-%m-%s")}.log"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
#else
            LatestLogStream = File.Open(Path.Combine(MelonEnvironment.MelonLoaderDirectory, "Latest.log"), new FileStreamOptions() { Access = FileAccess.ReadWrite, BufferSize = 0, Mode = FileMode.Create, Share = FileShare.ReadWrite });
            CachedLogStream = File.Open(Path.Combine(MelonEnvironment.MelonLoaderLogsDirectory, $"{DateTime.Now.ToString("%y-%M-%d_%H-%m-%s")}.log"), new FileStreamOptions() { Access = FileAccess.ReadWrite, BufferSize = 0, Mode = FileMode.Create, Share = FileShare.ReadWrite });
#endif

            LatestLogWriter = CreateLogWriter(LatestLogStream);
            CachedLogWriter = CreateLogWriter(CachedLogStream);
        }

        private static StreamWriter CreateLogWriter(FileStream stream)
        {
            var writer = new StreamWriter(stream, Encoding.UTF8, 1);
            writer.AutoFlush = true;
            return writer;
        }

        internal static void WriteLogToFile()
        {
            LatestLogWriter.WriteLine();
            CachedLogWriter.WriteLine();
        }

        internal static void WriteLogToFile(string message)
        {
            LatestLogWriter.WriteLine(message);
            CachedLogWriter.WriteLine(message);
        }

        //Identical to Msg(string) except it skips walking the stack to find a melon
        internal static void MsgDirect(string txt) => NativeMsg(DefaultMelonColor, DefaultTextColor, null, txt, true);

        public static void Msg(object obj) => NativeMsg(DefaultMelonColor, DefaultTextColor, null, obj.ToString());

        public static void Msg(string txt) => NativeMsg(DefaultMelonColor, DefaultTextColor, null, txt);

        public static void Msg(string txt, params object[] args) => NativeMsg(DefaultMelonColor, DefaultTextColor, null, string.Format(txt, args));

        public static void Msg(ConsoleColor txt_color, object obj) => NativeMsg(DefaultMelonColor, ConsoleColorToDrawingColor(txt_color), null, obj.ToString());

        public static void Msg(ConsoleColor txt_color, string txt) => NativeMsg(DefaultMelonColor, ConsoleColorToDrawingColor(txt_color), null, txt);

        public static void Msg(ConsoleColor txt_color, string txt, params object[] args) => NativeMsg(DefaultMelonColor, ConsoleColorToDrawingColor(txt_color), null, string.Format(txt, args));

        //Identical to Msg(Color, string) except it skips walking the stack to find a melon
        public static void MsgDirect(Color txt_color, string txt) => NativeMsg(DefaultMelonColor, txt_color, null, txt, true);

        public static void Msg(Color txt_color, object obj) => NativeMsg(DefaultMelonColor, txt_color, null, obj.ToString());

        public static void Msg(Color txt_color, string txt) => NativeMsg(DefaultMelonColor, txt_color, null, txt);

        public static void Msg(Color txt_color, string txt, params object[] args) => NativeMsg(DefaultMelonColor, txt_color, null, string.Format(txt, args));

        //Identical to MsgPastel(string) except it skips walking the stack to find a melon
        internal static void MsgPastelDirect(string txt) => NativePastelMsg(DefaultMelonColor, DefaultTextColor, null, txt, true);

        public static void MsgPastel(object obj) => NativePastelMsg(DefaultMelonColor, DefaultTextColor, null, obj.ToString());

        public static void MsgPastel(string txt) => NativePastelMsg(DefaultMelonColor, DefaultTextColor, null, txt);

        public static void MsgPastel(string txt, params object[] args) => NativePastelMsg(DefaultMelonColor, DefaultTextColor, null, string.Format(txt, args));

        public static void MsgPastel(ConsoleColor txt_color, object obj) => NativePastelMsg(DefaultMelonColor, ConsoleColorToDrawingColor(txt_color), null, obj.ToString());

        public static void MsgPastel(ConsoleColor txt_color, string txt) => NativePastelMsg(DefaultMelonColor, ConsoleColorToDrawingColor(txt_color), null, txt);

        public static void MsgPastel(ConsoleColor txt_color, string txt, params object[] args) => NativePastelMsg(DefaultMelonColor, ConsoleColorToDrawingColor(txt_color), null, string.Format(txt, args));

        //Identical to MsgPastel(Color, string) except it skips walking the stack to find a melon
        public static void MsgPastelDirect(Color txt_color, string txt) => NativePastelMsg(DefaultMelonColor, txt_color, null, txt, true);

        public static void MsgPastel(Color txt_color, object obj) => NativePastelMsg(DefaultMelonColor, txt_color, null, obj.ToString());

        public static void MsgPastel(Color txt_color, string txt) => NativePastelMsg(DefaultMelonColor, txt_color, null, txt);

        public static void MsgPastel(Color txt_color, string txt, params object[] args) => NativePastelMsg(DefaultMelonColor, txt_color, null, string.Format(txt, args));

        public static void Warning(object obj) => NativeWarning(null, obj.ToString());

        public static void Warning(string txt) => NativeWarning(null, txt);

        public static void Warning(string txt, params object[] args) => NativeWarning(null, string.Format(txt, args));

        public static void Error(object obj) => NativeError(null, obj.ToString());

        public static void Error(string txt) => NativeError(null, txt);

        public static void Error(string txt, params object[] args) => NativeError(null, string.Format(txt, args));

        public static void Error(string txt, Exception ex) => NativeError(null, $"{txt}\n{ex}");

        public static void WriteLine(int length = 30) => MsgDirect(new string('-', length));

        public static void WriteLine(Color color, int length = 30) => MsgDirect(color, new string('-', length));

        private static void NativeMsg(Color namesection_color, Color txt_color, string namesection, string txt, bool skipStackWalk = false)
        {
            if (string.IsNullOrEmpty(namesection))
            {
                MelonBase melon = MelonUtils.GetMelonFromStackTrace();
                if (melon != null)
                {
                    namesection = melon.Info?.Name?.Replace(" ", "_");
                    namesection_color = melon.ConsoleColor;
                }
            }

            Internal_Msg(namesection_color, txt_color, namesection, txt ?? "null");
            RunMsgCallbacks(namesection_color, txt_color, namesection, txt ?? "null");
        }

        private static void NativePastelMsg(Color namesection_color, Color txt_color, string namesection, string txt, bool skipStackWalk = false)
        {
            if (string.IsNullOrEmpty(namesection))
            {
                MelonBase melon = MelonUtils.GetMelonFromStackTrace();
                if (melon != null)
                {
                    namesection = melon.Info?.Name?.Replace(" ", "_");
                    namesection_color = melon.ConsoleColor;
                }
            }

            Internal_PastelMsg(namesection_color, txt_color, namesection, txt ?? "null");
            RunMsgCallbacks(namesection_color, txt_color, namesection, txt ?? "null");
        }

        private static void NativeWarning(string namesection, string txt)
        {
            namesection ??= MelonUtils.GetMelonFromStackTrace()?.Info?.Name?.Replace(" ", "_");

            Internal_Warning(namesection, txt ?? "null");
            RunWarningCallbacks(namesection, txt ?? "null");
        }

        private static void NativeError(string namesection, string txt)
        {
            namesection ??= MelonUtils.GetMelonFromStackTrace()?.Info?.Name?.Replace(" ", "_");

            Internal_Error(namesection, txt ?? "null");
            RunErrorCallbacks(namesection, txt ?? "null");
        }

        public static void BigError(string namesection, string txt)
        {
            RunErrorCallbacks(namesection, txt ?? "null");

            Internal_Error(namesection, new string('=', 50));
            foreach (var line in txt.Split('\n'))
                Internal_Error(namesection, line);
            Internal_Error(namesection, new string('=', 50));
        }

        internal static void RunMsgCallbacks(Color namesection_color, Color txt_color, string namesection, string txt)
        {
            MsgCallbackHandler?.Invoke(DrawingColorToConsoleColor(namesection_color), DrawingColorToConsoleColor(txt_color), namesection, txt);
            MsgDrawingCallbackHandler?.Invoke(namesection_color, txt_color, namesection, txt);
        }

        [Obsolete("MsgCallbackHandler is obsolete. Please use MsgDrawingCallbackHandler for full Color support.")]
        public static event Action<ConsoleColor, ConsoleColor, string, string> MsgCallbackHandler;

        public static event Action<Color, Color, string, string> MsgDrawingCallbackHandler;

        internal static void RunWarningCallbacks(string namesection, string txt) => WarningCallbackHandler?.Invoke(namesection, txt);

        public static event Action<string, string> WarningCallbackHandler;

        internal static void RunErrorCallbacks(string namesection, string txt) => ErrorCallbackHandler?.Invoke(namesection, txt);

        public static event Action<string, string> ErrorCallbackHandler;

        public class Instance
        {
            private string Name = null;

            [Obsolete("Color is obsolete. Please use DrawingColor for full Color support.")]
            private ConsoleColor Color
            {
                get => DrawingColorToConsoleColor(DrawingColor);
                set => DrawingColor = ConsoleColorToDrawingColor(value);
            }

            private Color DrawingColor = DefaultMelonColor;

            public Instance(string name) => Name = name?.Replace(" ", "_");

            [Obsolete("ConsoleColor is obsolete, use the (string, Color) constructor instead.")]
            public Instance(string name, ConsoleColor color) : this(name) => Color = color;

            public Instance(string name, Color color) : this(name) => DrawingColor = color;

            public void Msg(object obj) => NativeMsg(DrawingColor, DefaultTextColor, Name, obj.ToString());

            public void Msg(string txt) => NativeMsg(DrawingColor, DefaultTextColor, Name, txt);

            public void Msg(string txt, params object[] args) => NativeMsg(DrawingColor, DefaultTextColor, Name, string.Format(txt, args));

            public void Msg(ConsoleColor txt_color, object obj) => NativeMsg(DrawingColor, ConsoleColorToDrawingColor(txt_color), Name, obj.ToString());

            public void Msg(ConsoleColor txt_color, string txt) => NativeMsg(DrawingColor, ConsoleColorToDrawingColor(txt_color), Name, txt);

            public void Msg(ConsoleColor txt_color, string txt, params object[] args) => NativeMsg(DrawingColor, ConsoleColorToDrawingColor(txt_color), Name, string.Format(txt, args));

            public void Msg(Color txt_color, object obj) => NativeMsg(DrawingColor, txt_color, Name, obj.ToString());

            public void Msg(Color txt_color, string txt) => NativeMsg(DrawingColor, txt_color, Name, txt);

            public void Msg(Color txt_color, string txt, params object[] args) => NativeMsg(DrawingColor, txt_color, Name, string.Format(txt, args));

            public void MsgPastel(object obj) => NativePastelMsg(DrawingColor, DefaultTextColor, Name, obj.ToString());

            public void MsgPastel(string txt) => NativePastelMsg(DrawingColor, DefaultTextColor, Name, txt);

            public void MsgPastel(string txt, params object[] args) => NativePastelMsg(DrawingColor, DefaultTextColor, Name, string.Format(txt, args));

            public void MsgPastel(ConsoleColor txt_color, object obj) => NativePastelMsg(DrawingColor, ConsoleColorToDrawingColor(txt_color), Name, obj.ToString());

            public void MsgPastel(ConsoleColor txt_color, string txt) => NativePastelMsg(DrawingColor, ConsoleColorToDrawingColor(txt_color), Name, txt);

            public void MsgPastel(ConsoleColor txt_color, string txt, params object[] args) => NativePastelMsg(DrawingColor, ConsoleColorToDrawingColor(txt_color), Name, string.Format(txt, args));

            public void MsgPastel(Color txt_color, object obj) => NativePastelMsg(DrawingColor, txt_color, Name, obj.ToString());

            public void MsgPastel(Color txt_color, string txt) => NativePastelMsg(DrawingColor, txt_color, Name, txt);

            public void MsgPastel(Color txt_color, string txt, params object[] args) => NativePastelMsg(DrawingColor, txt_color, Name, string.Format(txt, args));

            public void Warning(object obj) => NativeWarning(Name, obj.ToString());

            public void Warning(string txt) => NativeWarning(Name, txt);

            public void Warning(string txt, params object[] args) => NativeWarning(Name, string.Format(txt, args));

            public void Error(object obj) => NativeError(Name, obj.ToString());

            public void Error(string txt) => NativeError(Name, txt);

            public void Error(string txt, params object[] args) => NativeError(Name, string.Format(txt, args));

            public void Error(string txt, Exception ex) => NativeError(Name, $"{txt}\n{ex}");

            public void WriteSpacer() => MelonLogger.WriteSpacer();

            public void WriteLine(int length = 30) => MelonLogger.WriteLine(length);

            public void WriteLine(Color color, int length = 30) => MelonLogger.WriteLine(color, length);

            public void BigError(string txt) => MelonLogger.BigError(Name, txt);
        }

        internal static void Internal_Msg(Color namesection_color, Color txt_color, string namesection, string txt)
        {
            WriteLogToFile($"[{GetTimeStamp()}] {(namesection is null ? "" : $"[{namesection}] ")}{txt}");

            StringBuilder builder = new StringBuilder();

            builder.Append(GetTimestamp(namesection_color == Color.IndianRed && txt_color == Color.IndianRed));

            if (namesection is not null)
            {
                builder.Append("[".Pastel(Color.LightGray));
                builder.Append(namesection.Pastel(namesection_color));
                builder.Append("] ".Pastel(Color.LightGray));
            }

            builder.Append(txt.Pastel(txt_color));
            Utils.MelonConsole.WriteLine(builder.ToString());
        }

        internal static void Internal_PastelMsg(Color namesection_color, Color txt_color, string namesection, string txt)
        {
            // Regex to check for ANSI
            string fileTxt = Regex.Replace(txt, @"(\x1B|\e|\033)\[(.*?)m", "");

            WriteLogToFile($"[{GetTimeStamp()}] {(namesection is null ? "" : $"[{namesection}] ")}{fileTxt}");

            StringBuilder builder = new StringBuilder();

            builder.Append(GetTimestamp(namesection_color == Color.IndianRed && txt_color == Color.IndianRed));

            if (namesection is not null)
            {
                builder.Append("[".Pastel(Color.LightGray));
                builder.Append(namesection.Pastel(namesection_color));
                builder.Append("] ".Pastel(Color.LightGray));
            }

            builder.Append(txt.Pastel(txt_color));
            Utils.MelonConsole.WriteLine(builder.ToString());
        }

        internal static string GetTimestamp(bool error)
        {
            StringBuilder builder = new StringBuilder();
            if (error)
            {
                builder.Append("[".Pastel(Color.IndianRed));
                builder.Append(GetTimeStamp().Pastel(Color.IndianRed));
                builder.Append("] ".Pastel(Color.IndianRed));
                return builder.ToString();
            }

            builder.Append("[".Pastel(Color.LightGray));
            builder.Append(GetTimeStamp().Pastel(Color.LimeGreen));
            builder.Append("] ".Pastel(Color.LightGray));

            return builder.ToString();
        }

        internal static void Internal_Warning(string namesection, string txt)
        {
            if (MelonLaunchOptions.Console.HideWarnings)
                return;

            Internal_Msg(Color.Yellow, Color.Yellow, namesection, txt);
        }

        internal static void Internal_Error(string namesection, string txt) => Internal_Msg(Color.IndianRed, Color.IndianRed, namesection, txt);

        internal static void ThrowInternalFailure(string txt) => Assertion.ThrowInternalFailure(txt);

        internal static void WriteSpacer()
        {
            WriteLogToFile();
            Utils.MelonConsole.WriteLine();
        }

        internal static void Internal_PrintModName(Color meloncolor, Color authorcolor, string name, string author, string additionalCredits, string version, string id)
        {
            WriteLogToFile($"[{GetTimeStamp()}] {name} v{version}{(id == null ? "" : $" ({id})")}");
            WriteLogToFile($"[{GetTimeStamp()}] by {author}");

            StringBuilder builder = new StringBuilder();
            builder.Append(GetTimestamp(false));
            builder.Append(name.Pastel(meloncolor));
            builder.AppendLine($" v{version} {(id == null ? "" : $"({id})")}");

            builder.Append(GetTimestamp(false));
            builder.Append($"by {author}".Pastel(authorcolor));

            if (additionalCredits is not null)
            {
                builder.AppendLine();
                builder.Append(GetTimestamp(false));
                builder.Append($"Additional credits: {additionalCredits}");
            }

            Utils.MelonConsole.WriteLine(builder.ToString());
        }

        internal static void Flush()
        {
            LatestLogWriter.Flush();
            LatestLogStream.Flush();
        }

        internal static void Close()
        {
            LatestLogWriter.Close();
            CachedLogWriter.Close();
        }

        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(string txt) => Msg(txt);

        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(string txt, params object[] args) => Msg(txt, args);

        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(object obj) => Msg(obj);

        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, string txt) => Msg(color, txt);

        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, string txt, params object[] args) => Msg(color, txt, args);

        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, object obj) => Msg(color, obj);

        [Obsolete("LogWarning is obsolete. Please use Warning instead.")]
        public static void LogWarning(string txt) => Warning(txt);

        [Obsolete("LogWarning is obsolete. Please use Warning instead.")]
        public static void LogWarning(string txt, params object[] args) => Warning(txt, args);

        [Obsolete("LogError is obsolete. Please use Error instead.")]
        public static void LogError(string txt) => Error(txt);

        [Obsolete("LogError is obsolete. Please use Error instead.")]
        public static void LogError(string txt, params object[] args) => Error(txt, args);
    }
}