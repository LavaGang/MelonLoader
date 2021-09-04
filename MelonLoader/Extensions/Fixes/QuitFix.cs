using System.Diagnostics;

namespace MelonLoader.Fixes
{
	internal static class QuitFix
	{
		
		if (!MelonLaunchOptions.Core.QuitFix)
			return;
		Process.GetCurrentProcess().Kill();
		
	}
}
