using System;

namespace MelonLoader.Fixes
{
	internal static class ApplicationBase
	{
		internal static void Run(AppDomain domain)
		{
			string gameDir = string.Copy(MelonUtils.GameDirectory);
			MelonUtils.SetCurrentDomainBaseDirectory(gameDir, domain);
		}
	}
}