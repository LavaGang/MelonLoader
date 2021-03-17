using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IllusionPlugin;
using IllusionInjector;
using MelonLoader;
#pragma warning disable 0618

namespace MelonLoader.CompatibilityLayers
{
	internal class IPA : MelonCompatibilityLayerResolver
	{
		private Type[] plugin_types = null;
		private Assembly asm = null;
		private IPA(Assembly assembly, IEnumerable<Type> types) { asm = assembly; plugin_types = Enumerable.ToArray(types); }

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
			IEnumerable<Type> plugin_types = args.assembly.GetTypes().Where(x => (x.GetInterface("IPlugin") != null));
			if ((plugin_types == null)
				|| (plugin_types.Count() <= 0))
				return;
			args.inter = new IPA(args.assembly, plugin_types);
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
				LoadPlugin(plugin_type, asm, filelocation, ref melonTbl);
		}

		private void LoadPlugin(Type plugin_type, Assembly asm, string filelocation, ref List<MelonBase> melonTbl)
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

			IPA_MelonModWrapper wrapper = new IPA_MelonModWrapper(pluginInstance);
			wrapper.Info = new MelonInfoAttribute(typeof(IPA_MelonModWrapper), plugin_name, plugin_version);
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

#region Redirect
namespace IllusionInjector
{
	[Obsolete("IllusionInjector.PluginManager is Only Here for Compatibility Reasons. Please use MelonHandler instead.")]
	public static class PluginManager
	{
		internal static List<IPlugin> _Plugins = new List<IPlugin>();
		[Obsolete("IllusionInjector.PluginManager.Plugins is Only Here for Compatibility Reasons. Please use MelonHandler.Mods instead.")]
		public static IEnumerable<IPlugin> Plugins { get => _Plugins; }
		[Obsolete("IllusionInjector.PluginManager.AppInfo is Only Here for Compatibility Reasons. Please use MelonUtils instead.")]
		public class AppInfo
		{
			[Obsolete("IllusionInjector.PluginManager.AppInfo.StartupPath is Only Here for Compatibility Reasons. Please use MelonUtils.GameDirectory instead.")]
			public static string StartupPath { get => MelonUtils.GameDirectory; }
		}
	}
}

namespace IllusionPlugin
{
	[Obsolete("IllusionPlugin.IPlugin is Only Here for Compatibility Reasons. Please use MelonPlugin or MelonMod instead.")]
	public interface IPlugin
	{
		[Obsolete("IllusionPlugin.IPlugin.Name is Only Here for Compatibility Reasons. Please use MelonPlugin.Info.Name or MelonMod.Info.Name instead.")]
		string Name { get; }
		[Obsolete("IllusionPlugin.IPlugin.Version is Only Here for Compatibility Reasons. Please use MelonPlugin.Info.Version or MelonMod.Info.Version instead.")]
		string Version { get; }
		[Obsolete("IllusionPlugin.IPlugin.OnApplicationStart() is Only Here for Compatibility Reasons. Please use MelonPlugin.OnApplicationStart() or MelonMod.OnApplicationStart() instead.")]
		void OnApplicationStart();
		[Obsolete("IllusionPlugin.IPlugin.OnApplicationQuit() is Only Here for Compatibility Reasons. Please use MelonPlugin.OnApplicationQuit() or MelonMod.OnApplicationQuit() instead.")]
		void OnApplicationQuit();
		[Obsolete("IllusionPlugin.IPlugin.OnLevelWasLoaded(int) is Only Here for Compatibility Reasons. Please use MelonPlugin.OnSceneWasLoaded(int, string) or MelonMod.OnSceneWasLoaded(int, string) instead.")]
		void OnLevelWasLoaded(int level);
		[Obsolete("IllusionPlugin.IPlugin.OnLevelWasInitialized(int) is Only Here for Compatibility Reasons. Please use MelonPlugin.OnSceneWasInitialized(int, string) or MelonMod.OnSceneWasInitialized(int, string) instead.")]
		void OnLevelWasInitialized(int level);
		[Obsolete("IllusionPlugin.IPlugin.OnUpdate() is Only Here for Compatibility Reasons. Please use MelonPlugin.OnUpdate() or MelonMod.OnUpdate() instead.")]
		void OnUpdate();
		[Obsolete("IllusionPlugin.IPlugin.OnFixedUpdate() is Only Here for Compatibility Reasons. Please use MelonMod.OnFixedUpdate() instead.")]
		void OnFixedUpdate();
	}

