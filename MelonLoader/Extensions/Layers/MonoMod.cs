using System;

namespace MelonLoader.CompatibilityLayers
{
	internal class MonoMod_CL
	{
		internal static void Setup(AppDomain domain)
		{
			domain.AssemblyResolve += (sender, args) =>
				(args.Name.StartsWith("MonoMod.RuntimeDetour, Version=")
				|| args.Name.StartsWith("MonoMod.Utils, Version="))
				? typeof(MonoMod_CL).Assembly
				: null;
		}
	}
}