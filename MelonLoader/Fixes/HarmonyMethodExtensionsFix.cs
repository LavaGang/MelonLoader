using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace MelonLoader.Fixes
{
	internal static class HarmonyMethodExtensionsFix
    {
        private static MethodInfo HarmonyMethodExtensions_SetValue;

        static HarmonyMethodExtensionsFix()
            => HarmonyMethodExtensions_SetValue = typeof(HarmonyMethodExtensions).GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Static);

        internal static void Install()
		{
            Type FixType = typeof(HarmonyMethodExtensionsFix);
            Type HarmonyMethodType = typeof(HarmonyMethod);
            Type ExtensionsType = typeof(HarmonyMethodExtensions);

            MethodInfo HarmonyMethod_Merge_Original = HarmonyMethodType.GetMethod("Merge", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo HarmonyMethod_Merge_PatchMethod = FixType.GetMethod("HarmonyMethod_Merge_Patch", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo HarmonyMethod_ToString_Original = HarmonyMethodType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo HarmonyMethod_ToString_PatchMethod = FixType.GetMethod("HarmonyMethod_ToString_Patch", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo HarmonyMethodExtensions_Merge_Original = ExtensionsType.GetMethod("Merge", BindingFlags.Public | BindingFlags.Static);
            MethodInfo HarmonyMethodExtensions_Merge_PatchMethod = FixType.GetMethod("HarmonyMethodExtensions_Merge_Patch", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo HarmonyMethodExtensions_CopyTo_Original = ExtensionsType.GetMethod("CopyTo", BindingFlags.Public | BindingFlags.Static);
            MethodInfo HarmonyMethodExtensions_CopyTo_PatchMethod = FixType.GetMethod("HarmonyMethodExtensions_CopyTo_Patch", BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                Core.HarmonyInstance.Patch(HarmonyMethod_Merge_Original, HarmonyMethod_Merge_PatchMethod.ToNewHarmonyMethod());
                Core.HarmonyInstance.Patch(HarmonyMethod_ToString_Original, HarmonyMethod_ToString_PatchMethod.ToNewHarmonyMethod());
                Core.HarmonyInstance.Patch(HarmonyMethodExtensions_Merge_Original, HarmonyMethodExtensions_Merge_PatchMethod.ToNewHarmonyMethod());
                Core.HarmonyInstance.Patch(HarmonyMethodExtensions_CopyTo_Original, HarmonyMethodExtensions_CopyTo_PatchMethod.ToNewHarmonyMethod());
            }
            catch (Exception ex) { MelonLogger.Warning($"HarmonyMethodExtensionsFix Exception: {ex}"); }
        }

        // Modified from HarmonyX's HarmonyLib.HarmonyMethod.ImportMethod : https://github.com/BepInEx/HarmonyX/blob/master/Harmony/Public/HarmonyMethod.cs#L68
        internal static void HarmonyMethod_ImportMethod(HarmonyMethod _this, MethodInfo theMethod)
        {
            _this.method = theMethod;
            if (_this.method is object)
            {
                var infos = HarmonyMethodExtensions.GetFromMethod(_this.method);
                if (infos is object)
                    HarmonyMethodExtensions_CopyTo(HarmonyMethod_Merge(infos), _this);
            }
        }

        // Modified from HarmonyX's HarmonyLib.HarmonyMethod.Merge : https://github.com/BepInEx/HarmonyX/blob/master/Harmony/Public/HarmonyMethod.cs#L140
        private static bool HarmonyMethod_Merge_Patch(IEnumerable<HarmonyMethod> __0, ref HarmonyMethod __result) { __result = HarmonyMethod_Merge(__0); return false; }
        internal static HarmonyMethod HarmonyMethod_Merge(IEnumerable<HarmonyMethod> attributes)
        {
            var result = new HarmonyMethod();
            if (attributes is null) return result;
            var resultTrv = Traverse.Create(result);
            attributes.Do(attribute =>
            {
                var trv = Traverse.Create(attribute);
                HarmonyMethod.HarmonyFields().ForEachElement(f =>
                {
                    var val = trv.Field(f).GetValue();
                    if (val is object && (f != nameof(HarmonyMethod.priority) || (int)val != -1))
                        HarmonyMethodExtensions_SetValue.Invoke(null, new object[] { resultTrv, f, val });
                });
            });
            return result;
        }

        // Modified from HarmonyX's HarmonyLib.HarmonyMethod.ToString : https://github.com/BepInEx/HarmonyX/blob/master/Harmony/Public/HarmonyMethod.cs#L164
        private static bool HarmonyMethod_ToString_Patch(HarmonyMethod __instance, string __result) { __result = HarmonyMethod_ToString(__instance); return false; }
        internal static string HarmonyMethod_ToString(HarmonyMethod __this)
        {
            var result = "";
            var trv = Traverse.Create(__this);
            HarmonyMethod.HarmonyFields().ForEachElement(f =>
            {
                if (result.Length > 0) result += ", ";
                result += $"{f}={trv.Field(f).GetValue()}";
            });
            return $"HarmonyMethod[{result}]";
        }

        // Modified from HarmonyX's HarmonyLib.HarmonyMethodExtensions.CopyTo : https://github.com/BepInEx/HarmonyX/blob/master/Harmony/Public/HarmonyMethod.cs#L225
        private static bool HarmonyMethodExtensions_CopyTo_Patch(HarmonyMethod __0, HarmonyMethod __1) { HarmonyMethodExtensions_CopyTo(__0, __1); return false; }
        internal static void HarmonyMethodExtensions_CopyTo(HarmonyMethod from, HarmonyMethod to)
        {
            if (to is null) return;
            var fromTrv = Traverse.Create(from);
            var toTrv = Traverse.Create(to);
            HarmonyMethod.HarmonyFields().ForEachElement(f =>
            {
                var val = fromTrv.Field(f).GetValue();
                if (val is object)
                    HarmonyMethodExtensions_SetValue.Invoke(null, new object[] { toTrv, f, val });
            });
        }

        // Modified from HarmonyX's HarmonyLib.HarmonyMethodExtensions.Merge : https://github.com/BepInEx/HarmonyX/blob/master/Harmony/Public/HarmonyMethod.cs#L254
        internal static bool HarmonyMethodExtensions_Merge_Patch(HarmonyMethod __0, HarmonyMethod __1, HarmonyMethod __result) { __result = HarmonyMethodExtensions_Merge(__0, __1); return false; }
        internal static HarmonyMethod HarmonyMethodExtensions_Merge(HarmonyMethod master, HarmonyMethod detail)
        {
            if (detail is null) return master;
            var result = new HarmonyMethod();
            var resultTrv = Traverse.Create(result);
            var masterTrv = Traverse.Create(master);
            var detailTrv = Traverse.Create(detail);
            HarmonyMethod.HarmonyFields().ForEachElement(f =>
            {
                var baseValue = masterTrv.Field(f).GetValue();
                var detailValue = detailTrv.Field(f).GetValue();
                if (f != nameof(HarmonyMethod.priority) || (int)detailValue != -1)
                    HarmonyMethodExtensions_SetValue.Invoke(null, new object[] { resultTrv, f, detailValue ?? baseValue });
            });
            return result;
        }
    }
}