using System;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace MelonLoader.Fixes.MonoMod
{
    internal static class InstancePatchFix
    {
        internal static void Install()
        {
            Hook.OnDetour += (detour, originalMethod, patchMethod, delegateTarget) => EnsureStatic(patchMethod);
            Detour.OnDetour += (detour, originalMethod, patchMethod) => EnsureStatic(patchMethod);
        }

        private static bool EnsureStatic(MethodBase method)
        {
            if (!method.IsStatic)
                throw new Exception("Patch Method must be a Static Method!");
            return true;
        }
    }
}
