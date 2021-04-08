using System;
using System.Reflection;
using HarmonyLib;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.Harmony
{
	internal static class PatchFunctions_ReversePatch
	{
		private static RuntimeILReferenceBag.FastDelegateInvokers.Func<
			HarmonyMethod,
			MethodBase,
			MethodInfo,
			MethodInfo,
			MethodInfo
			> Original = null;

		internal static void Install()
		{
			try
			{
				IDetour detour = new Detour(
					typeof(PatchFunctions).GetMethod("ReversePatch", BindingFlags.NonPublic | BindingFlags.Static),
					typeof(PatchFunctions_ReversePatch).GetMethod("New", BindingFlags.NonPublic | BindingFlags.Static)
				);

				Original = detour.GenerateTrampoline<
					RuntimeILReferenceBag.FastDelegateInvokers.Func<
						HarmonyMethod,
						MethodBase,
						MethodInfo,
						MethodInfo,
						MethodInfo
						>>();
			}
			catch (Exception ex) { PatchShield.LogException(ex); }
		}

		private static MethodInfo New(
			HarmonyMethod standin,
			MethodBase original,
			MethodInfo postTranspiler,
			MethodInfo postManipulator
		) {
			if ((original.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (original.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (original.GetCustomAttributes(typeof(PatchShield), false).Length > 0))
				return null;
			return Original(standin, original, postTranspiler, postManipulator);
		}
	}
}