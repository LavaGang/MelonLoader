using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MelonLoader.Fixes
{
	internal static class QuitFix
	{
		internal static void Run()
		{
			if (!ShouldRun())
				return;
			Process.GetCurrentProcess().Kill();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern static bool ShouldRun();
	}
}