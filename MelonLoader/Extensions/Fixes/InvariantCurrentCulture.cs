using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using HarmonyLib;

namespace MelonLoader.Fixes
{
	internal static class InvariantCurrentCulture
	{
		internal static void Install()
		{
			Type threadType = typeof(Thread);
			HarmonyMethod patchMethod = AccessTools.Method(typeof(InvariantCurrentCulture), "PatchMethod").ToNewHarmonyMethod();

			try{ Core.HarmonyInstance.Patch(AccessTools.PropertyGetter(threadType, "CurrentCulture"), patchMethod); }
			catch (Exception ex) { MelonLogger.Warning($"Thread.CurrentCulture Exception: {ex}"); }

			try { Core.HarmonyInstance.Patch(AccessTools.PropertyGetter(threadType, "CurrentUICulture"), patchMethod); }
			catch (Exception ex) { MelonLogger.Warning($"Thread.CurrentUICulture Exception: {ex}"); }
		}

		private static bool PatchMethod(ref CultureInfo __result)
		{
			__result = CultureInfo.InvariantCulture;
			return false;
		}
	}
}