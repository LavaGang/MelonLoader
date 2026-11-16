#if NET6_0_OR_GREATER
using HarmonyLib;
using System;
using System.Reflection;
using System.Runtime.Loader;

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
            if((requestedSimpleName == "MelonLoader.ModHandler")
               || (requestedSimpleName == "MelonLoader")
               || (requestedSimpleName == "Il2CppSystem.Private.CoreLib"))
            {
                __result = assembly;
                return false; //Don't validate the name. What could go wrong?
            }

            return true;
        }
    }
}

#endif