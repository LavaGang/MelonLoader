using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using MonoMod.RuntimeDetour;

namespace MelonLoader.Fixes
{
	internal static class InvariantCurrentCulture
	{
		internal static void Install()
		{
			MethodInfo patchMethod = typeof(InvariantCurrentCulture).GetMethod("New", BindingFlags.NonPublic | BindingFlags.Static);

			try
			{
				IDetour CurrentCulture_Detour = new Detour(
					typeof(Thread).GetProperty("CurrentCulture", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
					patchMethod
				);
			}
			catch (Exception ex) { MelonLogger.Warning($"Thread.CurrentCulture Exception: {ex}"); }

			try
			{
				IDetour CurrentUICulture_Detour = new Detour(
					typeof(Thread).GetProperty("CurrentUICulture", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
					patchMethod
				);
			}
			catch (Exception ex) { MelonLogger.Warning($"Thread.CurrentUICulture Exception: {ex}"); }
		}

		private static CultureInfo New(HarmonyLib.Harmony self) => CultureInfo.InvariantCulture;
	}
}