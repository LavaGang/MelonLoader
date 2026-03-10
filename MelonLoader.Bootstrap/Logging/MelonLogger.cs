using MelonLoader.Bootstrap.Utils;
using MelonLoader.Logging;

namespace MelonLoader.Bootstrap.Logging;

internal static class MelonLogger
{
    private static readonly string timeFormat = "HH:mm:ss.fff";
    private static readonly ColorARGB timeColor = ColorARGB.LimeGreen;
    private static readonly ConsoleColor legacyTimeColor = ConsoleColor.Green;

    private static readonly List<StreamWriter> logFiles = new();

    public static void Init()
    {
        // Making logs from this point is ok, but only for console

        MelonDebug.Log($"Creating log files (Max logs: {LoaderConfig.Current.Logs.MaxLogs})");

        var logsDir = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "Logs");
        Directory.CreateDirectory(logsDir);

        if (LoaderConfig.Current.Logs.MaxLogs > 0)
        {
            var logs = Directory.GetFiles(logsDir, "*.log", SearchOption.TopDirectoryOnly);
            if (logs.Length >= LoaderConfig.Current.Logs.MaxLogs)
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

                var toDelete = logs.Length - LoaderConfig.Current.Logs.MaxLogs + 1;
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

        var latestPath = Path.Combine(LoaderConfig.Current.Loader.BaseDirectory, "MelonLoader", "Latest.log");
        var cachedPath = Path.Combine(logsDir, $"{DateTime.Now:%y-%M-%d_%H-%m-%s}.log");

        //MelonDebug.Log("Opening stream to latest log");
        try
        {
            var latest = new FileStream(latestPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            latest.Seek(0, SeekOrigin.End);
            logFiles.Add(new StreamWriter(latest)
            {
                AutoFlush = true
            });
        }
        catch
        {
            Core.Logger.Warning($"Failed to create Latest.log. There might be another instance of the game");
        }

        //MelonDebug.Log("Opening stream to cached log");
        try
        {
            var cached = new FileStream(cachedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            cached.Seek(0, SeekOrigin.End);
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
            file.BaseStream.Seek(0, SeekOrigin.End);
            if (log == null)
            {
                file.WriteLine();
                continue;
            }

            file.WriteLine(log);
        }
    }

    public static void Log(ColorARGB msgColor, ReadOnlySpan<char> msg, ReadOnlySpan<char> strippedMessage)
    {
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] {strippedMessage}");

        if (!ConsoleHandler.IsOpen)
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
            Console.ResetColor();

            return;
        }

        Console.WriteLine($"[{time.Pastel(timeColor)}] {msg.Pastel(msgColor)}");
    }

    // HACK: There's definitely a better way to implement MsgPastel, but for now this will do. This required the strippedMessage parameter to be provided which isn't really optimal
    public static void Log(ColorARGB msgColor, ReadOnlySpan<char> msg, ColorARGB sectionColor, ReadOnlySpan<char> sectionName, ReadOnlySpan<char> strippedMessage)
    {
        var sectionPart = string.IsNullOrEmpty(sectionName.ToString()) ? "" : $"[{sectionName}] ";
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] {sectionPart}{strippedMessage}");

        if (!ConsoleHandler.IsOpen)
            return;

        if (WineUtils.IsWine)
        {
            Console.ResetColor();
            Console.Write('[');

            Console.ForegroundColor = legacyTimeColor;
            Console.Write(time);

            if (!sectionName.IsEmpty)
            {
                Console.ResetColor();
                Console.Write("] [");
                Console.ForegroundColor = ConsoleHandler.GetClosestConsoleColor(sectionColor);
                Console.Out.Write(sectionName);
                Console.ResetColor();
                Console.Write("] ");
            }
            else
            {
                Console.Write("] ");
            }

            Console.ForegroundColor = ConsoleHandler.GetClosestConsoleColor(msgColor);
            Console.Out.WriteLine(msg);
            Console.ResetColor();

            return;
        }
        
        if (!sectionName.IsEmpty)
            Console.WriteLine($"[{time.Pastel(timeColor)}] [{sectionName.Pastel(sectionColor)}] {msg.Pastel(msgColor)}");
        else
            Console.WriteLine($"[{time.Pastel(timeColor)}] {msg.Pastel(msgColor)}");
    }

    public static void LogWarning(ReadOnlySpan<char> msg)
    {
        if (LoaderConfig.Current.Console.HideWarnings)
        {
            var time = DateTime.Now.ToString(timeFormat);
            
            LogToFiles($"[{time}] {msg}");

            return;
        }

        Log(ColorARGB.Yellow, msg, msg);
    }

    public static void LogWarning(ReadOnlySpan<char> msg, ReadOnlySpan<char> sectionName)
    {
        if (LoaderConfig.Current.Console.HideWarnings)
        {
            var sectionPart = string.IsNullOrEmpty(sectionName.ToString()) ? "" : $"[{sectionName}] ";
            var time = DateTime.Now.ToString(timeFormat);
            
            LogToFiles($"[{time}] {sectionPart}{msg}");

            return;
        }

        Log(ColorARGB.Yellow, msg, ColorARGB.Yellow, sectionName, msg);
    }

    public static void LogError(ReadOnlySpan<char> msg)
    {
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] {msg}");

        if (!ConsoleHandler.IsOpen)
            return;

        if (WineUtils.IsWine)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{time}] {msg}");
            Console.ResetColor();

            return;
        }

        Console.WriteLine($"[{time}] {msg}".Pastel(ColorARGB.IndianRed));
    }

    public static void LogError(ReadOnlySpan<char> msg, ReadOnlySpan<char> sectionName)
    {
        var sectionPart = string.IsNullOrEmpty(sectionName.ToString()) ? "" : $"[{sectionName}] ";
        var time = DateTime.Now.ToString(timeFormat);
        
        LogToFiles($"[{time}] {sectionPart}{msg}");

        if (!ConsoleHandler.IsOpen)
            return;

        if (WineUtils.IsWine)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (!sectionName.IsEmpty)
                Console.WriteLine($"[{time}] [{sectionName}] {msg}");
            else
                Console.WriteLine($"[{time}] {msg}");

            Console.ResetColor();

            return;
        }

        if (!sectionName.IsEmpty)
            Console.WriteLine($"[{time}] [{sectionName}] {msg}".Pastel(ColorARGB.IndianRed));
        else
            Console.WriteLine($"[{time}] {msg}".Pastel(ColorARGB.IndianRed));
    }

    public static void LogMelonInfo(ColorARGB nameColor, ReadOnlySpan<char> name, ReadOnlySpan<char> info)
    {
        var time = DateTime.Now.ToString(timeFormat);

        LogToFiles($"[{time}] {name} {info}");

        if (!ConsoleHandler.IsOpen)
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
            Console.ResetColor();

            return;
        }

        Console.WriteLine($"[{time.Pastel(timeColor)}] {name.Pastel(nameColor)} {info}");
    }

    public static void LogSpacer()
    {
        LogToFiles(null);

        if (!ConsoleHandler.IsOpen)
            return;

        Console.WriteLine();
    }
}