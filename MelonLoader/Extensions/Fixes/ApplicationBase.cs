using System;
using System.IO;
using System.Reflection;

namespace MelonLoader.Fixes
{
	internal static class ApplicationBase
	{
		internal static void Run(AppDomain domain)
		{
			string gameDir = MelonUtils.GameDirectory;
			try
			{
				((AppDomainSetup)typeof(AppDomain).GetProperty("SetupInformationNoCopy", BindingFlags.NonPublic | BindingFlags.Instance)
					.GetValue(domain, new object[0]))
					.ApplicationBase = gameDir;
			}
			catch (Exception ex) { MelonLogger.Warning($"AppDomainSetup.ApplicationBase Exception: {ex}"); }
			Directory.SetCurrentDirectory(gameDir);
		}
	}
}