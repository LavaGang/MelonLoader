﻿using MelonLoader.Utils;
using System;
using System.IO;

using static MelonLoader.Utils.LoggerUtils;

namespace MelonLoader
{
    public class MelonLogger
    {
        public static readonly ConsoleColor DefaultMelonColor = ConsoleColor.Cyan;
        public static readonly ConsoleColor DefaultTextColor = ConsoleColor.Gray;

        private static FileStream LogStream = File.Open(Path.Combine(MelonEnvironment.MelonLoaderDirectory, "Latest-Managed.log"), FileMode.Create);
        private static StreamWriter LogWriter = new StreamWriter(LogStream);

        public static void Msg(object obj) => NativeMsg(DefaultMelonColor, DefaultTextColor, null, obj.ToString());
        public static void Msg(string txt) => NativeMsg(DefaultMelonColor, DefaultTextColor, null, txt);
        public static void Msg(string txt, params object[] args) => NativeMsg(DefaultMelonColor, DefaultTextColor, null, string.Format(txt, args));

        public static void Msg(ConsoleColor txt_color, object obj) => NativeMsg(DefaultMelonColor, txt_color, null, obj.ToString());
        public static void Msg(ConsoleColor txt_color, string txt) => NativeMsg(DefaultMelonColor, txt_color, null, txt);
        public static void Msg(ConsoleColor txt_color, string txt, params object[] args) => NativeMsg(DefaultMelonColor, txt_color, null, string.Format(txt, args));


        public static void Warning(object obj) => NativeWarning(null, obj.ToString());
        public static void Warning(string txt) => NativeWarning(null, txt);
        public static void Warning(string txt, params object[] args) => NativeWarning(null, string.Format(txt, args));


        public static void Error(object obj) => NativeError(null, obj.ToString());
        public static void Error(string txt) => NativeError(null, txt);
        public static void Error(string txt, params object[] args) => NativeError(null, string.Format(txt, args));
        public static void Error(string txt, Exception ex) => NativeError(null, $"{txt}\n{ex}");

        private static void NativeMsg(ConsoleColor namesection_color, ConsoleColor txt_color, string namesection, string txt)
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

        internal static void RunMsgCallbacks(ConsoleColor namesection_color, ConsoleColor txt_color, string namesection, string txt) => MsgCallbackHandler?.Invoke(namesection_color, txt_color, namesection, txt);
        public static event Action<ConsoleColor, ConsoleColor, string, string> MsgCallbackHandler;
        internal static void RunWarningCallbacks(string namesection, string txt) => WarningCallbackHandler?.Invoke(namesection, txt);
        public static event Action<string, string> WarningCallbackHandler;
        internal static void RunErrorCallbacks(string namesection, string txt) => ErrorCallbackHandler?.Invoke(namesection, txt);
        public static event Action<string, string> ErrorCallbackHandler;

        public class Instance
        {
            private string Name = null;
            private ConsoleColor Color = DefaultMelonColor;
            public Instance(string name) => Name = name?.Replace(" ", "_");
            public Instance(string name, ConsoleColor color) : this(name) => Color = color;

            public void Msg(object obj) => NativeMsg(Color, DefaultTextColor, Name, obj.ToString());
            public void Msg(string txt) => NativeMsg(Color, DefaultTextColor, Name, txt);
            public void Msg(string txt, params object[] args) => NativeMsg(Color, DefaultTextColor, Name, string.Format(txt, args));

            public void Msg(ConsoleColor txt_color, object obj) => NativeMsg(Color, txt_color, Name, obj.ToString());
            public void Msg(ConsoleColor txt_color, string txt) => NativeMsg(Color, txt_color, Name, txt);
            public void Msg(ConsoleColor txt_color, string txt, params object[] args) => NativeMsg(Color, txt_color, Name, string.Format(txt, args));

            public void Warning(object obj) => NativeWarning(Name, obj.ToString());
            public void Warning(string txt) => NativeWarning(Name, txt);
            public void Warning(string txt, params object[] args) => NativeWarning(Name, string.Format(txt, args));

            public void Error(object obj) => NativeError(Name, obj.ToString());
            public void Error(string txt) => NativeError(Name, txt);
            public void Error(string txt, params object[] args) => NativeError(Name, string.Format(txt, args));
            public void Error(string txt, Exception ex) => NativeError(Name, $"{txt}\n{ex}");
        }

        internal static void Internal_Msg(ConsoleColor namesection_color, ConsoleColor txt_color, string namesection, string txt)
        {
            LogWriter.WriteLine($"[{GetTimeStamp()}] {(namesection is null ? "" : $"[{namesection}] ")}{txt}");

            WriteTimestamp(namesection_color == ConsoleColor.Red && txt_color == ConsoleColor.Red);

            if (namesection is not null)
            {
                Console.Write($"{ColorToAnsi(ConsoleColor.Gray)}[");
                Console.Write($"{ColorToAnsi(namesection_color)}{namesection}");
                Console.Write($"{ColorToAnsi(ConsoleColor.Gray)}] ");
            }

            Console.Write($"{ColorToAnsi(txt_color)}{txt}\n");
        }

        internal static void WriteTimestamp(bool error)
        {
            if (error)
            {
                Console.Write($"{ColorToAnsi(ConsoleColor.Red)}[");
                Console.Write($"{GetTimeStamp()}");
                Console.Write($"] ");
                return;
            }

            Console.Write($"{ColorToAnsi(ConsoleColor.Gray)}[");
            Console.Write($"{ColorToAnsi(ConsoleColor.Green)}{GetTimeStamp()}");
            Console.Write($"{ColorToAnsi(ConsoleColor.Gray)}] ");
        }

        internal static void Internal_Warning(string namesection, string txt) => Internal_Msg(ConsoleColor.Yellow, ConsoleColor.Yellow, namesection, txt);


        internal static void Internal_Error(string namesection, string txt) => Internal_Msg(ConsoleColor.Red, ConsoleColor.Red, namesection, txt);


        internal static void ThrowInternalFailure(string txt) => Internal_Error("TODO: Internal Failure Impl", txt);


        internal static void WriteSpacer()
        {
            LogWriter.WriteLine();
            Console.WriteLine();
        }

        internal static void Internal_PrintModName(ConsoleColor meloncolor, ConsoleColor authorcolor, string name, string author, string version, string id)
        {
            LogWriter.WriteLine($"[{GetTimeStamp()}] {name} v{version}{(id == null ? "" : $" ({id})")}");
            LogWriter.WriteLine($"[{GetTimeStamp()}] by {author}");

            //TODO Colors

            WriteTimestamp(false);
            Console.WriteLine($"{ColorToAnsi(meloncolor)}{name} v{version}{(id == null ? "" : $" ({id})")}");
            WriteTimestamp(false);
            Console.WriteLine($"{ColorToAnsi(authorcolor)}by {author}");
        }


        internal static void Flush()
        {
            LogWriter.Flush();
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
