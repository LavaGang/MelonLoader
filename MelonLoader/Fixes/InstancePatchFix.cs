using System;
using System.Reflection;
using HarmonyLib;
using MonoMod.RuntimeDetour;

namespace MelonLoader.Fixes
{
    internal static class InstancePatchFix
    {
		internal static void Install()
		{
			Type instancePatchFixType = typeof(InstancePatchFix);
			HarmonyMethod patchMethod = AccessTools.Method(instancePatchFixType, "PatchMethod").ToNewHarmonyMethod();

			try
			{
				Core.HarmonyInstance.Patch(AccessTools.Method("HarmonyLib.PatchFunctions:ReversePatch"), patchMethod);
				Core.HarmonyInstance.Patch(AccessTools.Method("HarmonyLib.HarmonyMethod:ImportMethod"), patchMethod);
			}
			catch (Exception ex) { MelonLogger.Warning($"InstancePatchFix Exception: {ex}"); }

			Hook.OnDetour += (detour, originalMethod, patchMethod, delegateTarget) => PatchMethod(patchMethod);
			Detour.OnDetour += (detour, originalMethod, patchMethod) => PatchMethod(patchMethod);
		}

		private static bool PatchMethod(MethodBase __0)
        {
			if (__0 == null)
				throw new NullReferenceException("Patch Method");
			if ((__0 != null) && !__0.IsStatic)
				throw new Exception("Patch Method must be a Static Method!");
			return true;
		}
	}
}