using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace MelonLoader.Fixes
{
	internal static class AccessToolsFix
	{
		internal static void Install()
		{
			MethodInfo patchMethod = typeof(AccessToolsFix).GetMethod("PatchMethod", BindingFlags.NonPublic | BindingFlags.Static);
			MethodInfo targetMethod = typeof(AccessToolsFix).Assembly.GetType("HarmonyLib.AccessTools")
				.GetMethods(BindingFlags.Public | BindingFlags.Static)
				.FirstOrDefault(x => x.Name.Equals("Method") && (x.GetParameters().Count() == 4) && (x.GetParameters()[0].ParameterType == typeof(Type)));

			try
			{
				Core.HarmonyInstance.Patch(targetMethod, patchMethod.ToNewHarmonyMethod());
			}
			catch (Exception ex) { MelonLogger.Warning($"AccessToolsFix Exception: {ex}"); }
		}

		// Modified from HarmonyX's HarmonyLib.AccessTools.Method : https://github.com/BepInEx/HarmonyX/blob/master/Harmony/Tools/AccessTools.cs#L326
		private static bool PatchMethod(Type __0, string __1, Type[] __2, Type[] __3, ref MethodInfo  __result)
		{
			if (__0 is null)
			{
				MelonLogger.Warning("AccessTools.Method: type is null");
				__result = null;
				return false;
			}

			if (__1 is null)
			{
				MelonLogger.Warning("AccessTools.Method: name is null");
				__result = null;
				return false;
			}

			var modifiers = new ParameterModifier[] { };
			if (__2 is null)
			{
				try { __result = AccessTools.FindIncludingBaseTypes(__0, t => t.GetMethod(__1, AccessTools.all)); }
				catch (AmbiguousMatchException ex)
				{
					__result = AccessTools.FindIncludingBaseTypes(__0, t => t.GetMethod(__1, AccessTools.all, null, new Type[0], modifiers));
					if (__result is null)
					{
						AmbiguousMatchException newex = new AmbiguousMatchException($"Ambiguous match in Harmony patch for {__0}:{__1}");
						newex.SetInnerException(ex);
						throw newex;
					}
				}
			}
			else
				__result = AccessTools.FindIncludingBaseTypes(__0, t => t.GetMethod(__1, AccessTools.all, null, __2, modifiers));

			if (__result is null)
			{
				MelonLogger.Warning($"AccessTools.Method: Could not find method for type {__0} and name {__1} and parameters {__2?.Description()}");
				return false;
			}

			if (__3 is object)
				__result = __result.MakeGenericMethod(__3);
			return false;
		}
	}
}