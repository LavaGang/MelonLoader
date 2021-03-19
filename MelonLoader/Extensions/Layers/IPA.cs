using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IllusionPlugin;
using IllusionInjector;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
	internal class IPA_CL : MelonCompatibilityLayer.Resolver
	{
		private Type[] plugin_types = null;
		private Assembly asm = null;
		private IPA_CL(Assembly assembly, IEnumerable<Type> types) { asm = assembly; plugin_types = Enumerable.ToArray(types); }

		internal static void Setup(AppDomain domain)
		{
			domain.AssemblyResolve += (sender, args) =>
				(args.Name.StartsWith("IllusionPlugin, Version=")
				|| args.Name.StartsWith("IllusionInjector, Version="))
				? typeof(IPA_CL).Assembly
				: null;
			MelonCompatibilityLayer.ResolveAssemblyToLayerResolverEvents += ResolveAssemblyToLayerResolver;
			MelonCompatibilityLayer.RefreshModsTableEvents += (ds, da) => RefreshModsTable();
		}

		private static void ResolveAssemblyToLayerResolver(object sender, MelonCompatibilityLayer.LayerResolveEventArgs args)
		{
			if (args.inter != null)
				return;
			IEnumerable<Type> plugin_types = args.assembly.GetTypes().Where(x => (x.GetInterface("IPlugin") != null));
			if ((plugin_types == null)
				|| (plugin_types.Count() <= 0))
				return;
			args.inter = new IPA_CL(args.assembly, plugin_types);
		}

		private static void RefreshModsTable()
		{
			// PluginManager Conversion Here
		}

		internal override void CheckAndCreate(string filelocation, bool is_plugin, ref List<MelonBase> melonTbl)
		{
			if (string.IsNullOrEmpty(filelocation))
				filelocation = asm.GetName().Name;

			if (is_plugin)
			{
				MelonLogger.Error($"Mod in Plugins Folder: {filelocation}");
				return;
			}

			foreach (Type plugin_type in plugin_types)
				LoadPlugin(plugin_type, filelocation, ref melonTbl);
		}

		private void LoadPlugin(Type plugin_type, string filelocation, ref List<MelonBase> melonTbl)
        {
			IPlugin pluginInstance = Activator.CreateInstance(plugin_type) as IPlugin;

			string[] filter = null;
			if (pluginInstance is IEnhancedPlugin)
				filter = ((IEnhancedPlugin)pluginInstance).Filter;

			List<MelonGameAttribute> gamestbl = null;
			if ((filter != null) && (filter.Count() > 0))
			{
				string exe_name = Path.GetFileNameWithoutExtension(string.Copy(MelonUtils.GetApplicationPath()));
				gamestbl = new List<MelonGameAttribute>();
				bool game_found = false;
				foreach (string x in filter)
				{
					if (string.IsNullOrEmpty(x))
						continue;
					gamestbl.Add(new MelonGameAttribute(name: x));
					if (x.Equals(exe_name))
						game_found = true;
				}
				if (!game_found)
				{
					MelonLogger.Error($"Incompatible Game for Mod: {filelocation}");
					return;
				}
			}

			string plugin_name = pluginInstance.Name;
			if (string.IsNullOrEmpty(plugin_name))
				plugin_name = plugin_type.FullName;
			
			if (MelonHandler.IsModAlreadyLoaded(plugin_name))
			{
				MelonLogger.Error($"Duplicate Mod {plugin_name}: {filelocation}");
				return;
			}

			string plugin_version = pluginInstance.Version;
			if (string.IsNullOrEmpty(plugin_version))
				plugin_version = asm.GetName().Version.ToString();
			if (string.IsNullOrEmpty(plugin_version) || plugin_version.Equals("0.0.0.0"))
				plugin_version = "1.0.0.0";

			MelonModWrapper wrapper = new MelonModWrapper(pluginInstance);
			wrapper.Info = new MelonInfoAttribute(typeof(MelonModWrapper), plugin_name, plugin_version);
			if (gamestbl != null)
				wrapper.Games = gamestbl.ToArray();
			wrapper.ConsoleColor = MelonLogger.DefaultMelonColor;
			wrapper.Priority = 0;
			wrapper.Location = filelocation;
			wrapper.Assembly = asm;
			wrapper.Harmony = Harmony.HarmonyInstance.Create(asm.FullName);

			melonTbl.Add(wrapper);
			PluginManager._Plugins.Add(pluginInstance);
		}

		private class MelonModWrapper : MelonMod
		{
			private IPlugin pluginInstance;
			internal MelonModWrapper(IPlugin plugin) => pluginInstance = plugin;
			public override void OnApplicationStart() => pluginInstance.OnApplicationStart();
			public override void OnApplicationQuit() => pluginInstance.OnApplicationQuit();
			public override void OnSceneWasLoaded(int buildIndex, string sceneName) => pluginInstance.OnLevelWasLoaded(buildIndex);
			public override void OnSceneWasInitialized(int buildIndex, string sceneName) => pluginInstance.OnLevelWasInitialized(buildIndex);
			public override void OnUpdate() => pluginInstance.OnUpdate();
			public override void OnFixedUpdate() => pluginInstance.OnFixedUpdate();
			public override void OnLateUpdate() { if (pluginInstance is IEnhancedPlugin) ((IEnhancedPlugin)pluginInstance).OnLateUpdate(); }
		}
	}
}