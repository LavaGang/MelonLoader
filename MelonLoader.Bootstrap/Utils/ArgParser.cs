namespace MelonLoader.Bootstrap.Utils;

internal static class ArgParser
{
#if OSX
	[System.Runtime.InteropServices.DllImport("/usr/lib/libSystem.B.dylib")]
	private static extern IntPtr _NSGetArgc();

	[System.Runtime.InteropServices.DllImport("/usr/lib/libSystem.B.dylib")]
	private static extern IntPtr _NSGetArgv();

	private static string[] GetHostArguments() {
		// A NativeAOT shared library does not receive the host's argv through
		// Environment.GetCommandLineArgs(). Ask the C runtime for the game args.
		var count = System.Runtime.InteropServices.Marshal.ReadInt32(_NSGetArgc());
		var argv = System.Runtime.InteropServices.Marshal.ReadIntPtr(_NSGetArgv());
		var args = new string[count];
		for (var i = 0; i < count; i++) {
			var argument = System.Runtime.InteropServices.Marshal.ReadIntPtr(argv, i * IntPtr.Size);
			args[i] = System.Runtime.InteropServices.Marshal.PtrToStringUTF8(argument)!;
		}
		return args;
	}
#endif

    private static readonly List<Argument> arguments;

    static ArgParser()
    {
        arguments = [];

#if OSX
		var args = GetHostArguments();
#else
        var args = Environment.GetCommandLineArgs();
#endif

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
