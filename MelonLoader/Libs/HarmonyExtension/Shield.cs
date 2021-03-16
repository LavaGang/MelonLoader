using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using HarmonyLib.Tools;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
	public class HarmonyShield : Attribute
	{
		private static RuntimeILReferenceBag.FastDelegateInvokers.Func<HarmonyLib.Harmony, MethodBase, HarmonyMethod, HarmonyMethod, HarmonyMethod, HarmonyMethod, HarmonyMethod, MethodInfo> Harmony_Patch_Original = null;
		private static RuntimeILReferenceBag.FastDelegateInvokers.Func<HarmonyMethod, MethodBase, MethodInfo, MethodInfo, MethodInfo> PatchFunctions_ReversePatch_Original = null;
		private static RuntimeILReferenceBag.FastDelegateInvokers.Action<MethodBase, PatchInfo, ILContext> HarmonyManipulator_Manipulate_Original = null;

		internal static void Install()
        {
			try
			{
				IDetour Harmony_Patch_Detour = new Detour(
					typeof(HarmonyLib.Harmony).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(x => (x.Name.Equals("Patch") && (x.GetParameters().Length == 6))),
					typeof(HarmonyShield).GetMethod("Patch", BindingFlags.NonPublic | BindingFlags.Static)
				);
				Harmony_Patch_Original = Harmony_Patch_Detour.GenerateTrampoline<RuntimeILReferenceBag.FastDelegateInvokers.Func<HarmonyLib.Harmony, MethodBase, HarmonyMethod, HarmonyMethod, HarmonyMethod, HarmonyMethod, HarmonyMethod, MethodInfo>>();

				IDetour PatchFunctions_ReversePatch_Detour = new Detour(
					typeof(PatchFunctions).GetMethod("ReversePatch", BindingFlags.NonPublic | BindingFlags.Static),
					typeof(HarmonyShield).GetMethod("ReversePatch", BindingFlags.NonPublic | BindingFlags.Static)
				);
				PatchFunctions_ReversePatch_Original = PatchFunctions_ReversePatch_Detour.GenerateTrampoline<RuntimeILReferenceBag.FastDelegateInvokers.Func<HarmonyMethod, MethodBase, MethodInfo, MethodInfo, MethodInfo>>();

				IDetour HarmonyManipulator_Manipulate_Detour = new Detour(
					typeof(HarmonyLib.Public.Patching.HarmonyManipulator).GetMethod("Manipulate", BindingFlags.Public | BindingFlags.Static),
					typeof(HarmonyShield).GetMethod("Manipulate", BindingFlags.NonPublic | BindingFlags.Static)
				);
				HarmonyManipulator_Manipulate_Original = HarmonyManipulator_Manipulate_Detour.GenerateTrampoline<RuntimeILReferenceBag.FastDelegateInvokers.Action<MethodBase, PatchInfo, ILContext>>();
			}
			catch (Exception e) { Logger.LogText(Logger.LogChannel.Error, $"Failed to apply shield: ({e.GetType().FullName}) {e.Message}"); }
		}

		private static MethodInfo Patch(
			HarmonyLib.Harmony self,
			MethodBase original, 
			HarmonyMethod prefix,
			HarmonyMethod postfix,
			HarmonyMethod transpiler,
			HarmonyMethod finalizer,
			HarmonyMethod ilmanipulator)
		{
			if ((original.DeclaringType.Assembly.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0)
				|| (original.DeclaringType.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0)
				|| (original.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0))
				return null;
			return Harmony_Patch_Original(self, original, prefix, postfix, transpiler, finalizer, ilmanipulator);
		}

		private static MethodInfo ReversePatch(
			HarmonyMethod standin,
			MethodBase original,
			MethodInfo postTranspiler,
			MethodInfo postManipulator)
		{
			if ((original.DeclaringType.Assembly.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0)
				|| (original.DeclaringType.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0)
				|| (original.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0))
				return null;
			return PatchFunctions_ReversePatch_Original(standin, original, postTranspiler, postManipulator);
		}

		private static void Manipulate(
			MethodBase original,
			PatchInfo patchInfo,
			ILContext ctx)
		{
			if ((original.DeclaringType.Assembly.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0)
				|| (original.DeclaringType.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0)
				|| (original.GetCustomAttributes(typeof(HarmonyShield), false).Length > 0))
				return;
			HarmonyManipulator_Manipulate_Original(original, patchInfo, ctx);
		}
	}
}