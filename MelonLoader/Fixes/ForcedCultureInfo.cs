using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;

namespace MelonLoader.Fixes
{
	internal static class ForcedCultureInfo
	{
		private static CultureInfo SelectedCultureInfo = CultureInfo.InvariantCulture;

		internal static void Install()
		{
			Type PatchType = typeof(ForcedCultureInfo);
			HarmonyMethod PatchMethod_Get = PatchType.GetMethod("GetMethod", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();
			HarmonyMethod PatchMethod_Set = PatchType.GetMethod("SetMethod", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();

			Type CultureInfoType = typeof(CultureInfo);
			Type ThreadType = typeof(Thread);

			foreach (FieldInfo fieldInfo in ThreadType
				.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.Where(x => x.FieldType == CultureInfoType))
				fieldInfo.SetValue(null, SelectedCultureInfo);

			foreach (PropertyInfo propertyInfo in ThreadType
				.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				.Where(x => x.PropertyType == CultureInfoType))
			{
				MethodInfo getMethod = propertyInfo.GetGetMethod();
				if (getMethod != null)
					try { Core.HarmonyInstance.Patch(getMethod, PatchMethod_Get); }
					catch (Exception ex) { MelonLogger.Warning($"Exception Patching Thread Get Method {propertyInfo.Name}: {ex}"); }

				MethodInfo setMethod = propertyInfo.GetSetMethod();
				if (setMethod != null)
					try { Core.HarmonyInstance.Patch(setMethod, PatchMethod_Set); }
					catch (Exception ex) { MelonLogger.Warning($"Exception Patching Thread Set Method {propertyInfo.Name}: {ex}"); }
			}
		}

		private static bool GetMethod(ref CultureInfo __result)
		{
			__result = SelectedCultureInfo;
			return false;
		}
		private static bool SetMethod() => false;
	}
}