using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader.MonoInternals;
using IllusionPlugin;
using System.IO;
using IllusionInjector;
using MelonLoader.Modules;

namespace MelonLoader.CompatibilityLayers
{
    internal class IPA_Module : MelonModule
    {
        public override void OnInitialize()
        {
			// To-Do:
			// Detect if IPA is already Installed
			// Point AssemblyResolveInfo to already installed IPA Assembly
			// Point GetResolverFromAssembly to Dummy MelonCompatibilityLayer.Resolver

			string[] assembly_list =
			{
				"IllusionPlugin",
				"IllusionInjector",
			};
			Assembly base_assembly = typeof(IPA_Module).Assembly;
			foreach (string assemblyName in assembly_list)
				MonoResolveManager.GetAssemblyResolveInfo(assemblyName).Override = base_assembly;

			MelonBase.CustomMelonResolvers += Resolve;
		}

        private MelonBase[] Resolve(Assembly asm)
		{
			IEnumerable<Type> pluginTypes = asm.GetValidTypes(x =>
			{
				Type[] interfaces = x.GetInterfaces();
				return (interfaces != null) && interfaces.Any() && interfaces.Contains(typeof(IPlugin)); // To-Do: Change to Type Reflection based on Setup
			});
			if ((pluginTypes == null) || !pluginTypes.Any())
				return null;

			return pluginTypes.Select(x => LoadPlugin(asm, x)).ToArray();
		}

		private MelonBase LoadPlugin(Assembly asm, Type pluginType)
		{
			IPlugin pluginInstance = Activator.CreateInstance(pluginType) as IPlugin;

			MelonProcessAttribute[] processAttrs = null;
			if (pluginInstance is IEnhancedPlugin enPl)
				processAttrs = enPl.Filter?.Select(x => new MelonProcessAttribute(x)).ToArray();

			string pluginName = pluginInstance.Name;
			if (string.IsNullOrEmpty(pluginName))
				pluginName = pluginType.FullName;

			string plugin_version = pluginInstance.Version;
			if (string.IsNullOrEmpty(plugin_version))
				plugin_version = asm.GetName().Version.ToString();
			if (string.IsNullOrEmpty(plugin_version) || plugin_version.Equals("0.0.0.0"))
				plugin_version = "1.0.0.0";

			var melon = MelonBase.CreateWrapper<IPAPluginWrapper>(asm, pluginName, plugin_version, processes: processAttrs);

			melon.pluginInstance = pluginInstance;
			PluginManager._Plugins.Add(pluginInstance);
			return melon;
		}
	}
}