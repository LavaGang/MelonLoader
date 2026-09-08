using System;
using System.Reflection;
using HarmonyLib;
using MonoMod.RuntimeDetour;

namespace MelonLoader.Fixes.Harmony
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
            catch (Exception ex) { MelonLogger.Warning(ex); }

            try
            {
                Core.HarmonyInstance.Patch(AccessTools.Method(typeof(Hook), "Apply"),
                    AccessTools.Method(instancePatchFixType, "PatchHookApply").ToNewHarmonyMethod());
            }
            catch (Exception ex) { MelonLogger.Warning(ex); }
        }

        private static bool PatchMethod(MethodBase __0)
        {
            if (__0 == null)
                throw new NullReferenceException("Patch Method");
            if (__0 != null && !__0.IsStatic)
                throw new Exception("Patch Method must be a Static Method!");
            return true;
        }

        private static bool PatchHookApply(Hook __instance) => PatchMethod(__instance.Target);
    }
}