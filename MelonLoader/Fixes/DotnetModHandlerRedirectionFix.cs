#if NET6_0_OR_GREATER
using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.Loader;

namespace MelonLoader.Fixes
{
    internal class DotnetModHandlerRedirectionFix
    {
        public static void Install()
        {
            try
            {
                Core.HarmonyInstance.Patch(typeof(AssemblyLoadContext).GetMethod("ValidateAssemblyNameWithSimpleName", BindingFlags.Static | BindingFlags.NonPublic),
                    new HarmonyMethod(typeof(DotnetModHandlerRedirectionFix), nameof(PreValidateAssembly)));
            }
            catch (Exception ex) { MelonLogger.Warning($"DotnetModHandlerRedirectionFix Exception: {ex}"); }
        }

        public static bool PreValidateAssembly(Assembly assembly, string requestedSimpleName, ref Assembly __result)
        {
            if(requestedSimpleName.Contains("MelonLoader.ModHandler"))
            {
                __result = assembly;
                return false; //Don't validate the name. What could go wrong?
            }

            return true;
        }
    }
}

#endif