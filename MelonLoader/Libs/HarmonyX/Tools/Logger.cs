using System;
using System.IO;
using System.Linq;
using System.Reflection.Emit;

namespace HarmonyLib.Tools
{
    /// <summary>
    /// Default Harmony logger that writes to a file
    /// </summary>
    public static class HarmonyFileLog
    {
        private static bool enabled;
        private static TextWriter textWriter;

        /// <summary>
        /// Whether or not to enable writing the log.
        /// </summary>
        public static bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                ToggleDebug();
            }
        }

        /// <summary>
        /// Text writer to write the logs to. If not set, defaults to a file log.
        /// </summary>
        public static TextWriter Writer
        {
            get => textWriter;
            set
            {
                textWriter?.Flush();
                textWriter = value;
            }
        }

        /// <summary>
        /// File path of the log.
        /// </summary>
        public static string FileWriterPath { get; set; } = "HarmonyLog.txt";

        private static void ToggleDebug()
        {
            if (Enabled)
            {
                if (Writer == null)
                    Writer = new StreamWriter(File.Create(Path.GetFullPath(FileWriterPath)));
                Logger.MessageReceived += OnMessage;
            }
            else
                Logger.MessageReceived -= OnMessage;
        }

        private static void OnMessage(object sender, Logger.LogEventArgs e)
        {
            Writer.WriteLine($"[{e.LogChannel}] {e.Message}");
            Writer.Flush();
        }

        public static string FormatArgument(object argument)
        {
            if (argument == null) return "NULL";
            var type = argument.GetType();

            if (type == typeof(string))
                return "\"" + argument + "\"";
            if (type == typeof(Label))
                return "Label" + ((Label)argument).GetHashCode();
            if (type == typeof(Label[]))
                return "Labels" + string.Join(",", ((Label[])argument).Select(l => l.GetHashCode().ToString()).ToArray());
            if (type == typeof(LocalBuilder))
                return ((LocalBuilder)argument).LocalIndex + " (" + ((LocalBuilder)argument).LocalType + ")";

            return argument.ToString().Trim();
        }
    }

    /// <summary>
    /// Main logger class that exposes log events.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// A single log event that represents a single log message.
        /// </summary>
        public class LogEventArgs : EventArgs
        {
            /// <summary>
            /// Log channel of the message.
            /// </summary>
            public LogChannel LogChannel { get; internal set; }

            /// <summary>
            /// The log message.
            /// </summary>
            public string Message { get; internal set; }
        }

        /// <summary>
        /// Log channel for the messages.
        /// </summary>
        [Flags]
        public enum LogChannel
        {
            /// <summary>
            /// No channels (or an empty channel).
            /// </summary>
            None = 0,

            /// <summary>
            /// Basic information.
            /// </summary>
            Info = 1 << 1,

            /// <summary>
            /// Full IL dumps of the generated dynamic methods.
            /// </summary>
            IL = 1 << 2,

            /// <summary>
            /// Channel for warnings.
            /// </summary>
            Warn = 1 << 3,

            /// <summary>
            /// Channel for errors.
            /// </summary>
            Error = 1 << 4,

            /// <summary>
            /// Additional debug information that is related to patching
            /// </summary>
            Debug = 1 << 5,

            /// <summary>
            /// All channels.
            /// </summary>
            All = Info | IL | Warn | Error | Debug
        }

        /// <summary>
        /// Filter for which channels should be listened to.
        /// If the channel is in the filter, all log messages from that channel get propagated into <see cref="MessageReceived"/> event.
        /// </summary>
        public static LogChannel ChannelFilter { get; set; } = LogChannel.None;

        /// <summary>
        /// Event fired on any incoming message that passes the channel filter.
        /// </summary>
        public static event EventHandler<LogEventArgs> MessageReceived;

        internal static void Log(LogChannel channel, Func<string> message)
        {
            if ((channel & ChannelFilter) != LogChannel.None)
                MessageReceived?.Invoke(null, new LogEventArgs { LogChannel = channel, Message = message() });
        }

        internal static void LogText(LogChannel channel, string message)
        {
            if ((channel & ChannelFilter) != LogChannel.None)
                MessageReceived?.Invoke(null, new LogEventArgs { LogChannel = channel, Message = message });
        }
    }
}
