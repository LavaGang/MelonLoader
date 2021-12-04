using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MonoMod.RuntimeDetour;

namespace MelonLoader
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
	public class PatchShield : Attribute //Naming violation?
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
			Type patchProcessorType = typeof(PatchProcessor);
			Type patchShieldType = typeof(PatchShield);
			PatchProcessor_OriginalRef = AccessTools.FieldRefAccess<MethodBase>(patchProcessorType, "original");

			try
			{
				Core.HarmonyInstance.Patch(
					AccessTools.Method("HarmonyLib.PatchFunctions:ReversePatch"),
					AccessTools.Method(patchShieldType, "PatchMethod_PatchFunctions_ReversePatch").ToNewHarmonyMethod()
					);
			}
			catch (Exception ex) { LogException(ex); }

			try
			{
				HarmonyMethod unpatchMethod = AccessTools.Method(patchShieldType, "PatchMethod_PatchProcessor_Unpatch").ToNewHarmonyMethod();
				foreach (MethodInfo method in patchProcessorType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name.Equals("Unpatch")))
					Core.HarmonyInstance.Patch(method, unpatchMethod);
			}
			catch (Exception ex) { LogException(ex); }

			try
			{
				Core.HarmonyInstance.Patch(AccessTools.Method(patchProcessorType, "Patch"),
					AccessTools.Method(patchShieldType, "PatchMethod_PatchProcessor_Patch").ToNewHarmonyMethod()
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