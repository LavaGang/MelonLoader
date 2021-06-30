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

			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
				(args.Name.StartsWith("IllusionPlugin, Version=")
				|| args.Name.StartsWith("IllusionInjector, Version="))
				? typeof(IPA_Module).Assembly
				: null;
		}

		public override MelonCompatibilityLayer.Resolver GetResolverFromAssembly(Assembly assembly, string filepath)
		{
			IEnumerable<Type> plugin_types = assembly.GetValidTypes(x =>
			{
				Type[] interfaces = x.GetInterfaces();
				return (interfaces != null) && interfaces.Any() && interfaces.Contains(typeof(IPlugin)); // To-Do: Change to Type Reflection based on Setup
			});
			if ((plugin_types == null) || !plugin_types.Any())
				return null;

			return new IPA_Resolver(assembly, filepath, plugin_types);
		}
	}
}