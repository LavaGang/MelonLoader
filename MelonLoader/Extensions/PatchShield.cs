using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
	public class PatchShield : Attribute
	{
		internal static DetourConfig DConfig = new DetourConfig() { ManualApply = true };

		internal static void LogException(Exception ex) => MelonLogger.Warning($"Patch Shield Exception: {ex}");

		internal static bool MethodCheck(MethodBase method) =>
			(method != null)
			&& (method.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
			&& (method.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
			&& (method.GetCustomAttributes(typeof(PatchShield), false).Length <= 0);

		internal static void Install()
		{
			// Harmony
			HarmonyShields.Harmony_Patch.Install();
			HarmonyShields.PatchFunctions_ReversePatch.Install();
			HarmonyShields.HarmonyManipulator_Manipulate.Install();

			// MonoMod
			Hook.OnDetour += (detour, originalMethod, patchMethod, delegateTarget) => MethodCheck(originalMethod);
			ILHook.OnDetour += (detour, originalMethod, ilmanipulator) => MethodCheck(originalMethod);

			// Always Run Last
			Detour.OnDetour += (detour, originalMethod, patchMethod) => MethodCheck(originalMethod);
		}

		private static class HarmonyShields
		{
			internal static class Harmony_Patch
			{
				private static Detour DetourInterface = null;

				private static RuntimeILReferenceBag.FastDelegateInvokers.Func<
					HarmonyLib.Harmony,
					MethodBase,
					HarmonyMethod,
					HarmonyMethod,
					HarmonyMethod,
					HarmonyMethod,
					HarmonyMethod,
					MethodInfo
					> OriginalMethod = null;

				private static RuntimeILReferenceBag.FastDelegateInvokers.Func<
					HarmonyLib.Harmony,
					MethodBase,
					HarmonyMethod,
					HarmonyMethod,
					HarmonyMethod,
					HarmonyMethod,
					HarmonyMethod,
					MethodInfo
					> DetourMethod = null;

				internal static void Install()
				{
					try
					{
						DetourMethod =
							(self, original, prefix, postfix, transpiler, finalizer, ilmanipulator) =>
							(MethodCheck(original)
							? OriginalMethod(self, original, prefix, postfix, transpiler, finalizer, ilmanipulator)
							: null);

						DetourInterface = new Detour(
							typeof(HarmonyLib.Harmony).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(x => (x.Name.Equals("Patch") && (x.GetParameters().Length == 6))),
							DetourMethod.Method,
							DConfig
						);

						OriginalMethod = DetourInterface.GenerateTrampoline<RuntimeILReferenceBag.FastDelegateInvokers.Func<
							HarmonyLib.Harmony,
							MethodBase,
							HarmonyMethod,
							HarmonyMethod,
							HarmonyMethod,
							HarmonyMethod,
							HarmonyMethod,
							MethodInfo
						>>();

						DetourInterface.Apply();
					}
					catch (Exception ex) { LogException(ex); }
				}
			}

			internal static class HarmonyManipulator_Manipulate
			{
				private static Detour DetourInterface = null;

				private static RuntimeILReferenceBag.FastDelegateInvokers.Action<
					MethodBase,
					PatchInfo,
					ILContext
					> OriginalMethod = null;

				private static RuntimeILReferenceBag.FastDelegateInvokers.Action<
					MethodBase,
					PatchInfo,
					ILContext
					> DetourMethod = null;

				internal static void Install()
				{
					try
					{
						DetourMethod =
							(original, patchInfo, ctx) =>
							{
								if (PatchShield.MethodCheck(original))
									OriginalMethod(original, patchInfo, ctx);
							};

						DetourInterface = new Detour(
							typeof(HarmonyLib.Public.Patching.HarmonyManipulator).GetMethod("Manipulate", BindingFlags.Public | BindingFlags.Static),
							DetourMethod.Method,
							DConfig
						);

						OriginalMethod = DetourInterface.GenerateTrampoline<RuntimeILReferenceBag.FastDelegateInvokers.Action<
							MethodBase,
							PatchInfo,
							ILContext
						>>();

						DetourInterface.Apply();
					}
					catch (Exception ex) { PatchShield.LogException(ex); }
				}
			}

			internal static class PatchFunctions_ReversePatch
			{
				private static Detour DetourInterface = null;

				private static RuntimeILReferenceBag.FastDelegateInvokers.Func<
					HarmonyMethod,
					MethodBase,
					MethodInfo,
					MethodInfo,
					MethodInfo
					> OriginalMethod = null;

				private static RuntimeILReferenceBag.FastDelegateInvokers.Func<
					HarmonyMethod,
					MethodBase,
					MethodInfo,
					MethodInfo,
					MethodInfo
					> DetourMethod = null;

				internal static void Install()
				{
					try
					{
						DetourMethod =
							(standin, original, postTranspiler, postManipulator) =>
							(PatchShield.MethodCheck(original)
							? OriginalMethod(standin, original, postTranspiler, postManipulator)
							: null);

						DetourInterface = new Detour(
							typeof(PatchFunctions).GetMethod("ReversePatch", BindingFlags.NonPublic | BindingFlags.Static),
							DetourMethod.Method,
							DConfig
						);

						OriginalMethod = DetourInterface.GenerateTrampoline<
							RuntimeILReferenceBag.FastDelegateInvokers.Func<
								HarmonyMethod,
								MethodBase,
								MethodInfo,
								MethodInfo,
								MethodInfo
								>>();

						DetourInterface.Apply();
					}
					catch (Exception ex) { PatchShield.LogException(ex); }
				}
			}
		}
	}
}