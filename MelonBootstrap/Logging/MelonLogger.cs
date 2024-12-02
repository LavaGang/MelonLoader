using MelonLoader.Bootstrap.Utils;
using Pastel;
using System.Drawing;

namespace MelonLoader.Bootstrap.Logging;

internal static class MelonLogger
{
    private static readonly string timeFormat = "HH:mm:ss.fff";
    private static readonly Color timeColor = Color.LimeGreen;
    private static readonly ConsoleColor legacyTimeColor = ConsoleColor.Green;

    private static readonly List<StreamWriter> logFiles = [];

    public static void Init()
    {
        var version = typeof(Core).Assembly.GetName().Version!;
        var versionStr = version.ToString(3);
        if (version.Revision != 0)
            versionStr += "-ci." + version.Revision.ToString();

        var onTop = ArgParser.IsDefined("melonloader.consoleontop");
        string? title = null;
        if (!ArgParser.IsDefined("melonloader.consoledst"))
        {
            title = "MelonLoader v" + versionStr;
            if (Core.Debug)
                title = "[D] " + title;
        }

        ConsoleHandler.OpenConsole(onTop, title);
        // Making logs from this point is ok, but only for console

        uint maxLogs;
        if (Core.Debug)
        {
            maxLogs = 0;
        }
        else
        {
            if (!uint.TryParse(ArgParser.GetValue("melonloader.maxlogs"), out maxLogs))
                maxLogs = 10;
        }

        MelonDebug.Log($"Creating log files (Max logs: {maxLogs})");

        var logsDir = Path.Combine(Core.BaseDir, "MelonLoader", "Logs");
        Directory.CreateDirectory(logsDir);

        if (maxLogs > 0)
        {
            var logs = Directory.GetFiles(logsDir, "*.log", SearchOption.TopDirectoryOnly);
            if (logs.Length >= maxLogs)
            {
                var queue = new List<(string, DateTime)>();

                foreach (var file in logs)
                    queue.Add((file, File.GetLastWriteTime(file)));

                queue.Sort((x, y) =>
                {
                    if (x.Item2 >= y.Item2)
                        return 0;
                    return 1;
                });

                var toDelete = logs.Length - maxLogs + 1;
                for (var i = 0; i < toDelete; i++)
                {
                    var file = queue[i];

                    try
                    {
                        File.Delete(file.Item1);
                    }
                    catch
                    {
                        Core.Logger.Warning($"Failed to delete log file: '{Path.GetFileName(file.Item1)}'");
                    }
                }
            }
        }

        var latestPath = Path.Combine(Core.BaseDir, "MelonLoader", "Latest.log");
        var cachedPath = Path.Combine(logsDir, $"{DateTime.Now:%y-%M-%d_%H-%m-%s}.log");

        MelonDebug.Log("Opening stream to latest log");
        try
        {
            var latest = new FileStream(latestPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            logFiles.Add(new StreamWriter(latest)
            {
                AutoFlush = true
            });
        }
        catch
        {
            Core.Logger.Warning($"Failed to create Latest.log. There might be another instance of the game");
        }

        MelonDebug.Log("Opening stream to cached log");
        try
        {
            var cached = new FileStream(cachedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            logFiles.Add(new StreamWriter(cached)
            {
                AutoFlush = true
            });
        }
        catch
        {
            Core.Logger.Warning($"Failed to create {Path.GetFileName(cachedPath)}");
        }

        if (logFiles.Count == 0)
        {
            Core.Logger.Error("Failed to create any log files. Logging to console only");
        }
    }

    private static void LogToFiles(string? log)
    {
        foreach (var file in logFiles)
        {
            if (log == null)
            {
                file.WriteLine();
                continue;
            }

            file.WriteLine(log);
        }
    }

    public static void Log(ColorRGB msgColor, ReadOnlySpan<char> msg)
    {
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] {msg}");

        if (ConsoleHandler.Hidden)
            return;

        if (WineUtils.IsWine)
        {
            Console.ResetColor();
            Console.Write('[');

            Console.ForegroundColor = legacyTimeColor;
            Console.Write(time);

            Console.ResetColor();
            Console.Write("] ");

            Console.ForegroundColor = ConsoleHandler.GetClosestConsoleColor(msgColor);
            Console.Out.WriteLine(msg);

            return;
        }

        Console.WriteLine($"[{time.Pastel(timeColor)}] {msg.Pastel(msgColor)}");
    }

    public static void Log(ColorRGB msgColor, ReadOnlySpan<char> msg, ColorRGB sectionColor, ReadOnlySpan<char> sectionName)
    {
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] [{sectionName}] {msg}");

        if (ConsoleHandler.Hidden)
            return;

        if (WineUtils.IsWine)
        {
            Console.ResetColor();
            Console.Write('[');

            Console.ForegroundColor = legacyTimeColor;
            Console.Write(time);

            Console.ResetColor();
            Console.Write("] [");

            Console.ForegroundColor = ConsoleHandler.GetClosestConsoleColor(sectionColor);
            Console.Out.Write(sectionName);

            Console.ResetColor();
            Console.Write("] ");

            Console.ForegroundColor = ConsoleHandler.GetClosestConsoleColor(msgColor);
            Console.Out.WriteLine(msg);

            return;
        }

        Console.WriteLine($"[{time.Pastel(timeColor)}] [{sectionName.Pastel(sectionColor)}] {msg.Pastel(msgColor)}");
    }

    public static void LogError(ReadOnlySpan<char> msg)
    {
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] {msg}");

        if (ConsoleHandler.Hidden)
            return;

        if (WineUtils.IsWine)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{time}] {msg}");

            return;
        }

        Console.WriteLine($"[{time}] {msg}".Pastel(Color.IndianRed));
    }

    public static void LogError(ReadOnlySpan<char> msg, ReadOnlySpan<char> sectionName)
    {
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] [{sectionName}] {msg}");

        if (ConsoleHandler.Hidden)
            return;

        if (WineUtils.IsWine)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{time}] [{sectionName}] {msg}");

            return;
        }

        Console.WriteLine($"[{time}] [{sectionName}] {msg}".Pastel(Color.IndianRed));
    }

    public static void LogMelonInfo(ColorRGB nameColor, ReadOnlySpan<char> name, ReadOnlySpan<char> info)
    {
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] {name} {info}");

        if (ConsoleHandler.Hidden)
            return;

        if (WineUtils.IsWine)
        {
            Console.ResetColor();
            Console.Write('[');

            Console.ForegroundColor = legacyTimeColor;
            Console.Write(time);

            Console.ResetColor();
            Console.Write("] ");

            Console.ForegroundColor = ConsoleHandler.GetClosestConsoleColor(nameColor);
            Console.Out.Write(name);
            Console.Write(' ');

            Console.ResetColor();
            Console.Out.WriteLine(info);

            return;
        }

        Console.WriteLine($"[{time.Pastel(timeColor)}] {name.Pastel(nameColor)} {info}");
    }

    public static void LogSpacer()
    {
        LogToFiles(null);

        if (ConsoleHandler.Hidden)
            return;

        Console.WriteLine();
    }
}
