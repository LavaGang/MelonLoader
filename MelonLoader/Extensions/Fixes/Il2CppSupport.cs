using System;

namespace MelonLoader.Fixes
{
	internal static class Il2CppSupport
	{
		internal static void Run(AppDomain domain)
		{
			if (!MelonUtils.IsGameIl2Cpp())
				return;

			domain.AssemblyResolve += Il2CppAssemblyGenerator.AssemblyResolver;
			HarmonyLib.Public.Patching.PatchManager.ResolvePatcher += HarmonyIl2CppMethodPatcher.TryResolve;
		}
	}
}