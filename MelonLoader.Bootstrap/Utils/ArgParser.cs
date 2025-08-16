namespace MelonLoader.Bootstrap.Utils;

internal static class ArgParser
{
    private static readonly List<Argument> arguments;

    static ArgParser()
    {
        arguments = [];

        var args = Environment.GetCommandLineArgs();

        for (var i = 1; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg.StartsWith("--"))
            {
                arg = arg[2..];
            }
            else if (arg.StartsWith('-'))
            {
                arg = arg[1..];
            }
            else
            {
                continue;
            }

            string? value = null;

            var eqIdx = arg.IndexOf('=');
            if (eqIdx >= 0)
            {
                value = arg[(eqIdx + 1)..];
                arg = arg[..eqIdx];
            }
            else if (i + 1 < args.Length)
            {
                var next = args[i + 1];

                if (!next.StartsWith('-'))
                {
                    value = next;
                    i++;
                }
            }

            arguments.Add(new()
            {
                Name = arg,
                Value = value
            });
        }
    }

    public static bool IsDefined(string longName)
    {
        return arguments.Exists(x => x.Name.Equals(longName, StringComparison.OrdinalIgnoreCase));
    }

    public static string? GetValue(string longName)
    {
        var arg = arguments.Find(x => x.Name.Equals(longName, StringComparison.OrdinalIgnoreCase));
        return arg?.Value;
    }

    private class Argument
    {
        public required string Name { get; init; }
        public string? Value { get; init; }
    }
}
