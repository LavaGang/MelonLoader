using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;

namespace MelonLoader.Fixes
{
	internal static class ForcedCultureInfo
	{
		private static readonly CultureInfo SelectedCultureInfo = CultureInfo.InvariantCulture;
		private static readonly Type CultureInfoType = typeof(CultureInfo);
		private static readonly Type ThreadType = typeof(Thread);

		internal static void Install()
		{
            try
            {
	            Type PatchType = typeof(ForcedCultureInfo);
	            HarmonyMethod PatchMethod_Get = PatchType.GetMethod("GetMethod", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();
	            HarmonyMethod PatchMethod_Set = PatchType.GetMethod("SetMethod", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();

	            FixAllForType(CultureInfoType, PatchMethod_Get, PatchMethod_Set);
	            FixAllForType(ThreadType, PatchMethod_Get, PatchMethod_Set);
            }
            catch (Exception ex) { MelonLogger.Warning(ex); }
		}

		private static void FixAllForType(Type targetType, HarmonyMethod PatchMethod_Get, HarmonyMethod PatchMethod_Set)
		{
			foreach (FieldInfo fieldInfo in targetType
				         .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				         .Where(x => x.FieldType == CultureInfoType && !x.IsInitOnly))
			{
				MelonDebug.Msg($"Setting {targetType.Name}.{fieldInfo.Name} to Invariant CultureInfo...");
				try
                {
	                fieldInfo.SetValue(null, SelectedCultureInfo);
                }
                catch (Exception ex)
                {
                	MelonLogger.Warning($"Exception Setting {targetType.Name}.{fieldInfo.Name}: {ex}");
                }
			}

			List<MethodInfo> patchedMethods = new();
			foreach (MethodInfo method in targetType
				         .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				         .Where(x => x.ReturnType == CultureInfoType))
			{
				MelonDebug.Msg($"Patching {targetType.Name}.{method.Name}...");
				patchedMethods.Add(method);
				try
				{
					Core.HarmonyInstance.Patch(method, PatchMethod_Get);
				}
				catch (Exception ex)
				{
					MelonLogger.Warning($"Exception Patching {targetType.Name}.{method.Name}: {ex}");
				}
			}

			foreach (PropertyInfo propertyInfo in targetType
				         .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				         .Where(x => x.PropertyType == CultureInfoType))
			{
				MethodInfo getMethod = propertyInfo.GetGetMethod();
				if (!patchedMethods.Contains(getMethod) 
				    && (getMethod != null))
				{
					MelonDebug.Msg($"Patching {targetType.Name}.{propertyInfo.Name} Get Method...");
					patchedMethods.Add(getMethod);
					try
					{
						Core.HarmonyInstance.Patch(getMethod, PatchMethod_Get);
					}
					catch (Exception ex)
					{
						MelonLogger.Warning($"Exception Patching {targetType.Name}.{propertyInfo.Name} Get Method: {ex}");
					}
				}

				MethodInfo setMethod = propertyInfo.GetSetMethod();
				if (!patchedMethods.Contains(setMethod) 
				    && (setMethod != null))
				{
					MelonDebug.Msg($"Patching {targetType.Name}.{propertyInfo.Name} Set Method...");
					patchedMethods.Add(setMethod);
					try
					{
						Core.HarmonyInstance.Patch(setMethod, PatchMethod_Set);
					}
					catch (Exception ex)
					{
						MelonLogger.Warning($"Exception Patching {targetType.Name}.{propertyInfo.Name} Set Method: {ex}");
					}
				}
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