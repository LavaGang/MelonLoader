using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MonoMod.RuntimeDetour;

namespace MelonLoader
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
	public class PatchShield : Attribute
	{
		private static AccessTools.FieldRef<object, MethodBase> PatchProcessor_OriginalRef;

		private static void LogException(Exception ex) => MelonLogger.Warning($"Patch Shield Exception: {ex}");

		private static bool MethodCheck(MethodBase method) =>
			(method != null)
			&& (method.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
			&& (method.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
			&& (method.GetCustomAttributes(typeof(PatchShield), false).Length <= 0);

		internal static void Install()
        {
			PatchProcessor_OriginalRef = AccessTools.FieldRefAccess<MethodBase>(typeof(PatchProcessor), "original");
			HarmonyLib.Harmony harmonyInstance = new HarmonyLib.Harmony("PatchShield");

			try
			{
				harmonyInstance.Patch(
					typeof(PatchFunctions).GetMethod("ReversePatch", BindingFlags.NonPublic | BindingFlags.Static),
					new HarmonyMethod(typeof(PatchShield).GetMethod("PatchMethod_PatchFunctions_ReversePatch", BindingFlags.NonPublic | BindingFlags.Static))
					);
			}
			catch (Exception ex) { LogException(ex); }

			try
			{
				foreach (MethodInfo method in typeof(PatchProcessor).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name.Equals("Unpatch")))
					harmonyInstance.Patch(
						method,
						new HarmonyMethod(typeof(PatchShield).GetMethod("PatchMethod_PatchProcessor_Unpatch", BindingFlags.NonPublic | BindingFlags.Static))
						);
			}
			catch (Exception ex) { LogException(ex); }

			try
			{
				harmonyInstance.Patch(
					typeof(PatchProcessor).GetMethod("Patch", BindingFlags.Public | BindingFlags.Instance),
					new HarmonyMethod(typeof(PatchShield).GetMethod("PatchMethod_PatchProcessor_Patch", BindingFlags.NonPublic | BindingFlags.Static))
					);
			}
			catch (Exception ex) { LogException(ex); }

			Hook.OnDetour += (detour, originalMethod, patchMethod, delegateTarget) => MethodCheck(originalMethod);
			ILHook.OnDetour += (detour, originalMethod, ilmanipulator) => MethodCheck(originalMethod);
			Detour.OnDetour += (detour, originalMethod, patchMethod) => MethodCheck(originalMethod);
		}

		private static bool PatchMethod_PatchFunctions_ReversePatch(MethodBase __1) => MethodCheck(__1);
		private static bool PatchMethod_PatchProcessor_Patch(PatchProcessor __instance) => MethodCheck(PatchProcessor_OriginalRef(__instance));
		private static bool PatchMethod_PatchProcessor_Unpatch(PatchProcessor __instance) => MethodCheck(PatchProcessor_OriginalRef(__instance));
	}
}