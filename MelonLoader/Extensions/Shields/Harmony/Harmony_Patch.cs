using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.Harmony
{
	internal static class Harmony_Patch
	{
		private static RuntimeILReferenceBag.FastDelegateInvokers.Func<
			HarmonyLib.Harmony,
			MethodBase,
			HarmonyMethod,
			HarmonyMethod,
			HarmonyMethod,
			HarmonyMethod,
			HarmonyMethod,
			MethodInfo
			> Original = null;

		internal static void Install()
		{
			try
			{
				IDetour detour = new Detour(
					typeof(HarmonyLib.Harmony).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(x => (x.Name.Equals("Patch") && (x.GetParameters().Length == 6))),
					typeof(Harmony_Patch).GetMethod("New", BindingFlags.NonPublic | BindingFlags.Static)
				);

				Original = detour.GenerateTrampoline<
					RuntimeILReferenceBag.FastDelegateInvokers.Func<
						HarmonyLib.Harmony,
						MethodBase,
						HarmonyMethod,
						HarmonyMethod,
						HarmonyMethod,
						HarmonyMethod,
						HarmonyMethod,
						MethodInfo
						>>();
			}
			catch (Exception ex) { PatchShield.LogException(ex); }
		}

		private static MethodInfo New(
			HarmonyLib.Harmony self,
			MethodBase original,
			HarmonyMethod prefix,
			HarmonyMethod postfix,
			HarmonyMethod transpiler,
			HarmonyMethod finalizer,
			HarmonyMethod ilmanipulator
		) {
			if ((original.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (original.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (original.GetCustomAttributes(typeof(PatchShield), false).Length > 0))
				return null;
			return Original(self, original, prefix, postfix, transpiler, finalizer, ilmanipulator);
		}
	}
}