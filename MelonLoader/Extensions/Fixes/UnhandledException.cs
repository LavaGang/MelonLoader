using System;

namespace MelonLoader.Fixes
{
	internal static class UnhandledException
	{
		internal static void Run(AppDomain domain) =>
			domain.UnhandledException +=
				(sender, args) =>
					MelonLogger.Error((args.ExceptionObject as Exception).ToString());
	}
}