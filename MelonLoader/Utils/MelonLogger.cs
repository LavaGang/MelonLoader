using MelonLoader.Utils;
using System;
using System.Drawing;
using static MelonLoader.Utils.LoggerUtils;
using System.Text.RegularExpressions;
using MelonLoader.Bootstrap.Logging;
using MelonLoader.InternalUtils;

namespace MelonLoader
{
    public class MelonLogger
    {
        public static readonly Color DefaultMelonColor = Color.Cyan;
        public static readonly Color DefaultTextColor = Color.LightGray;

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
            if (namesection == null)
            {
                var melon = MelonUtils.GetMelonFromStackTrace();
                if (melon != null && melon.Info?.Name != null)
                {
                    namesection = melon.Info.Name;
                    namesection_color = melon.ConsoleColor;
                }
            }

            PassLogMsg(txt_color, txt ?? "null", namesection_color, namesection);
            RunMsgCallbacks(namesection_color, txt_color, namesection, txt ?? "null");
        }

        private static void NativePastelMsg(Color namesection_color, Color txt_color, string namesection, string txt, bool skipStackWalk = false)
        {
            if (namesection == null)
            {
                var melon = MelonUtils.GetMelonFromStackTrace();
                if (melon != null && melon.Info != null)
                {
                    namesection = melon.Info.Name;
                    namesection_color = melon.ConsoleColor;
                }
            }

            PastelMsg(namesection_color, txt_color, namesection, txt ?? "null");
            RunMsgCallbacks(namesection_color, txt_color, namesection, txt ?? "null");
        }

        private static void NativeWarning(string namesection, string txt)
        {
            namesection ??= MelonUtils.GetMelonFromStackTrace()?.Info?.Name;

            Warning(namesection, txt ?? "null");
            RunWarningCallbacks(namesection, txt ?? "null");
        }

        private static void NativeError(string namesection, string txt)
        {
            namesection ??= MelonUtils.GetMelonFromStackTrace()?.Info?.Name;

            PassLogError(txt ?? "null", namesection, false);
            RunErrorCallbacks(namesection, txt ?? "null");
        }

        public static void BigError(string namesection, string txt)
        {
            RunErrorCallbacks(namesection, txt ?? "null");

            PassLogError(new string('=', 50), namesection, false);
            foreach (var line in txt.Split('\n'))
                PassLogError(line, namesection, false);

            PassLogError(new string('=', 50), namesection, false);
        }

        internal static void RunMsgCallbacks(Color namesection_color, Color txt_color, string namesection, string txt)
        {
            MsgCallbackHandler?.Invoke(DrawingColorToConsoleColor(namesection_color), DrawingColorToConsoleColor(txt_color), namesection, txt);
            MsgDrawingCallbackHandler?.Invoke(namesection_color, txt_color, namesection, txt);
        }

        [Obsolete("MsgCallbackHandler is obsolete. Please use MsgDrawingCallbackHandler for full Color support. This will be removed in a future update.", true)]
        public static event Action<ConsoleColor, ConsoleColor, string, string> MsgCallbackHandler;

        public static event Action<Color, Color, string, string> MsgDrawingCallbackHandler;

        internal static void RunWarningCallbacks(string namesection, string txt) => WarningCallbackHandler?.Invoke(namesection, txt);

        public static event Action<string, string> WarningCallbackHandler;

        internal static void RunErrorCallbacks(string namesection, string txt) => ErrorCallbackHandler?.Invoke(namesection, txt);

        public static event Action<string, string> ErrorCallbackHandler;

        public class Instance
        {
            private string Name = null;

            [Obsolete("Color is obsolete. Please use DrawingColor for full Color support. This will be removed in a future update.", true)]
            private ConsoleColor Color
            {
                get => DrawingColorToConsoleColor(DrawingColor);
                set => DrawingColor = ConsoleColorToDrawingColor(value);
            }

            private Color DrawingColor = DefaultMelonColor;

            public Instance(string name) => Name = name?.Replace(" ", "_");

            [Obsolete("ConsoleColor is obsolete, use the (string, Color) constructor instead. This will be removed in a future update.", true)]
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

        internal static void PastelMsg(Color namesection_color, Color txt_color, string namesection, string txt)
        {
            // Regex to check for ANSI
            var cleanTxt = Regex.Replace(txt, @"(\x1B|\e|\033)\[(.*?)m", "");

            PassLogMsg(txt_color, cleanTxt, namesection_color, namesection);
        }

