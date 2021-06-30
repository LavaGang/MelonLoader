using System;
using System.Collections.Generic;
using System.Linq;
using IllusionPlugin;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
    internal class IPA_Module : MelonCompatibilityLayer.Module
    {
        public void Setup(AppDomain domain)
        {
			// To-Do:
			// Detect if IPA is already Installed
			// Point domain.AssemblyResolve to already installed IPA Assembly
			// Point ResolveAssemblyToLayerResolver to Dummy MelonCompatibilityLayer.Resolver

			domain.AssemblyResolve += (sender, args) =>
				(args.Name.StartsWith("IllusionPlugin, Version=")
				|| args.Name.StartsWith("IllusionInjector, Version="))
				? typeof(IPA_Module).Assembly
				: null;
			MelonCompatibilityLayer.AddResolveAssemblyToLayerResolverEvent(ResolveAssemblyToLayerResolver);
		}

		private static void ResolveAssemblyToLayerResolver(MelonCompatibilityLayer.LayerResolveEventArgs args)
		{
			if (args.inter != null)
				return;

			IEnumerable<Type> plugin_types = args.assembly.GetValidTypes(x =>
			{
				Type[] interfaces = x.GetInterfaces();
				return (interfaces != null) && interfaces.Any() && interfaces.Contains(typeof(IPlugin)); // To-Do: Change to Type Reflection based on Setup
			});
			if ((plugin_types == null) || !plugin_types.Any())
				return;

			args.inter = new IPA_Resolver(args.assembly, args.filepath, plugin_types);
		}
	}
}