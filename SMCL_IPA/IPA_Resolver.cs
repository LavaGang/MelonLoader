using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IllusionPlugin;
using IllusionInjector;
using MelonLoader.Attributes;
using MelonLoader.Melons;
using MelonLoader.Utils;

#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
	internal class IPA_Resolver : MelonCompatibilityLayer.Resolver
	{
		private readonly Type[] plugin_types = null;

		internal IPA_Resolver(Assembly assembly, string filepath, IEnumerable<Type> types) : base(assembly, filepath)
			=> plugin_types = types.ToArray();

		public override void CheckAndCreate(ref List<MelonBase> melonTbl)
		{
			foreach (Type plugin_type in plugin_types)
				LoadPlugin(plugin_type, ref melonTbl);
		}

		private void LoadPlugin(Type plugin_type, ref List<MelonBase> melonTbl)
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
					MelonLogger.Error($"Incompatible Game for {FilePath}");
					return;
				}
			}

			string plugin_name = pluginInstance.Name;
			if (string.IsNullOrEmpty(plugin_name))
				plugin_name = plugin_type.FullName;

			if (MelonHandler.IsModAlreadyLoaded(plugin_name))
			{
				MelonLogger.Error($"Duplicate File {plugin_name}: {FilePath}");
				return;
			}

			string plugin_version = pluginInstance.Version;
			if (string.IsNullOrEmpty(plugin_version))
				plugin_version = Assembly.GetName().Version.ToString();
			if (string.IsNullOrEmpty(plugin_version) || plugin_version.Equals("0.0.0.0"))
				plugin_version = "1.0.0.0";

			MelonModWrapper wrapper = new MelonCompatibilityLayer.WrapperData()
			{
				Assembly = Assembly,
				Info = new MelonInfoAttribute(typeof(MelonModWrapper), plugin_name, plugin_version),
				Games = gamestbl?.ToArray(),
				Priority = 0,
				Location = FilePath
			}.CreateMelon<MelonModWrapper>();
			if (wrapper == null)
				return;

			wrapper.pluginInstance = pluginInstance;
			melonTbl.Add(wrapper);
			PluginManager._Plugins.Add(pluginInstance);
		}

		private class MelonModWrapper : MelonMod
		{
			internal IPlugin pluginInstance;
			public override void OnApplicationStart() => pluginInstance.OnApplicationStart();
			public override void OnApplicationQuit() => pluginInstance.OnApplicationQuit();
			public override void OnSceneWasLoaded(int buildIndex, string sceneName) => pluginInstance.OnLevelWasLoaded(buildIndex);
			public override void OnSceneWasInitialized(int buildIndex, string sceneName) => pluginInstance.OnLevelWasInitialized(buildIndex);
			public override void OnUpdate() => pluginInstance.OnUpdate();
			public override void OnFixedUpdate() => pluginInstance.OnFixedUpdate();
			public override void OnLateUpdate() { if (pluginInstance is IEnhancedPlugin plugin) plugin.OnLateUpdate(); }
		}
	}
}