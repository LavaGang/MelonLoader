using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IllusionPlugin;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
	internal class IPA : MelonCompatibilityLayerResolver
	{
		private Type plugin_type = null;
		private IPA(Type type) => plugin_type = type;

		internal static void Register()
        {
			MelonCompatibilityLayer.LayerResolveEvents += TryResolve;
			MelonCompatibilityLayer.AssemblyResolveEvents += AddAssemblyResolver;
		}

		private static void AddAssemblyResolver(object ds, EventArgs da) => AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
		{
			if (args.Name.StartsWith("IllusionPlugin, Version=")
				|| args.Name.StartsWith("IllusionInjector, Version="))
				return typeof(IPA).Assembly;
			return null;
		};

		private static void TryResolve(object sender, MelonCompatibilityLayerResolverEventArgs args)
		{
			if (args.inter != null)
				return;
			Type plugin_type = args.assembly.GetTypes().FirstOrDefault(x => (x.GetInterface("IPlugin") != null));
			if (plugin_type == null)
				return;
			args.inter = new IPA(plugin_type);
		}

		internal override bool CheckAndCreate(Assembly asm, string filelocation, bool is_plugin, ref MelonBase baseInstance)
		{
			if (is_plugin)
            {
				// Error Here | IPA Plugins can Only be Loaded as Mods
				return false;
            }

			if (string.IsNullOrEmpty(filelocation))
				filelocation = asm.GetName().Name;

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
					// Error Here
					return false;
                }
            }

			string plugin_name = pluginInstance.Name;
			if (string.IsNullOrEmpty(plugin_name))
				plugin_name = plugin_type.FullName;

			bool isAlreadyLoaded = (is_plugin ? MelonHandler.IsPluginAlreadyLoaded(plugin_name) : MelonHandler.IsModAlreadyLoaded(plugin_name));
			if (isAlreadyLoaded)
			{
				MelonLogger.Warning("Duplicate File: " + filelocation);
				return false;
			}

			string plugin_version = pluginInstance.Version;
			if (string.IsNullOrEmpty(plugin_version))
				plugin_version = asm.GetName().Version.ToString();
			if (string.IsNullOrEmpty(plugin_version) || plugin_version.Equals("0.0.0.0"))
				plugin_version = "1.0.0.0";

			IPA_MelonModWrapper wrapper = new IPA_MelonModWrapper(pluginInstance);
			wrapper.Info = new MelonInfoAttribute(typeof(IPA_MelonModWrapper), plugin_name, plugin_version, null);
			if (gamestbl != null)
				wrapper.Games = gamestbl.ToArray();
			wrapper.ConsoleColor = MelonLogger.DefaultMelonColor;
			wrapper.Priority = 0;
			wrapper.Location = filelocation;
			wrapper.Assembly = asm;
			wrapper.Harmony = Harmony.HarmonyInstance.Create(asm.FullName);

			baseInstance = wrapper;
			IllusionInjector.PluginManager._Plugins.Add(pluginInstance);
			return true;
		}
	}

	internal class IPA_MelonModWrapper : MelonMod
	{
		private IPlugin pluginInstance;
		internal IPA_MelonModWrapper(IPlugin plugin) => pluginInstance = plugin;
		public override void OnApplicationStart() => pluginInstance.OnApplicationStart();
		public override void OnApplicationQuit() => pluginInstance.OnApplicationQuit();
		public override void OnSceneWasLoaded(int buildIndex, string sceneName) => pluginInstance.OnLevelWasLoaded(buildIndex);
		public override void OnSceneWasInitialized(int buildIndex, string sceneName) => pluginInstance.OnLevelWasInitialized(buildIndex);
		public override void OnUpdate() => pluginInstance.OnUpdate();
		public override void OnFixedUpdate() => pluginInstance.OnFixedUpdate();
		public override void OnLateUpdate() { if (pluginInstance is IEnhancedPlugin) ((IEnhancedPlugin)pluginInstance).OnLateUpdate(); }
	}
}
