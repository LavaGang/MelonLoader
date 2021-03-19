using System;

namespace MelonLoader
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
	public class PatchShield : Attribute
	{
		internal static void LogException(Exception ex) => MelonLogger.Warning($"Patch Shield Exception: {ex}");

		internal static void Install()
        {
			PatchShields.Harmony.Harmony_Patch.Install();
			PatchShields.Harmony.PatchFunctions_ReversePatch.Install();
			PatchShields.Harmony.HarmonyManipulator_Manipulate.Install();
		}
	}
}