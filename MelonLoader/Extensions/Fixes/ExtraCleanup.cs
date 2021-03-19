using System;
using System.IO;

namespace MelonLoader.Fixes
{
	internal static class ExtraCleanup
	{
		private static void LogException(Exception ex) => MelonLogger.Warning($"MelonLoader.Fixes.ExtraCleanup Exception: {ex}");

		internal static void Run()
		{
			try { Cleanup(MelonUtils.GameDirectory); } catch (Exception ex) { LogException(ex); }
			try { Cleanup(MelonUtils.UserDataDirectory); } catch (Exception ex) { LogException(ex); }
			try { Cleanup(MelonHandler.PluginsDirectory); } catch (Exception ex) { LogException(ex); }
			try { Cleanup(MelonHandler.ModsDirectory); } catch (Exception ex) { LogException(ex); }
		}

		private static void Cleanup(string destination)
		{
			string main_dll = Path.Combine(destination, "MelonLoader.dll");
			if (File.Exists(main_dll))
				File.Delete(main_dll);
			string main2_dll = Path.Combine(destination, "MelonLoader.ModHandler.dll");
			if (File.Exists(main2_dll))
				File.Delete(main2_dll);
		}
	}
}