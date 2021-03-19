using System;
using System.Reflection;
using HarmonyLib;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.Harmony
{
	internal static class HarmonyManipulator_Manipulate
	{
		private static RuntimeILReferenceBag.FastDelegateInvokers.Action<
			MethodBase,
			PatchInfo,
			ILContext
			> Original = null;

		internal static void Install()
		{
			try
			{
				IDetour detour = new Detour(
					typeof(HarmonyLib.Public.Patching.HarmonyManipulator).GetMethod("Manipulate", BindingFlags.Public | BindingFlags.Static),
					typeof(HarmonyManipulator_Manipulate).GetMethod("New", BindingFlags.NonPublic | BindingFlags.Static)
				);

				Original = detour.GenerateTrampoline<
					RuntimeILReferenceBag.FastDelegateInvokers.Action<
						MethodBase,
						PatchInfo,
						ILContext
						>>();
			}
			catch (Exception ex) { PatchShield.LogException(ex); }
		}

		private static void New(
			MethodBase original,
			PatchInfo patchInfo,
			ILContext ctx
		) {
			if ((original.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (original.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (original.GetCustomAttributes(typeof(PatchShield), false).Length > 0))
				return;
			Original(original, patchInfo, ctx);
		}
	}
}