	[Obsolete("IllusionPlugin.IEnhancedPlugin is Only Here for Compatibility Reasons. Please use MelonPlugin or MelonMod instead.")]
	public interface IEnhancedPlugin : IPlugin
	{
		[Obsolete("IllusionPlugin.IEnhancedPlugin.Filter is Only Here for Compatibility Reasons. Please use the MelonGame Attribute instead.")]
		string[] Filter { get; }
		[Obsolete("IllusionPlugin.IEnhancedPlugin.OnLateUpdate() is Only Here for Compatibility Reasons. Please use MelonPlugin.OnLateUpdate() or MelonMod.OnLateUpdate() instead.")]
		void OnLateUpdate();
	}

	[Obsolete("IllusionPlugin.ModPrefs is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
	public static class ModPrefs
	{
		[Obsolete("IllusionPlugin.ModPrefs.GetString is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
		public static string GetString(string section, string name, string defaultValue = "", bool autoSave = false)
		{
			MelonPreferences_Category category = MelonPreferences.GetCategory(section);
			if (category == null)
				category = MelonPreferences.CreateCategory(section);
			MelonPreferences_Entry<string> entry = category.GetEntry<string>(name);
			if (entry == null)
				entry = category.CreateEntry(name, defaultValue);
			return entry.Value;
		}
		[Obsolete("IllusionPlugin.ModPrefs.GetInt is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
		public static int GetInt(string section, string name, int defaultValue = 0, bool autoSave = false)
		{
			MelonPreferences_Category category = MelonPreferences.GetCategory(section);
			if (category == null)
				category = MelonPreferences.CreateCategory(section);
			MelonPreferences_Entry<int> entry = category.GetEntry<int>(name);
			if (entry == null)
				entry = category.CreateEntry(name, defaultValue);
			return entry.Value;
		}
		[Obsolete("IllusionPlugin.ModPrefs.GetFloat is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
		public static float GetFloat(string section, string name, float defaultValue = 0f, bool autoSave = false)
		{
			MelonPreferences_Category category = MelonPreferences.GetCategory(section);
			if (category == null)
				category = MelonPreferences.CreateCategory(section);
			MelonPreferences_Entry<float> entry = category.GetEntry<float>(name);
			if (entry == null)
				entry = category.CreateEntry(name, defaultValue);
			return entry.Value;
		}
		[Obsolete("IllusionPlugin.ModPrefs.GetBool is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
		public static bool GetBool(string section, string name, bool defaultValue = false, bool autoSave = false)
		{
			MelonPreferences_Category category = MelonPreferences.GetCategory(section);
			if (category == null)
				category = MelonPreferences.CreateCategory(section);
			MelonPreferences_Entry<bool> entry = category.GetEntry<bool>(name);
			if (entry == null)
				entry = category.CreateEntry(name, defaultValue);
			return entry.Value;
		}
		[Obsolete("IllusionPlugin.ModPrefs.HasKey is Only Here for Compatibility Reasons. Please use MelonPreferences.HasEntry instead.")]
		public static bool HasKey(string section, string name) => MelonPreferences.HasEntry(section, name);
		[Obsolete("IllusionPlugin.ModPrefs.SetFloat is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
		public static void SetFloat(string section, string name, float value)
		{
			MelonPreferences_Category category = MelonPreferences.GetCategory(section);
			if (category == null)
				category = MelonPreferences.CreateCategory(section);
			MelonPreferences_Entry<float> entry = category.GetEntry<float>(name);
			if (entry == null)
				entry = category.CreateEntry(name, value);
			entry.Value = value;
		}
		[Obsolete("IllusionPlugin.ModPrefs.SetInt is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
		public static void SetInt(string section, string name, int value)
		{
			MelonPreferences_Category category = MelonPreferences.GetCategory(section);
			if (category == null)
				category = MelonPreferences.CreateCategory(section);
			MelonPreferences_Entry<int> entry = category.GetEntry<int>(name);
			if (entry == null)
				entry = category.CreateEntry(name, value);
			entry.Value = value;
		}
		[Obsolete("IllusionPlugin.ModPrefs.SetString is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
		public static void SetString(string section, string name, string value)
		{
			MelonPreferences_Category category = MelonPreferences.GetCategory(section);
			if (category == null)
				category = MelonPreferences.CreateCategory(section);
			MelonPreferences_Entry<string> entry = category.GetEntry<string>(name);
			if (entry == null)
				entry = category.CreateEntry(name, value);
			entry.Value = value;
		}
		[Obsolete("IllusionPlugin.ModPrefs.SetBool is Only Here for Compatibility Reasons. Please use MelonPreferences instead.")]
		public static void SetBool(string section, string name, bool value)
		{
			MelonPreferences_Category category = MelonPreferences.GetCategory(section);
			if (category == null)
				category = MelonPreferences.CreateCategory(section);
			MelonPreferences_Entry<bool> entry = category.GetEntry<bool>(name);
			if (entry == null)
				entry = category.CreateEntry(name, value);
			entry.Value = value;
		}
	}
}
#endregion