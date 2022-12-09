#if NET6_0
using HarmonyLib;
using System.Reflection;
using System.Runtime.Loader;

namespace MelonLoader.Fixes
{
    internal class DotnetModHandlerRedirectionFix
    {
        public static void Install()
        {
            Core.HarmonyInstance.Patch(typeof(AssemblyLoadContext).GetMethod("ValidateAssemblyNameWithSimpleName", BindingFlags.Static | BindingFlags.NonPublic), new HarmonyMethod(typeof(DotnetModHandlerRedirectionFix), nameof(PreValidateAssembly)));
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