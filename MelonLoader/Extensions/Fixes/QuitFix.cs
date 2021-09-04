using System.Diagnostics;

namespace MelonLoader.Fixes
{
	internal static class QuitFix
	{
		
		internal static void Run()
		{
			Process.GetCurrentProcess().Kill();
		}
		
	}
}
