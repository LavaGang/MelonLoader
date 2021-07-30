using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IllusionPlugin;

#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
    internal class IPA_Module : MelonCompatibilityLayer.Module
    {
        public override void Setup()
        {
			// To-Do:
			// Detect if IPA is already Installed
			// Point domain.AssemblyResolve to already installed IPA Assembly
			// Point GetResolverFromAssembly to Dummy MelonCompatibilityLayer.Resolver

			Assembly base_assembly = typeof(IPA_Module).Assembly;
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
				new AssemblyName(args.Name).Name switch
				{
					"IllusionPlugin" => base_assembly,
					"IllusionInjector" => base_assembly,
					_ => null,
				};

			MelonCompatibilityLayer.AddAssemblyToResolverEvent(GetResolverFromAssembly);
		}

		private static MelonCompatibilityLayer.Resolver GetResolverFromAssembly(Assembly assembly, string filepath)
		{
			IEnumerable<Type> plugin_types = assembly.GetValidTypes(x =>
			{
				Type[] interfaces = x.GetInterfaces();
				return interfaces != null && interfaces.Any() && interfaces.Contains(typeof(IPlugin)); // To-Do: Change to Type Reflection based on Setup
			});
			if (plugin_types == null || !plugin_types.Any())
				return null;

			return new IPA_Resolver(assembly, filepath, plugin_types);
		}
	}
}