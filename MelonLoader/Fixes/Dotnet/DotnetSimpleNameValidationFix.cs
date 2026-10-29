#if NET6_0_OR_GREATER
using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.Loader;
using MelonLoader.Resolver;

namespace MelonLoader.Fixes.Dotnet
{
    internal class DotnetSimpleNameValidationFix
    {
        internal static bool Error { get; private set; }
        
        public static void Install()
        {
            try
            {
                Core.HarmonyInstance.Patch(
                    typeof(AssemblyLoadContext).GetMethod("ValidateAssemblyNameWithSimpleName",
                        BindingFlags.Static | BindingFlags.NonPublic),
                    new HarmonyMethod(typeof(DotnetSimpleNameValidationFix), nameof(PreValidateAssembly)));
            }
            catch (Exception ex)
            {
                MelonLogger.Warning(ex);
                Error = true;
            }
        }

        public static bool PreValidateAssembly(Assembly assembly, string requestedSimpleName, ref Assembly __result)
        {
            var info = MelonAssemblyResolver.GetAssemblyResolveInfo(requestedSimpleName);
            if ((info.Override == assembly)
                || (info.Fallback == assembly))
            {
                __result = assembly;
                return false;
            }

            if ((info.Versions != null)
                && (info.Versions.Count > 0))
                foreach (var version in info.Versions)
                    if (version.Value == assembly)
                    {
                        __result = assembly;
                        return false;
                    }

            return true;
        }
    }
}

#endif