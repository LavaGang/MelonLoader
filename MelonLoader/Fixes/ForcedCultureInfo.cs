using HarmonyLib;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MelonLoader.Fixes;

internal static class ForcedCultureInfo
{
    private static readonly CultureInfo SelectedCultureInfo = CultureInfo.InvariantCulture;

    internal static void Install()
    {
        var PatchType = typeof(ForcedCultureInfo);
        var PatchMethod_Get = PatchType.GetMethod("GetMethod", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();
        var PatchMethod_Set = PatchType.GetMethod("SetMethod", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod();

        try
        {
            var CultureInfoType = typeof(CultureInfo);
            var ThreadType = typeof(Thread);

            foreach (var fieldInfo in ThreadType
               .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
               .Where(x => x.FieldType == CultureInfoType))
                fieldInfo.SetValue(null, SelectedCultureInfo);

            foreach (var propertyInfo in ThreadType
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(x => x.PropertyType == CultureInfoType))
            {
                var getMethod = propertyInfo.GetGetMethod();
                if (getMethod != null)
                    try
                    {
                        Core.HarmonyInstance.Patch(getMethod, PatchMethod_Get);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning($"Exception Patching Thread Get Method {propertyInfo.Name}: {ex}");
                    }

                var setMethod = propertyInfo.GetSetMethod();
                if (setMethod != null)
                    try
                    {
                        Core.HarmonyInstance.Patch(setMethod, PatchMethod_Set);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning($"Exception Patching Thread Set Method {propertyInfo.Name}: {ex}");
                    }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"ForcedCultureInfo Exception: {ex}");
        }
    }

    private static bool GetMethod(ref CultureInfo __result)
    {
        __result = SelectedCultureInfo;
        return false;
    }
    private static bool SetMethod() => false;
}