        internal static void Warning(string namesection, string txt)
        {
            PassLogError(txt, namesection, true);
        }

        internal static void ThrowInternalFailure(string txt) => Assertion.ThrowInternalFailure(txt);

        internal static unsafe void WriteSpacer()
        {
            BootstrapInterop.Library.LogMsg(null, null, 0, null, null, 0);
        }

        internal static void PrintModName(Color meloncolor, Color authorcolor, string name, string author, string additionalCredits, string version, string id)
        {
            PassLogMelonInfo(meloncolor, name, $"v{version}{(id == null ? "" : $" ({id})")}");
            PassLogMsg(authorcolor, $"by {author}", default, null);

            if (additionalCredits != null)
                PassLogMsg(DefaultTextColor, $"Additional credits: {additionalCredits}", default, null);
        }

        internal static unsafe void PassLogMsg(ColorRGB msgColor, string msg, ColorRGB sectionColor, string section)
        {
            char[] msgArray = msg.ToCharArray();
            if (section == null)
            {
                fixed (char* pMsg = msgArray)
                {
                    BootstrapInterop.Library.LogMsg(&msgColor, pMsg, msg.Length, null, null, 0);
                }

                return;
            }

            fixed (char* pMsg = msgArray)
            {
                char[] sectionArray = section.ToCharArray();
                fixed (char* pSection = sectionArray)
                {
                    BootstrapInterop.Library.LogMsg(&msgColor, pMsg, msg.Length, &sectionColor, pSection, section.Length);
                }
            }
        }

        internal static unsafe void PassLogError(string msg, string section, bool warning)
        {
            char[] msgArray = msg.ToCharArray();
            if (section == null)
            {
                fixed (char* pMsg = msgArray)
                {
                    BootstrapInterop.Library.LogError(pMsg, msg.Length, null, 0, warning);
                }

                return;
            }

            fixed (char* pMsg = msgArray)
            {
                char[] sectionArray = section.ToCharArray();
                fixed (char* pSection = sectionArray)
                {
                    BootstrapInterop.Library.LogError(pMsg, msg.Length, pSection, section.Length, warning);
                }
            }
        }

        internal static unsafe void PassLogMelonInfo(ColorRGB nameColor, string name, string info)
        {
            char[] nameArray = name.ToCharArray();
            fixed (char* pName = nameArray)
            {
                char[] infoArray = info.ToCharArray();
                fixed (char* pInfo = infoArray)
                {
                    BootstrapInterop.Library.LogMelonInfo(&nameColor, pName, name.Length, pInfo, info.Length);
                }
            }
        }

        [Obsolete("Log is obsolete. Please use Msg instead. This will be removed in a future update.", true)]
        public static void Log(string txt) => Msg(txt);

        [Obsolete("Log is obsolete. Please use Msg instead. This will be removed in a future update.", true)]
        public static void Log(string txt, params object[] args) => Msg(txt, args);

        [Obsolete("Log is obsolete. Please use Msg instead. This will be removed in a future update.", true)]
        public static void Log(object obj) => Msg(obj);

        [Obsolete("Log is obsolete. Please use Msg instead. This will be removed in a future update.", true)]
        public static void Log(ConsoleColor color, string txt) => Msg(color, txt);

        [Obsolete("Log is obsolete. Please use Msg instead. This will be removed in a future update.", true)]
        public static void Log(ConsoleColor color, string txt, params object[] args) => Msg(color, txt, args);

        [Obsolete("Log is obsolete. Please use Msg instead. This will be removed in a future update.", true)]
        public static void Log(ConsoleColor color, object obj) => Msg(color, obj);

        [Obsolete("LogWarning is obsolete. Please use Warning instead. This will be removed in a future update.", true)]
        public static void LogWarning(string txt) => Warning(txt);

        [Obsolete("LogWarning is obsolete. Please use Warning instead. This will be removed in a future update.", true)]
        public static void LogWarning(string txt, params object[] args) => Warning(txt, args);

        [Obsolete("LogError is obsolete. Please use Error instead. This will be removed in a future update.", true)]
        public static void LogError(string txt) => Error(txt);

        [Obsolete("LogError is obsolete. Please use Error instead. This will be removed in a future update.", true)]
        public static void LogError(string txt, params object[] args) => Error(txt, args);
    }
}