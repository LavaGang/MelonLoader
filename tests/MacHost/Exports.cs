using System.Runtime.InteropServices;
using MelonLoader.Bootstrap.Utils;
using MelonLoader.Utils;

internal static class Exports {
	[UnmanagedCallersOnly(EntryPoint = "check_host")]
	public static int CheckHost(IntPtr expectedApp, IntPtr expectedBase) {
		try {
			var app = Marshal.PtrToStringUTF8(expectedApp)!;
			var basedir = Marshal.PtrToStringUTF8(expectedBase)!;
			Check(ArgParser.GetValue("melonloader.basedir") == basedir, "host basedir argument");
			Check(ArgParser.GetValue("test.empty") == "", "empty argument");
			Check(ArgParser.GetValue("test.equals") == "a=b", "embedded equals");
			Check(ArgParser.GetValue("test.unicode") == "音楽 café", "UTF-8 argument");
			Check(ArgParser.IsDefined("test.flag"), "boolean flag");
			Check(ArgParser.GetValue("test.flag") == null, "flag has no value");
			Check(!ArgParser.IsDefined("test.absent"), "absent flag");
			Check(MelonEnvironment.GameExecutablePath == app, "game app bundle");
			Check(MelonEnvironment.UnityGameDataDirectory == Path.Combine(app, "Contents/Resources/Data"), "game data directory");
			Check(MelonEnvironment.MelonBaseDirectory == basedir, "separate loader base directory");
			Console.WriteLine("PASS: native host arguments and executable path");
			return 0;
		} catch (Exception e) {
			Console.Error.WriteLine(e);
			return 1;
		}
	}

	private static void Check(bool condition, string label) {
		if (!condition) {
			throw new Exception("FAIL: " + label);
		}
	}
}

// Only the environment's unrelated configuration and logging dependencies are
// stubbed; argument parsing and executable discovery use production sources.
namespace MelonLoader {
	internal static class LoaderConfig {
		internal static Configuration Current { get; } = new();
		internal sealed class Configuration {
			internal LoaderOptions Loader { get; } = new();
		}
		internal sealed class LoaderOptions {
			internal string BaseDirectory => ArgParser.GetValue("melonloader.basedir")!;
		}
	}
	internal static class MelonLogger {
		internal static void MsgDirect(string message) => Console.WriteLine(message);
	}
	internal static class MelonUtils {
		internal static string GetPathAncestor(string path, int count) {
			for (var i = 0; i < count; i++) {
				path = Path.GetDirectoryName(path)!;
			}
			return path;
		}
	}
}
